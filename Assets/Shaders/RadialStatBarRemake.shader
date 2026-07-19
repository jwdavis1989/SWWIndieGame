Shader "Shader Graphs/RadialStatBarRemake"
{
    Properties
    {
        [MainTexture][NoScaleOffset]_MainTex("MainTex", 2D) = "white" {}
        _Radius("_Radius", Float) = 0.4
        _LineWidth("_LineWidth", Float) = 0.05
        _Color("_Color", Color) = (1, 1, 1, 0)
        _Rotation("_Rotation", Float) = 0
        _RemovedSegments("_RemovedSegments", Float) = 0
        _SegmentSpacing("_SegmentSpacing", Float) = 0.03
        _SegmentCount("_SegmentCount", Float) = 10
        _Stencil("Stencil", Float) = 0
        _StencilComp("Stencil Comp", Float) = 8
        _StencilOp("Stencil Op", Float) = 0
        _StencilReadMask("Stencil Read Mask", Float) = 255
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _ColorMask("Color Mask", Float) = 15
        [HideInInspector]_BUILTIN_QueueOffset("Float", Float) = 0
        [HideInInspector]_BUILTIN_QueueControl("Float", Float) = -1
    }
    SubShader
    {
        Tags
        {
            // RenderPipeline: <None>
            "RenderType"="Transparent"
            "BuiltInMaterialType" = "Unlit"
            "Queue"="Transparent"
            // DisableBatching: <None>
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="BuiltInUnlitSubTarget"
        }
        Pass
        {
            Name "Pass"
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        ColorMask RGB
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 3.0
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma multi_compile_fwdbase
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define BUILTIN_TARGET_API 1
        #define _BUILTIN_SURFACE_TYPE_TRANSPARENT 1
        #define _BUILTIN_AlphaClip 1
        #define _BUILTIN_ALPHATEST_ON 1
        #ifdef _BUILTIN_SURFACE_TYPE_TRANSPARENT
        #define _SURFACE_TYPE_TRANSPARENT _BUILTIN_SURFACE_TYPE_TRANSPARENT
        #endif
        #ifdef _BUILTIN_ALPHATEST_ON
        #define _ALPHATEST_ON _BUILTIN_ALPHATEST_ON
        #endif
        #ifdef _BUILTIN_AlphaClip
        #define _AlphaClip _BUILTIN_AlphaClip
        #endif
        #ifdef _BUILTIN_ALPHAPREMULTIPLY_ON
        #define _ALPHAPREMULTIPLY_ON _BUILTIN_ALPHAPREMULTIPLY_ON
        #endif
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Shim/Shims.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/LegacySurfaceVertex.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/ShaderLibrary/ShaderGraphFunctions.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _Radius;
        float _LineWidth;
        float4 _Color;
        float _Rotation;
        float _RemovedSegments;
        float _SegmentSpacing;
        float _SegmentCount;
        float _Stencil;
        float _StencilComp;
        float _StencilOp;
        float _StencilReadMask;
        float _StencilWriteMask;
        float _ColorMask;
        CBUFFER_END
        
        
        // Object and Global properties
        TEXTURE2D(_MainTex);
        SAMPLER(sampler_MainTex);
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // Graph Functions
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Absolute_float(float In, out float Out)
        {
            Out = abs(In);
        }
        
        void Unity_DDXY_3ad82e17c7c74d68a426f0c6901d680e_float(float In, out float Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDXY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = abs(ddx(In)) + abs(ddy(In));
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Clamp_float(float In, float Min, float Max, out float Out)
        {
            Out = clamp(In, Min, Max);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Rotate_Radians_float(float2 UV, float2 Center, float Rotation, out float2 Out)
        {
            //rotation matrix
            UV -= Center;
            float s = sin(Rotation);
            float c = cos(Rotation);
        
            //center rotation matrix
            float2x2 rMatrix = float2x2(c, -s, s, c);
            rMatrix *= 0.5;
            rMatrix += 0.5;
            rMatrix = rMatrix*2 - 1;
        
            //multiply the UVs by the rotation matrix
            UV.xy = mul(UV.xy, rMatrix);
            UV += Center;
        
            Out = UV;
        }
        
        void Unity_Arctangent2_float(float A, float B, out float Out)
        {
            Out = atan2(A, B);
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_DDXY_6e2823d3f1604e2fa698e45545369877_float(float In, out float Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDXY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = abs(ddx(In)) + abs(ddy(In));
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Round_float(float In, out float Out)
        {
            Out = round(In);
        }
        
        void Unity_Modulo_float(float A, float B, out float Out)
        {
            Out = fmod(A, B);
        }
        
        void Unity_Sine_float(float In, out float Out)
        {
            Out = sin(In);
        }
        
        void Unity_DDXY_103b895756854f9cb7ba66442bd0771d_float(float In, out float Out)
        {
            
                    #if defined(SHADER_STAGE_RAY_TRACING) && defined(RAYTRACING_SHADER_GRAPH_DEFAULT)
                    #error 'DDXY' node is not supported in ray tracing, please provide an alternate implementation, relying for instance on the 'Raytracing Quality' keyword
                    #endif
            Out = abs(ddx(In)) + abs(ddy(In));
        }
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float AlphaClipThreshold;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float4 _Property_069bf124750a43dbae98a051ac2e408f_Out_0_Vector4 = _Color;
            float2 _TilingAndOffset_39fc4a03eacc4ecdb56f1dd10b3be02e_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), float2 (-0.5, -0.5), _TilingAndOffset_39fc4a03eacc4ecdb56f1dd10b3be02e_Out_3_Vector2);
            float _Length_84a8982ae93941b28f943697994d825d_Out_1_Float;
            Unity_Length_float2(_TilingAndOffset_39fc4a03eacc4ecdb56f1dd10b3be02e_Out_3_Vector2, _Length_84a8982ae93941b28f943697994d825d_Out_1_Float);
            float _Property_4f408190ae6142039e59f15b3b81f498_Out_0_Float = _Radius;
            float _Subtract_e4aeb354889e457f9c97f3e15be0cf52_Out_2_Float;
            Unity_Subtract_float(_Length_84a8982ae93941b28f943697994d825d_Out_1_Float, _Property_4f408190ae6142039e59f15b3b81f498_Out_0_Float, _Subtract_e4aeb354889e457f9c97f3e15be0cf52_Out_2_Float);
            float _Absolute_5a976cb0b37640648ac6dbffd019a5ba_Out_1_Float;
            Unity_Absolute_float(_Subtract_e4aeb354889e457f9c97f3e15be0cf52_Out_2_Float, _Absolute_5a976cb0b37640648ac6dbffd019a5ba_Out_1_Float);
            float _Property_18f4a10b129f42148245171013c3e5c2_Out_0_Float = _LineWidth;
            float _Subtract_fbddd74af54640eea739924cb82dc722_Out_2_Float;
            Unity_Subtract_float(_Absolute_5a976cb0b37640648ac6dbffd019a5ba_Out_1_Float, _Property_18f4a10b129f42148245171013c3e5c2_Out_0_Float, _Subtract_fbddd74af54640eea739924cb82dc722_Out_2_Float);
            float _DDXY_3ad82e17c7c74d68a426f0c6901d680e_Out_1_Float;
            Unity_DDXY_3ad82e17c7c74d68a426f0c6901d680e_float(_Subtract_e4aeb354889e457f9c97f3e15be0cf52_Out_2_Float, _DDXY_3ad82e17c7c74d68a426f0c6901d680e_Out_1_Float);
            float _Divide_7f57fe2acadd406e9128ccea2bcd4b83_Out_2_Float;
            Unity_Divide_float(_Subtract_fbddd74af54640eea739924cb82dc722_Out_2_Float, _DDXY_3ad82e17c7c74d68a426f0c6901d680e_Out_1_Float, _Divide_7f57fe2acadd406e9128ccea2bcd4b83_Out_2_Float);
            float _OneMinus_fc1e991f0de7477384ca2dc3327d671a_Out_1_Float;
            Unity_OneMinus_float(_Divide_7f57fe2acadd406e9128ccea2bcd4b83_Out_2_Float, _OneMinus_fc1e991f0de7477384ca2dc3327d671a_Out_1_Float);
            float _Clamp_3ba05b7c57104781bb383f8950f37c54_Out_3_Float;
            Unity_Clamp_float(_OneMinus_fc1e991f0de7477384ca2dc3327d671a_Out_1_Float, float(0), float(1), _Clamp_3ba05b7c57104781bb383f8950f37c54_Out_3_Float);
            float _Property_e82839ff83ae4e99884767bffdf36a7e_Out_0_Float = _RemovedSegments;
            float _Property_0af3a2d469ef4153a3a687072bd3827e_Out_0_Float = _SegmentCount;
            float _Divide_758d05c0312a436f82b13a79e117ad86_Out_2_Float;
            Unity_Divide_float(float(6.282), _Property_0af3a2d469ef4153a3a687072bd3827e_Out_0_Float, _Divide_758d05c0312a436f82b13a79e117ad86_Out_2_Float);
            float _Multiply_32946e0b7336450fa804502300ea9dad_Out_2_Float;
            Unity_Multiply_float_float(_Property_e82839ff83ae4e99884767bffdf36a7e_Out_0_Float, _Divide_758d05c0312a436f82b13a79e117ad86_Out_2_Float, _Multiply_32946e0b7336450fa804502300ea9dad_Out_2_Float);
            float _Property_8454e14b0b2e4220ad6873a5fe279b95_Out_0_Float = _Rotation;
            float2 _Rotate_9d78d4bbe2ea4f589688464af35630e0_Out_3_Vector2;
            Unity_Rotate_Radians_float(_TilingAndOffset_39fc4a03eacc4ecdb56f1dd10b3be02e_Out_3_Vector2, float2 (0.5, 0.5), _Property_8454e14b0b2e4220ad6873a5fe279b95_Out_0_Float, _Rotate_9d78d4bbe2ea4f589688464af35630e0_Out_3_Vector2);
            float _Split_3b2e5bca06494272852f92796c88ea66_R_1_Float = _Rotate_9d78d4bbe2ea4f589688464af35630e0_Out_3_Vector2[0];
            float _Split_3b2e5bca06494272852f92796c88ea66_G_2_Float = _Rotate_9d78d4bbe2ea4f589688464af35630e0_Out_3_Vector2[1];
            float _Split_3b2e5bca06494272852f92796c88ea66_B_3_Float = 0;
            float _Split_3b2e5bca06494272852f92796c88ea66_A_4_Float = 0;
            float _Arctangent2_de9e29c73a714a1cabd2b4dc7a29f0a1_Out_2_Float;
            Unity_Arctangent2_float(_Split_3b2e5bca06494272852f92796c88ea66_R_1_Float, _Split_3b2e5bca06494272852f92796c88ea66_G_2_Float, _Arctangent2_de9e29c73a714a1cabd2b4dc7a29f0a1_Out_2_Float);
            float _Add_b09d6945d78540d9b1898fbf66fc47f5_Out_2_Float;
            Unity_Add_float(_Arctangent2_de9e29c73a714a1cabd2b4dc7a29f0a1_Out_2_Float, float(3.141), _Add_b09d6945d78540d9b1898fbf66fc47f5_Out_2_Float);
            float _Subtract_4e35fb73669343849f2655f66144be06_Out_2_Float;
            Unity_Subtract_float(_Multiply_32946e0b7336450fa804502300ea9dad_Out_2_Float, _Add_b09d6945d78540d9b1898fbf66fc47f5_Out_2_Float, _Subtract_4e35fb73669343849f2655f66144be06_Out_2_Float);
            float _DDXY_6e2823d3f1604e2fa698e45545369877_Out_1_Float;
            Unity_DDXY_6e2823d3f1604e2fa698e45545369877_float(_Subtract_4e35fb73669343849f2655f66144be06_Out_2_Float, _DDXY_6e2823d3f1604e2fa698e45545369877_Out_1_Float);
            float _Divide_c6f316d620c849779c12c312ec2116e5_Out_2_Float;
            Unity_Divide_float(_Subtract_4e35fb73669343849f2655f66144be06_Out_2_Float, _DDXY_6e2823d3f1604e2fa698e45545369877_Out_1_Float, _Divide_c6f316d620c849779c12c312ec2116e5_Out_2_Float);
            float _Clamp_818f79cdf2994b15adfa65df3742bc60_Out_3_Float;
            Unity_Clamp_float(_Divide_c6f316d620c849779c12c312ec2116e5_Out_2_Float, float(0), float(1), _Clamp_818f79cdf2994b15adfa65df3742bc60_Out_3_Float);
            float _Subtract_6657c041c53b462d85e00fed9aaa770c_Out_2_Float;
            Unity_Subtract_float(_Clamp_3ba05b7c57104781bb383f8950f37c54_Out_3_Float, _Clamp_818f79cdf2994b15adfa65df3742bc60_Out_3_Float, _Subtract_6657c041c53b462d85e00fed9aaa770c_Out_2_Float);
            float _Property_6a99ecaef705439c86274b8b0240446a_Out_0_Float = _SegmentCount;
            float _Remap_840729507bdc456c829deffd3054ed3c_Out_3_Float;
            Unity_Remap_float(_Property_6a99ecaef705439c86274b8b0240446a_Out_0_Float, float2 (1, 2), float2 (0, 0.51), _Remap_840729507bdc456c829deffd3054ed3c_Out_3_Float);
            float _Clamp_b35ddd0d1d7d4c7eb696285e588418f9_Out_3_Float;
            Unity_Clamp_float(_Remap_840729507bdc456c829deffd3054ed3c_Out_3_Float, float(0), float(1), _Clamp_b35ddd0d1d7d4c7eb696285e588418f9_Out_3_Float);
            float _Round_422fb56cd46b40f3bdcf6666c2a81dbe_Out_1_Float;
            Unity_Round_float(_Clamp_b35ddd0d1d7d4c7eb696285e588418f9_Out_3_Float, _Round_422fb56cd46b40f3bdcf6666c2a81dbe_Out_1_Float);
            float _Divide_686f552a1c59471481ad1063f71c2a6f_Out_2_Float;
            Unity_Divide_float(_Divide_758d05c0312a436f82b13a79e117ad86_Out_2_Float, float(2), _Divide_686f552a1c59471481ad1063f71c2a6f_Out_2_Float);
            float _Add_c1c4cddfeccc43b19767c4ad2d1bd54d_Out_2_Float;
            Unity_Add_float(_Add_b09d6945d78540d9b1898fbf66fc47f5_Out_2_Float, _Divide_686f552a1c59471481ad1063f71c2a6f_Out_2_Float, _Add_c1c4cddfeccc43b19767c4ad2d1bd54d_Out_2_Float);
            float _Modulo_93927359693f425dbadefc6251782298_Out_2_Float;
            Unity_Modulo_float(_Add_c1c4cddfeccc43b19767c4ad2d1bd54d_Out_2_Float, _Divide_758d05c0312a436f82b13a79e117ad86_Out_2_Float, _Modulo_93927359693f425dbadefc6251782298_Out_2_Float);
            float _Subtract_5039532c93d3407aba3e500404c18106_Out_2_Float;
            Unity_Subtract_float(_Modulo_93927359693f425dbadefc6251782298_Out_2_Float, _Divide_686f552a1c59471481ad1063f71c2a6f_Out_2_Float, _Subtract_5039532c93d3407aba3e500404c18106_Out_2_Float);
            float _Sine_2fdaea5170464f26bd25f48c52875cfb_Out_1_Float;
            Unity_Sine_float(_Subtract_5039532c93d3407aba3e500404c18106_Out_2_Float, _Sine_2fdaea5170464f26bd25f48c52875cfb_Out_1_Float);
            float _Absolute_e4f75081bab748c48ff43e5d934e7772_Out_1_Float;
            Unity_Absolute_float(_Sine_2fdaea5170464f26bd25f48c52875cfb_Out_1_Float, _Absolute_e4f75081bab748c48ff43e5d934e7772_Out_1_Float);
            float2 _TilingAndOffset_8ef7ec5d3b3644d8832fe166f9c1dbaa_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), float2 (-0.5, -0.5), _TilingAndOffset_8ef7ec5d3b3644d8832fe166f9c1dbaa_Out_3_Vector2);
            float _Length_fab6e3bf17d54613aaba1bdb613782ed_Out_1_Float;
            Unity_Length_float2(_TilingAndOffset_8ef7ec5d3b3644d8832fe166f9c1dbaa_Out_3_Vector2, _Length_fab6e3bf17d54613aaba1bdb613782ed_Out_1_Float);
            float _Multiply_da8c8335aad64f0bb48d3f6d6a95782a_Out_2_Float;
            Unity_Multiply_float_float(_Absolute_e4f75081bab748c48ff43e5d934e7772_Out_1_Float, _Length_fab6e3bf17d54613aaba1bdb613782ed_Out_1_Float, _Multiply_da8c8335aad64f0bb48d3f6d6a95782a_Out_2_Float);
            float _Property_4d6eb3ffe5e4405390f54f7e14c8ab1a_Out_0_Float = _SegmentSpacing;
            float _Subtract_b46577c33b744bd399416342c671d81d_Out_2_Float;
            Unity_Subtract_float(_Multiply_da8c8335aad64f0bb48d3f6d6a95782a_Out_2_Float, _Property_4d6eb3ffe5e4405390f54f7e14c8ab1a_Out_0_Float, _Subtract_b46577c33b744bd399416342c671d81d_Out_2_Float);
            float _DDXY_103b895756854f9cb7ba66442bd0771d_Out_1_Float;
            Unity_DDXY_103b895756854f9cb7ba66442bd0771d_float(_Subtract_b46577c33b744bd399416342c671d81d_Out_2_Float, _DDXY_103b895756854f9cb7ba66442bd0771d_Out_1_Float);
            float _Divide_13029ed5db9948be9b9afc66abda4435_Out_2_Float;
            Unity_Divide_float(_Subtract_b46577c33b744bd399416342c671d81d_Out_2_Float, _DDXY_103b895756854f9cb7ba66442bd0771d_Out_1_Float, _Divide_13029ed5db9948be9b9afc66abda4435_Out_2_Float);
            float _OneMinus_2b0d43f45c1944ecba1fde6ccfe66cf9_Out_1_Float;
            Unity_OneMinus_float(_Divide_13029ed5db9948be9b9afc66abda4435_Out_2_Float, _OneMinus_2b0d43f45c1944ecba1fde6ccfe66cf9_Out_1_Float);
            float _Clamp_12dd4f43791245a592e9a49917cf3802_Out_3_Float;
            Unity_Clamp_float(_OneMinus_2b0d43f45c1944ecba1fde6ccfe66cf9_Out_1_Float, float(0), float(1), _Clamp_12dd4f43791245a592e9a49917cf3802_Out_3_Float);
            float _Multiply_474fdb7b3f624da5aeffd9601cd2ba28_Out_2_Float;
            Unity_Multiply_float_float(_Round_422fb56cd46b40f3bdcf6666c2a81dbe_Out_1_Float, _Clamp_12dd4f43791245a592e9a49917cf3802_Out_3_Float, _Multiply_474fdb7b3f624da5aeffd9601cd2ba28_Out_2_Float);
            float _Subtract_5564ceb1553f4c87a5089e2fc1b813be_Out_2_Float;
            Unity_Subtract_float(_Subtract_6657c041c53b462d85e00fed9aaa770c_Out_2_Float, _Multiply_474fdb7b3f624da5aeffd9601cd2ba28_Out_2_Float, _Subtract_5564ceb1553f4c87a5089e2fc1b813be_Out_2_Float);
            float _Clamp_7a258b2dbc704119a000b418400f66cd_Out_3_Float;
            Unity_Clamp_float(_Subtract_5564ceb1553f4c87a5089e2fc1b813be_Out_2_Float, float(0), float(1), _Clamp_7a258b2dbc704119a000b418400f66cd_Out_3_Float);
            float4 _Multiply_9787efb793b745f88439d939acdde2ba_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Property_069bf124750a43dbae98a051ac2e408f_Out_0_Vector4, (_Clamp_7a258b2dbc704119a000b418400f66cd_Out_3_Float.xxxx), _Multiply_9787efb793b745f88439d939acdde2ba_Out_2_Vector4);
            surface.BaseColor = (_Multiply_9787efb793b745f88439d939acdde2ba_Out_2_Vector4.xyz);
            surface.Alpha = _Clamp_7a258b2dbc704119a000b418400f66cd_Out_3_Float;
            surface.AlphaClipThreshold = float(0.5);
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        void BuildAppDataFull(Attributes attributes, VertexDescription vertexDescription, inout appdata_full result)
        {
            result.vertex     = float4(attributes.positionOS, 1);
            result.tangent    = attributes.tangentOS;
            result.normal     = attributes.normalOS;
            result.texcoord   = attributes.uv0;
            result.vertex     = float4(vertexDescription.Position, 1);
            result.normal     = vertexDescription.Normal;
            result.tangent    = float4(vertexDescription.Tangent, 0);
            #if UNITY_ANY_INSTANCING_ENABLED
            #endif
        }
        
        void VaryingsToSurfaceVertex(Varyings varyings, inout v2f_surf result)
        {
            result.pos = varyings.positionCS;
            // World Tangent isn't an available input on v2f_surf
        
        
            #if UNITY_ANY_INSTANCING_ENABLED
            #endif
            #if UNITY_SHOULD_SAMPLE_SH
            #if !defined(LIGHTMAP_ON)
            #endif
            #endif
            #if defined(LIGHTMAP_ON)
            #endif
            #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                result.fogCoord = varyings.fogFactorAndVertexLight.x;
                COPY_TO_LIGHT_COORDS(result, varyings.fogFactorAndVertexLight.yzw);
            #endif
        
            DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(varyings, result);
        }
        
        void SurfaceVertexToVaryings(v2f_surf surfVertex, inout Varyings result)
        {
            result.positionCS = surfVertex.pos;
            // viewDirectionWS is never filled out in the legacy pass' function. Always use the value computed by SRP
            // World Tangent isn't an available input on v2f_surf
        
            #if UNITY_ANY_INSTANCING_ENABLED
            #endif
            #if UNITY_SHOULD_SAMPLE_SH
            #if !defined(LIGHTMAP_ON)
            #endif
            #endif
            #if defined(LIGHTMAP_ON)
            #endif
            #ifdef VARYINGS_NEED_FOG_AND_VERTEX_LIGHT
                result.fogFactorAndVertexLight.x = surfVertex.fogCoord;
                COPY_FROM_LIGHT_COORDS(result.fogFactorAndVertexLight.yzw, surfVertex);
            #endif
        
            DEFAULT_UNITY_TRANSFER_VERTEX_OUTPUT_STEREO(surfVertex, result);
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.shadergraph/Editor/Generation/Targets/BuiltIn/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "UnityEditor.Rendering.BuiltIn.ShaderGraph.BuiltInUnlitGUI" ""
    FallBack "Hidden/Shader Graph/FallbackError"
}