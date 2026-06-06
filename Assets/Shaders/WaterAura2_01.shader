Shader "Custom/BasicWaterAura2_02"
{
    Properties
    {
        _Color ("Background Color", Color) = (0.1, 0.4, 0.8, 0.8)
        _TextureColor ("Texture Color", Color) = (1, 1, 1, 1)
        _MainTex ("Water Texture", 2D) = "white" {}
        _WaveSpeed ("Wave Speed", Float) = 0.5
        _WaveStrength ("Wave Strength", Range(0, 0.1)) = 0.01
        _WaveAmount ("Wave Amount", Float) = 0.1
        _WaveFrequency ("Wave Frequency", Float) = 1
        _TextureDistortion ("Texture Distortion", Range(0, 1)) = 0.5
        _FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _FoamAmount ("Foam Amount", Range(0, 1)) = 0.1
        _FoamCutoff ("Foam Cutoff", Range(0, 1)) = 0.5
        _FoamSpeed ("Foam Speed", Float) = 0.1
        _FoamNoiseScale ("Foam Noise Scale", Float) = 20
        _AuraIntensity ("Aura Intensity", Range(0, 1)) = 0.2
        
        //Fading controls
        _FadeStart ("Fog Fade Start", Float) = 500
        _FadeEnd ("Fog Fade End", Float) = 1000
        _HorizonDarkness ("Horizon Darkness", Range(0, 1)) = 1.0 //1 = fully dark at edge
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200

        //PASS 1: WATER
        CGPROGRAM
        #pragma surface surf Lambert alpha:fade
        #pragma target 3.0

        sampler2D _MainTex, _CameraDepthTexture;
        fixed4 _Color, _TextureColor, _FoamColor;
        float _WaveSpeed, _WaveStrength, _WaveAmount, _WaveFrequency, _TextureDistortion;
        float _FoamAmount, _FoamCutoff, _FoamSpeed, _FoamNoiseScale;
        float _FadeStart, _FadeEnd, _HorizonDarkness;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
            float4 screenPos;
        };

        //Pseudo-random and noise functions omitted for brevity
        float2 random2(float2 st) { st = float2(dot(st, float2(127.1, 311.7)), dot(st, float2(269.5, 183.3))); return -1.0 + 2.0 * frac(sin(st) * 43758.5453123); }
        float gradientNoise(float2 st) { float2 i = floor(st); float2 f = frac(st); float2 u = f*f*(3.0-2.0*f); return lerp(lerp(dot(random2(i + float2(0.0,0.0)), f - float2(0.0,0.0)), dot(random2(i + float2(1.0,0.0)), f - float2(1.0,0.0)), u.x), lerp(dot(random2(i + float2(0.0,1.0)), f - float2(0.0,1.0)), dot(random2(i + float2(1.0,1.0)), f - float2(1.0,1.0)), u.x), u.y); }

        void surf (Input IN, inout SurfaceOutput o) {
            float2 uv = IN.uv_MainTex;
            float2 waveOffset = float2(gradientNoise(uv * _WaveFrequency + _Time.y * _WaveSpeed), gradientNoise(uv * _WaveFrequency * 1.2 + _Time.y * _WaveSpeed * 1.1)) * _WaveAmount;
            float2 distortedUV = uv + waveOffset * _WaveStrength * _TextureDistortion;
            fixed4 c = tex2D(_MainTex, distortedUV);
            c = lerp(tex2D(_MainTex, uv), c, _TextureDistortion) * _TextureColor;
            
            float2 foamUV = IN.worldPos.xz * _FoamNoiseScale + _Time.y * _FoamSpeed;
            float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.screenPos)));
            float foamLine = 1 - saturate(_FoamAmount * (depth - IN.screenPos.w));
            float foam = smoothstep(_FoamCutoff, 1, saturate(gradientNoise(foamUV) + foamLine));

            float dist = distance(_WorldSpaceCameraPos, IN.worldPos);
            float horizonFade = saturate((_FadeEnd - dist) / (_FadeEnd - _FadeStart));
            
            //DARKNESS: Lerp the color to black as it hits the horizon
            float darkness = lerp(1.0, 1.0 - _HorizonDarkness, 1.0 - horizonFade);
            
            o.Albedo = lerp(lerp(_Color.rgb, c.rgb, c.a), _FoamColor.rgb, foam) * darkness;
            o.Alpha = _Color.a * horizonFade;
        }
        ENDCG

        //PASS 2: AURA
        Pass
        {
            ZWrite Off Blend One One 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Assets/Aura 2/Core/Code/Shaders/Aura.cginc"

            struct v2f { float4 pos : SV_POSITION; float3 worldPos : TEXCOORD0; };
            float _AuraIntensity, _FadeStart, _FadeEnd;

            v2f vert (appdata_base v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz; return o; }

            fixed4 frag (v2f i) : SV_Target {
                float3 auraCoords = Aura2_GetFrustumSpaceCoordinates(float4(i.worldPos, 1.0));
                float3 auraResult = float3(0,0,0);
                Aura2_ApplyLighting(auraResult, auraCoords, _AuraIntensity);
                Aura2_ApplyFog(auraResult, auraCoords);
                
                float dist = distance(_WorldSpaceCameraPos, i.worldPos);
                float horizonFade = saturate((_FadeEnd - dist) / (_FadeEnd - _FadeStart));
                
                //Fade Aura lighting completely at the edge to prevent the "glow"
                return fixed4(auraResult * horizonFade, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
