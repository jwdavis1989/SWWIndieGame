Shader "Custom/Water_Tessellated_Aura2"
{
    Properties
    {
        _Color ("Background Color", Color) = (0.1, 0.4, 0.8, 0.8)
        _TextureColor ("Texture Color", Color) = (1, 1, 1, 1)
        _MainTex ("Water Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Float) = 0.5
        _WaveFrequency ("Wave Frequency", Float) = 1
        
        [Header(Tessellation)]
        _Tess ("Tessellation Amount", Range(1,32)) = 15
        _MinDist ("Tess Min Distance", Float) = 5
        _MaxDist ("Tess Max Distance", Float) = 100
        
        [Header(Vertical Waves)]
        _VertWaveHeight ("Height", Float) = 0.5
        _VertWaveFreq ("Frequency", Float) = 0.5
        _VertWaveSpeed ("Speed", Float) = 1.0
        
        [Header(Horizon Fog)]
        _FadeStart ("Fade Start", Float) = 500
        _FadeEnd ("Fade End", Float) = 1000
        _HorizonDarkness ("Darkness", Range(0, 1)) = 0.8

        [Header(Aura 2)]
        _AuraIntensity ("Aura Intensity", Range(0, 1)) = 0.15
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 300

        //PASS 1: WATER SURFACE
        CGPROGRAM
        #pragma surface surf Lambert alpha:fade vertex:vert tessellate:tess
        #pragma target 4.6
        #include "Tessellation.cginc"

        sampler2D _MainTex, _CameraDepthTexture;
        fixed4 _Color, _TextureColor;
        float _WaveSpeed, _WaveFrequency;
        float _VertWaveHeight, _VertWaveFreq, _VertWaveSpeed;
        float _Tess, _MinDist, _MaxDist, _FadeStart, _FadeEnd, _HorizonDarkness;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
            float4 screenPos;
        };

        //GPU Tessellation calculation
        float4 tess (appdata_full v0, appdata_full v1, appdata_full v2) {
            return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, _MinDist, _MaxDist, _Tess);
        }

        //Shared Wave Logic
        float calculateWave(float3 worldPos) {
            return sin(_Time.y * _VertWaveSpeed + (worldPos.x * _VertWaveFreq) + (worldPos.z * _VertWaveFreq * 0.8)) * _VertWaveHeight;
        }

        void vert (inout appdata_full v) {
            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
            v.vertex.y += calculateWave(worldPos);
        }

        void surf (Input IN, inout SurfaceOutput o) {
            //Simple Wave Texture
            float2 uv = IN.uv_MainTex + float2(_Time.x * _WaveSpeed, _Time.x * _WaveSpeed);
            fixed4 c = tex2D(_MainTex, uv) * _TextureColor;
            
            //Distance Fading
            float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
            float horizonFade = saturate((_FadeEnd - dist) / (_FadeEnd - _FadeStart));
            float darkness = lerp(1.0, 1.0 - _HorizonDarkness, 1.0 - horizonFade);
            
            o.Albedo = lerp(_Color.rgb, c.rgb, c.a) * darkness;
            o.Alpha = _Color.a * horizonFade;
        }
        ENDCG

        //PASS 2: AURA VOLUMETRICS
        Pass
        {
            ZWrite Off Blend One One 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.6
            #include "UnityCG.cginc"
            #include "Assets/Aura 2/Core/Code/Shaders/Aura.cginc"

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float _AuraIntensity, _FadeStart, _FadeEnd;
            float _VertWaveHeight, _VertWaveFreq, _VertWaveSpeed;

            v2f vert (appdata_base v) {
                v2f o;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                //Match the vertex displacement exactly
                float wave = sin(_Time.y * _VertWaveSpeed + (worldPos.x * _VertWaveFreq) + (worldPos.z * _VertWaveFreq * 0.8)) * _VertWaveHeight;
                v.vertex.y += wave;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 auraCoords = Aura2_GetFrustumSpaceCoordinates(float4(i.worldPos, 1.0));
                float3 auraResult = float3(0,0,0);
                
                Aura2_ApplyLighting(auraResult, auraCoords, _AuraIntensity);
                Aura2_ApplyFog(auraResult, auraCoords);
                
                float dist = distance(_WorldSpaceCameraPos, i.worldPos);
                float horizonFade = saturate((_FadeEnd - dist) / (_FadeEnd - _FadeStart));
                
                return fixed4(auraResult * horizonFade, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
