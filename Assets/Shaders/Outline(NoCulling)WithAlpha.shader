Shader "Custom/Outline (No Culling) With Alpha"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5

        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 4)) = 0.25
        
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 0
    }
    SubShader
    {
        // Changed RenderType to TransparentCutout so Unity handles the sorting better
        Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest" }
        LOD 200
        
        // This allows the "Cull" dropdown in the inspector to control the whole shader again
        Cull [_Cull]

        Pass{
            Name "OUTLINE"
            // Restored ZWrite Off so the outline doesn't block the main object
            ZWrite Off
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc" 

            struct appdata {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0; 
            };

            struct v2f{
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            fixed4 _OutlineColor;
            half _OutlineWidth;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _Cutoff;

            v2f vert(appdata input){
                v2f output;
                
                // Standard Vertex Extrusion
                // We use xyz += normal to avoid distorting the W component
                input.vertex.xyz += input.normal * _OutlineWidth;

                output.pos = UnityObjectToClipPos(input.vertex);
                output.normal = mul(unity_ObjectToWorld, input.normal);
                
                // Pass the texture coordinates
                output.uv = TRANSFORM_TEX(input.texcoord, _MainTex);

                return output;
            }

            fixed4 frag(v2f input) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, input.uv);
                
                // If the texture pixel is transparent, don't draw the outline there
                clip(col.a - _Cutoff);

                return _OutlineColor;
            }

            ENDCG
        }

        ZWrite On
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alphatest:_Cutoff

        #ifndef SHADER_API_D3D11
            #pragma target 3.0
        #else
            #pragma target 4.0
        #endif

        struct Input
        {
            float2 uv_MainTex;
        };

        fixed4 _Color;
        sampler2D _MainTex;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 pixel = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = pixel.rgb;
            o.Alpha = pixel.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}