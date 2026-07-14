Shader "Custom/BasicWaterShader_Aura2_Final_Ultimate"
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
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 200

        // --- PASS 1: THE WATER ---
        CGPROGRAM
        #pragma surface surf Lambert alpha:fade
        #pragma target 3.0

        sampler2D _MainTex, _CameraDepthTexture;
        fixed4 _Color, _TextureColor, _FoamColor;
        float _WaveSpeed, _WaveStrength, _WaveAmount, _WaveFrequency, _TextureDistortion;
        float _FoamAmount, _FoamCutoff, _FoamSpeed, _FoamNoiseScale;

        struct Input {
            float2 uv_MainTex;
            float3 worldPos;
            float4 screenPos;
        };

        float2 random2(float2 st) {
            st = float2(dot(st, float2(127.1, 311.7)), dot(st, float2(269.5, 183.3)));
            return -1.0 + 2.0 * frac(sin(st) * 43758.5453123);
        }

        float gradientNoise(float2 st) {
            float2 i = floor(st); float2 f = frac(st);
            float2 u = f*f*(3.0-2.0*f);
            return lerp(lerp(dot(random2(i + float2(0.0,0.0)), f - float2(0.0,0.0)),
                             dot(random2(i + float2(1.0,0.0)), f - float2(1.0,0.0)), u.x),
                        lerp(dot(random2(i + float2(0.0,1.0)), f - float2(0.0,1.0)),
                             dot(random2(i + float2(1.0,1.0)), f - float2(1.0,1.0)), u.x), u.y);
        }

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

            o.Albedo = lerp(lerp(_Color.rgb, c.rgb, c.a), _FoamColor.rgb, foam);
            o.Alpha = _Color.a;
        }
        ENDCG

        // --- PASS 2: AURA VOLUMETRICS (ADDITIVE) ---
        Pass
        {
            Name "AuraPass"
            ZWrite Off
            Blend One One // Additive blending for light and fog injection
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #include "Assets/Aura 2/Core/Code/Shaders/Aura.cginc"

            struct v2f {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float _AuraIntensity;

            v2f vert (appdata_base v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float3 auraCoords = Aura2_GetFrustumSpaceCoordinates(float4(i.worldPos, 1.0));
                
                // Start with black to only add the Aura contribution
                float3 auraResult = float3(0,0,0);
                
                // Apply lighting and fog to the empty black base
                Aura2_ApplyLighting(auraResult, auraCoords, _AuraIntensity);
                Aura2_ApplyFog(auraResult, auraCoords);
                
                // Additive blend means we only return what Aura adds to the pixel
                return fixed4(auraResult, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
