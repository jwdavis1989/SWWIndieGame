// Shader "UI/HexagonStatBarSmooth"
// {
//     Properties
//     {
//         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
//         _Color ("Tint (UI Default)", Color) = (1,1,1,1)
//         _StatColor ("Stat Color Accent", Color) = (1,0,0,1)
//         _GridLineColor ("Grid Line Color", Color) = (0.1, 0.1, 0.1, 0.8)
//         _Health ("Health Percent (0-1)", Range(0,1)) = 1.0
//         _LineThickness ("Border Thickness", Range(0.005, 0.3)) = 0.05
//         _GlobalHexSize ("Global Hex Size", Range(0.5, 1.5)) = 1.15
//         _VarianceRangeMin ("Min Brightness Offset", Range(0.0, 1.0)) = 0.5
//     }

//     SubShader
//     {
//         Tags 
//         { 
//             "Queue"="Transparent" 
//             "IgnoreProjector"="True" 
//             "RenderType"="Transparent" 
//             "PreviewType"="Plane" 
//             "CanUseSpriteAtlas"="True" 
//         }
        
//         Cull Off 
//         Lighting Off 
//         ZWrite Off 
//         Blend SrcAlpha OneMinusSrcAlpha

//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #include "UnityCG.cginc"

//             struct appdata_t
//             {
//                 float4 vertex   : POSITION;
//                 float4 color    : COLOR;
//                 float2 texcoord : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float4 vertex   : SV_POSITION;
//                 fixed4 color    : COLOR;
//                 float2 texcoord  : TEXCOORD0;
//             };

//             fixed4 _Color;
//             fixed4 _StatColor;
//             fixed4 _GridLineColor;
//             float _Health;
//             float _LineThickness;
//             float _GlobalHexSize;
//             float _VarianceRangeMin;

//             v2f vert(appdata_t IN)
//             {
//                 v2f OUT;
//                 OUT.vertex = UnityObjectToClipPos(IN.vertex);
//                 OUT.texcoord = IN.texcoord;
//                 OUT.color = IN.color * _Color;
//                 return OUT;
//             }

//             // High-precision distance function for pointy-topped hexagons
//             float PointyHexDistance(float2 p)
//             {
//                 float2 q = abs(p);
//                 return max(q.y * 0.866025 + q.x * 0.5, q.x);
//             }

//             float HashGrid(float2 p)
//             {
//                 return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453123);
//             }

//             fixed4 frag(v2f IN) : SV_Target
//             {
//                 // FIX: Compress the UV coordinates to create a protective bounding padding margin window
//                 // This scales the entire interlocking map layout down slightly within the UI bounds.
//                 float2 paddedUV = IN.texcoord;
//                 paddedUV.x = paddedUV.x * 0.96 + 0.02; // 2% padding margin on the left and right sides
//                 paddedUV.y = paddedUV.y * 0.85 + 0.075; // Padding margin on the top and bottom sides

//                 // Precise aspect ratios for gapless structural nesting 
//                 float2 hexSpacing = float2(1.5, 1.73205); 
//                 float2 uv = paddedUV * float2(18.0 * 1.5, 2.0 * 1.73205);

//                 // Generate overlapping offset matrices to cleanly map the interlocking diagonal valleys
//                 float2 gridA = frac(uv / hexSpacing) * hexSpacing - hexSpacing * 0.5;
//                 float2 gridB = frac((uv - hexSpacing * 0.5) / hexSpacing) * hexSpacing - hexSpacing * 0.5;

//                 // Bind every individual pixel to its true closest mathematical hexagon center
//                 float2 cellUV, cellID;
//                 if (length(gridA) < length(gridB))
//                 {
//                     cellUV = gridA;
//                     cellID = floor(uv / hexSpacing);
//                 }
//                 else
//                 {
//                     cellUV = gridB;
//                     cellID = floor((uv - hexSpacing * 0.5) / hexSpacing) + 0.5;
//                 }

//                 // Resolve absolute tracking indexes (0-17 columns, 0-1 rows)
//                 float colIndex = clamp(cellID.x, 0.0, 17.0);
//                 float rowIndex = clamp(floor(cellID.y + 0.25), 0.0, 1.0);

//                 // Right-to-Left drainage calculation index (0 to 35)
//                 float totalHexes = 36.0;
//                 float hexID = (colIndex * 2.0) + rowIndex;
//                 float hexStartThreshold = hexID / totalHexes;

//                 // Apply Global Hex Size multiplier to pull the shapes away from each other
//                 float dist = PointyHexDistance(cellUV) * _GlobalHexSize;
                
//                 float outerRadius = 0.866025; // Clean physical perimeter boundary
//                 float innerRadius = outerRadius - _LineThickness;

//                 float fillMask = smoothstep(innerRadius, innerRadius - 0.005, dist);
//                 float lineMask = smoothstep(innerRadius, innerRadius + 0.005, dist) * smoothstep(outerRadius, outerRadius - 0.005, dist);

//                 // Apply individual cell brightness variance
//                 float cellSeed = HashGrid(float2(colIndex, rowIndex));
//                 float brightnessFactor = lerp(_VarianceRangeMin, 1.0, cellSeed);

//                 // Composite final colors
//                 fixed4 fillCol = _StatColor * IN.color;
//                 fillCol.rgb *= brightnessFactor;
//                 fillCol.a *= fillMask;

//                 fixed4 lineCol = _GridLineColor;
//                 lineCol.a *= lineMask;

//                 fixed4 finalColor = lerp(fillCol, lineCol, lineMask);

//                 // Health Bar fill drainage math
//                 float hexVisibility = saturate((_Health - hexStartThreshold) / 0.1);
                
//                 // Keeps your board-game border lines permanently drawn on screen, fading only the filled cores
//                 finalColor.a *= lerp(lineMask, 1.0, hexVisibility);
                
//                 return finalColor;
//             }
//             ENDCG
//         }
//     }
// }

// Shader "UI/HexagonStatBarSmooth"
// {
//     Properties
//     {
//         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
//         _Color ("Tint (UI Default)", Color) = (1,1,1,1)
//         _StatColor ("Stat Color Accent", Color) = (1,0,0,1)
//         _GridLineColor ("Grid Line Color", Color) = (0.1, 0.1, 0.1, 0.8)
//         _Health ("Health Percent (0-1)", Range(0,1)) = 1.0
//         _LineThickness ("Border Thickness", Range(0.005, 0.3)) = 0.05
//         _GlobalHexSize ("Global Hex Size", Range(0.5, 1.5)) = 1.15
//         _VarianceRangeMin ("Min Brightness Offset", Range(0.0, 1.0)) = 0.5
//     }

//     SubShader
//     {
//         Tags 
//         { 
//             "Queue"="Transparent" 
//             "IgnoreProjector"="True" 
//             "RenderType"="Transparent" 
//             "PreviewType"="Plane" 
//             "CanUseSpriteAtlas"="True" 
//         }
        
//         Cull Off 
//         Lighting Off 
//         ZWrite Off 
//         Blend SrcAlpha OneMinusSrcAlpha

//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #include "UnityCG.cginc"

//             struct appdata_t
//             {
//                 float4 vertex   : POSITION;
//                 float4 color    : COLOR;
//                 float2 texcoord : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float4 vertex   : SV_POSITION;
//                 fixed4 color    : COLOR;
//                 float2 texcoord  : TEXCOORD0;
//             };

//             fixed4 _Color;
//             fixed4 _StatColor;
//             fixed4 _GridLineColor;
//             float _Health;
//             float _LineThickness;
//             float _GlobalHexSize;
//             float _VarianceRangeMin;

//             v2f vert(appdata_t IN)
//             {
//                 v2f OUT;
//                 OUT.vertex = UnityObjectToClipPos(IN.vertex);
//                 OUT.texcoord = IN.texcoord;
//                 OUT.color = IN.color * _Color;
//                 return OUT;
//             }

//             // High-precision distance field algorithm for pointy-topped hexagons
//             float PointyHexDistance(float2 p)
//             {
//                 float2 q = abs(p);
//                 return max(q.y * 0.866025 + q.x * 0.5, q.x);
//             }

//             float HashGrid(float2 p)
//             {
//                 return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453123);
//             }

//             fixed4 frag(v2f IN) : SV_Target
//             {
//                 // 1. Map texture coordinates cleanly across an 18x2 interlocking aspect scale
//                 float2 hexSpacing = float2(1.5, 1.73205); 
//                 float2 uv = IN.texcoord * float2(18.0 * 1.5, 2.0 * 1.73205);

//                 // 2. Generate overlapping grid matrices to allow geometric intersection tracking
//                 float2 gridA = frac(uv / hexSpacing) * hexSpacing - hexSpacing * 0.5;
//                 float2 gridB = frac((uv - hexSpacing * 0.5) / hexSpacing) * hexSpacing - hexSpacing * 0.5;

//                 // 3. Find which hexagon cell center this specific pixel is physically closest to
//                 float2 cellUV, cellID;
//                 if (length(gridA) < length(gridB))
//                 {
//                     cellUV = gridA;
//                     cellID = floor(uv / hexSpacing);
//                 }
//                 else
//                 {
//                     cellUV = gridB;
//                     cellID = floor((uv - hexSpacing * 0.5) / hexSpacing) + 0.5;
//                 }

//                 // 4. Clean isolation tracking for column indices (0-17) and row indices (0-1)
//                 float colIndex = clamp(cellID.x, 0.0, 17.0);
//                 float rowIndex = clamp(floor(cellID.y + 0.25), 0.0, 1.0);

//                 // ---------------------------------------------------------
//                 // FIX PART 1: REMOVED FIXED BOUNDARY CLIPPING LINE
//                 // ---------------------------------------------------------
//                 // Instead of using coordinate lines to chop out extra cells, we selectively 
//                 // disable rendering only if a calculation drifts beyond our grid coordinates.
//                 if (cellID.y + 0.25 < 0.0 || cellID.y + 0.25 >= 2.0)
//                 {
//                     discard;
//                 }

//                 // Right-to-Left drainage calculation indexing tracker
//                 float totalHexes = 36.0;
//                 float hexID = (colIndex * 2.0) + rowIndex;
//                 float hexStartThreshold = hexID / totalHexes;

//                 // 5. Draw the uniform hexagon boundaries
//                 float dist = PointyHexDistance(cellUV) * _GlobalHexSize;
                
//                 float outerRadius = 0.866025; 
//                 float innerRadius = outerRadius - _LineThickness;

//                 float fillMask = smoothstep(innerRadius, innerRadius - 0.005, dist);
//                 float lineMask = smoothstep(innerRadius, innerRadius + 0.005, dist) * smoothstep(outerRadius, outerRadius - 0.005, dist);

//                 // Apply cell color variance 
//                 float cellSeed = HashGrid(float2(colIndex, rowIndex));
//                 float brightnessFactor = lerp(_VarianceRangeMin, 1.0, cellSeed);

//                 // 6. Color Composition
//                 fixed4 fillCol = _StatColor * IN.color;
//                 fillCol.rgb *= brightnessFactor;
//                 fillCol.a *= fillMask;

//                 fixed4 lineCol = _GridLineColor;
//                 lineCol.a *= lineMask;

//                 fixed4 finalColor = lerp(fillCol, lineCol, lineMask);

//                 // ---------------------------------------------------------
//                 // FIX PART 2: MATCH TRANSPARENCY CHANNEL TO VECTOR MASK
//                 // ---------------------------------------------------------
//                 // This ensures the invisible bounds outside the shapes register as empty space.
//                 float compositeMask = saturate(fillMask + lineMask);
//                 finalColor.a *= compositeMask;

//                 // Health Bar drainage fade math
//                 float hexVisibility = saturate((_Health - hexStartThreshold) / 0.1);
//                 finalColor.a *= lerp(lineMask, 1.0, hexVisibility);
                
//                 return finalColor;
//             }
//             ENDCG
//         }
//     }
// }

// Shader "UI/HexagonStatBarSmooth"
// {
//     Properties
//     {
//         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
//         _Color ("Tint (UI Default)", Color) = (1,1,1,1)
//         _StatColor ("Stat Color Accent", Color) = (1,0,0,1)
//         _GridLineColor ("Grid Line Color", Color) = (0.05, 0.05, 0.05, 0.8)
//         _Health ("Health Percent (0-1)", Range(0,1)) = 1.0
//         _BorderWidth ("Line Thickness", Range(0.01, 0.15)) = 0.03
//         _VarianceRangeMin ("Min Brightness Offset", Range(0.0, 1.0)) = 0.65
//     }

//     SubShader
//     {
//         Tags 
//         { 
//             "Queue"="Transparent" 
//             "IgnoreProjector"="True" 
//             "RenderType"="Transparent" 
//             "PreviewType"="Plane" 
//             "CanUseSpriteAtlas"="True" 
//         }
        
//         Cull Off 
//         Lighting Off 
//         ZWrite Off 
//         Blend SrcAlpha OneMinusSrcAlpha

//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #include "UnityCG.cginc"

//             struct appdata_t
//             {
//                 float4 vertex   : POSITION;
//                 float4 color    : COLOR;
//                 float2 texcoord : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float4 vertex   : SV_POSITION;
//                 fixed4 color    : COLOR;
//                 float2 texcoord  : TEXCOORD0;
//             };

//             fixed4 _Color;
//             fixed4 _StatColor;
//             fixed4 _GridLineColor;
//             float _Health;
//             float _BorderWidth;
//             float _VarianceRangeMin;

//             v2f vert(appdata_t IN)
//             {
//                 v2f OUT;
//                 OUT.vertex = UnityObjectToClipPos(IN.vertex);
//                 OUT.texcoord = IN.texcoord;
//                 OUT.color = IN.color * _Color;
//                 return OUT;
//             }

//             // Geometric distance function specifically optimized for pointy-topped hexagons
//             float PointyHexDistance(float2 p)
//             {
//                 float2 q = abs(p);
//                 return max(q.y * 0.866025 + q.x * 0.5, q.x);
//             }

//             // Pseudo-random cell hash modifier
//             float HashGrid(float2 p)
//             {
//                 return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
//             }

//             fixed4 frag(v2f IN) : SV_Target
//             {
//                 // 1. Establish the precise aspect scaling bounds for 18 interlocking columns.
//                 // In an interlocking honeycomb grid, the columns pack at a width ratio of 1.5,
//                 // and the rows nest tightly at a height ratio of sqrt(3) ≈ 1.73205.
//                 float2 spacingMatrix = float2(1.5, 1.73205);
                
//                 // Scale the texture coordinate canvas map to cleanly hold exactly 18 columns.
//                 // We add a tiny safety buffer padding window to ensure the outer hex bounds don't get trimmed.
//                 float2 scaledUV = IN.texcoord * float2(17.3, 1.5); 
//                 scaledUV += float2(0.35, 0.45); // Centers the 2 rows inside your RectTransform viewport

//                 // 2. Dual-Grid Honeycomb Tiling Algorithm
//                 // Generates primary grid A and shifted secondary grid B to evaluate overlapping valleys
//                 float2 gridA = frac(scaledUV / spacingMatrix) * spacingMatrix - spacingMatrix * 0.5;
//                 float2 gridB = frac((scaledUV - spacingMatrix * 0.5) / spacingMatrix) * spacingMatrix - spacingMatrix * 0.5;

//                 // 3. Voronoi Point Evaluation: Bind every individual pixel to its true mathematical center
//                 float2 cellUV, cellID;
//                 if (length(gridA) < length(gridB))
//                 {
//                     cellUV = gridA;
//                     cellID = floor(scaledUV / spacingMatrix);
//                 }
//                 else
//                 {
//                     cellUV = gridB;
//                     cellID = floor((scaledUV - spacingMatrix * 0.5) / spacingMatrix) + 0.5;
//                 }

//                 // 4. Clean isolation tracking for column indices (0-17) and row indices (0-1)
//                 float colIndex = clamp(cellID.x, 0.0, 17.0);
//                 float rowIndex = clamp(floor(cellID.y + 0.5), 0.0, 1.0);

//                 // 5. HARD HONEYCOMB SHAPE CEILING MASK
//                 // Instead of using straight coordinate clipping lines that cut hexes in half,
//                 // we evaluate the true center index. If it drifts outside our 2 rows, we drop it.
//                 // This ensures the outer edges of your bar take on a jagged, organic honeycomb shape.
//                 float checkRow = floor(cellID.y + 0.5);
//                 if (checkRow < 0.0 || checkRow > 1.0)
//                 {
//                     discard;
//                 }

//                 // Sequential Right-to-Left drainage tracking index (0 to 35)
//                 float totalHexes = 36.0;
//                 float hexID = (colIndex * 2.0) + rowIndex;
//                 float hexStartThreshold = hexID / totalHexes;

//                 // 6. Draw the crisp inner core fills versus thin outer tabletop lines
//                 float dist = PointyHexDistance(cellUV);
                
//                 float outerRadius = 0.46; // Hard limit boundary radius where interlocking edges lock up
//                 float innerRadius = outerRadius - _BorderWidth;

//                 float fillMask = smoothstep(innerRadius, innerRadius - 0.005, dist);
//                 float lineMask = smoothstep(innerRadius, innerRadius + 0.005, dist) * smoothstep(outerRadius, outerRadius - 0.005, dist);

//                 // Calculate random variation values for the fill cores
//                 float cellSeed = HashGrid(float2(colIndex, rowIndex));
//                 float brightnessFactor = lerp(_VarianceRangeMin, 1.0, cellSeed);

//                 // 7. Composite final colors
//                 fixed4 fillCol = _StatColor * IN.color;
//                 fillCol.rgb *= brightnessFactor;
//                 fillCol.a *= fillMask;

//                 fixed4 lineCol = _GridLineColor;
//                 lineCol.a *= lineMask;

//                 fixed4 finalColor = lerp(fillCol, lineCol, lineMask);

//                 // Handle cutouts for empty space outside of any active vector hexagon boundaries
//                 float compositeVisibilityMask = saturate(fillMask + lineMask);
//                 finalColor.a *= compositeVisibilityMask;

//                 // 8. Health Bar fill drainage math
//                 float hexVisibility = saturate((_Health - hexStartThreshold) / 0.1);
                
//                 // Keeps your board-game border lines permanently drawn on screen, fading only the filled cores
//                 finalColor.a *= lerp(lineMask, 1.0, hexVisibility);
                
//                 return finalColor;
//             }
//             ENDCG
//         }
//     }
// }

// Shader "UI/HexagonStatBarSmooth"
// {
//     Properties
//     {
//         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
//         _Color ("Tint (UI Default)", Color) = (1,1,1,1)
//         _StatColor ("Stat Color Accent", Color) = (1,0,0,1)
//         _GridLineColor ("Grid Line Color", Color) = (0.05, 0.05, 0.05, 0.8)
//         _Health ("Health Percent (0-1)", Range(0,1)) = 1.0
//         _BorderWidth ("Line Thickness", Range(0.01, 0.15)) = 0.03
//         _VarianceRangeMin ("Min Brightness Offset", Range(0.0, 1.0)) = 0.65
//     }

//     SubShader
//     {
//         Tags 
//         { 
//             "Queue"="Transparent" 
//             "IgnoreProjector"="True" 
//             "RenderType"="Transparent" 
//             "PreviewType"="Plane" 
//             "CanUseSpriteAtlas"="True" 
//         }
        
//         Cull Off 
//         Lighting Off 
//         ZWrite Off 
//         Blend SrcAlpha OneMinusSrcAlpha

//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #include "UnityCG.cginc"

//             struct appdata_t
//             {
//                 float4 vertex   : POSITION;
//                 float4 color    : COLOR;
//                 float2 texcoord : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float4 vertex   : SV_POSITION;
//                 fixed4 color    : COLOR;
//                 float2 texcoord  : TEXCOORD0;
//             };

//             fixed4 _Color;
//             fixed4 _StatColor;
//             fixed4 _GridLineColor;
//             float _Health;
//             float _BorderWidth;
//             float _VarianceRangeMin;

//             v2f vert(appdata_t IN)
//             {
//                 v2f OUT;
//                 OUT.vertex = UnityObjectToClipPos(IN.vertex);
//                 OUT.texcoord = IN.texcoord;
//                 OUT.color = IN.color * _Color;
//                 return OUT;
//             }

//             // High-precision distance function for pointy-topped hexagons
//             float PointyHexDistance(float2 p)
//             {
//                 float2 q = abs(p);
//                 return max(q.y * 0.866025 + q.x * 0.5, q.x);
//             }

//             // High-frequency hash pattern for cell variation brightness
//             float HashGrid(float2 p)
//             {
//                 return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
//             }

//             fixed4 frag(v2f IN) : SV_Target
//             {
//                 // ---------------------------------------------------------
//                 // FIXED REMAPPER: REMOVE CARTEASIAN BOUNDS
//                 // ---------------------------------------------------------
//                 // 1. Establish the clean interlocking step ratios 
//                 float2 spacingMatrix = float2(1.5, 1.73205);
                
//                 // 2. Map coordinates precisely to match 18 horizontal units.
//                 // We add an open multiplier scalar on the Y axis to let the 
//                 // calculations overflow past the raw 0-1 texture constraints safely.
//                 float2 scaledUV = IN.texcoord * float2(18.0 * 1.5, 2.0 * 1.73205);
                
//                 // Shift coordinates away from the absolute frame margins to stop clamping compression lines
//                 scaledUV.x += 0.75; 
//                 scaledUV.y += 0.866;

//                 // 3. Voronoi Point Separation Matrix Loop
//                 float2 gridA = frac(scaledUV / spacingMatrix) * spacingMatrix - spacingMatrix * 0.5;
//                 float2 gridB = frac((scaledUV - spacingMatrix * 0.5) / spacingMatrix) * spacingMatrix - spacingMatrix * 0.5;

//                 float2 cellUV, cellID;
//                 if (length(gridA) < length(gridB))
//                 {
//                     cellUV = gridA;
//                     cellID = floor(scaledUV / spacingMatrix);
//                 }
//                 else
//                 {
//                     cellUV = gridB;
//                     cellID = floor((scaledUV - spacingMatrix * 0.5) / spacingMatrix) + 0.5;
//                 }

//                 // 4. Resolve absolute tracking IDs based on center locations rather than bounding lines
//                 float colIndex = cellID.x;
//                 float rowIndex = floor(cellID.y + 0.5);

//                 // HARD GRID LIMIT: If the pixel falls into an invalid column or an extra row, 
//                 // we discard it instantly. This is what breaks the rigid rectangular border!
//                 if (colIndex < 1.0 || colIndex > 18.0 || rowIndex < 1.0 || rowIndex > 2.0)
//                 {
//                     discard;
//                 }

//                 // Normalize index coordinates down for sequential right-to-left math operations
//                 float finalCol = clamp(colIndex - 1.0, 0.0, 17.0);
//                 float finalRow = clamp(rowIndex - 1.0, 0.0, 1.0);

//                 float totalHexes = 36.0;
//                 float hexID = (finalCol * 2.0) + finalRow;
//                 float hexStartThreshold = hexID / totalHexes;

//                 // 5. Draw vector hexagon profiles
//                 float dist = PointyHexDistance(cellUV);
                
//                 float outerRadius = 0.46; 
//                 float innerRadius = outerRadius - _BorderWidth;

//                 float fillMask = smoothstep(innerRadius, innerRadius - 0.005, dist);
//                 float lineMask = smoothstep(innerRadius, innerRadius + 0.005, dist) * smoothstep(outerRadius, outerRadius - 0.005, dist);

//                 // Per-cell brightness variance
//                 float cellSeed = HashGrid(float2(finalCol, finalRow));
//                 float brightnessFactor = lerp(_VarianceRangeMin, 1.0, cellSeed);

//                 // 6. Color Composition Channels
//                 fixed4 fillCol = _StatColor * IN.color;
//                 fillCol.rgb *= brightnessFactor;
//                 fillCol.a *= fillMask;

//                 fixed4 lineCol = _GridLineColor;
//                 lineCol.a *= lineMask;

//                 fixed4 finalColor = lerp(fillCol, lineCol, lineMask);

//                 // Enforce sharp transparency clipping bounds outside the active geometry curves
//                 float compositeVisibilityMask = saturate(fillMask + lineMask);
//                 finalColor.a *= compositeVisibilityMask;

//                 // Health Bar drainage fade math
//                 float hexVisibility = saturate((_Health - hexStartThreshold) / 0.1);
//                 finalColor.a *= lerp(lineMask, 1.0, hexVisibility);
                
//                 return finalColor;
//             }
//             ENDCG
//         }
//     }
// }

// Shader "UI/HexagonStatBarSmooth"
// {
//     Properties
//     {
//         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
//         _Color ("Tint (UI Default)", Color) = (1,1,1,1)
//         _StatColor ("Stat Color Accent", Color) = (1,0,0,1)
//         _GridLineColor ("Grid Line Color", Color) = (0.05, 0.05, 0.05, 0.8)
//         _Health ("Health Percent (0-1)", Range(0,1)) = 1.0
//         _BorderWidth ("Line Thickness", Range(0.01, 0.15)) = 0.03
//         _VarianceRangeMin ("Min Brightness Offset", Range(0.0, 1.0)) = 0.65
//         _ScrollSpeed ("Matrix Scroll Speed", Range(-2.0, 2.0)) = 0.5
//     }

//     SubShader
//     {
//         Tags 
//         { 
//             "Queue"="Transparent" 
//             "IgnoreProjector"="True" 
//             "RenderType"="Transparent" 
//             "PreviewType"="Plane" 
//             "CanUseSpriteAtlas"="True" 
//         }
        
//         Cull Off 
//         Lighting Off 
//         ZWrite Off 
//         Blend SrcAlpha OneMinusSrcAlpha

//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #include "UnityCG.cginc"

//             struct appdata_t
//             {
//                 float4 vertex   : POSITION;
//                 float4 color    : COLOR;
//                 float2 texcoord : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float4 vertex   : SV_POSITION;
//                 fixed4 color    : COLOR;
//                 float2 texcoord  : TEXCOORD0;
//             };

//             fixed4 _Color;
//             fixed4 _StatColor;
//             fixed4 _GridLineColor;
//             float _Health;
//             float _BorderWidth;
//             float _VarianceRangeMin;
//             float _ScrollSpeed;

//             v2f vert(appdata_t IN)
//             {
//                 v2f OUT;
//                 OUT.vertex = UnityObjectToClipPos(IN.vertex);
//                 OUT.texcoord = IN.texcoord;
//                 OUT.color = IN.color * _Color;
//                 return OUT;
//             }

//             // High-precision distance function for pointy-topped hexagons
//             float PointyHexDistance(float2 p)
//             {
//                 float2 q = abs(p);
//                 return max(q.y * 0.866025 + q.x * 0.5, q.x);
//             }

//             // High-frequency hash pattern for cell variation brightness
//             float HashGrid(float2 p)
//             {
//                 return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
//             }

//             fixed4 frag(v2f IN) : SV_Target
//             {
//                 // 1. Establish the precise structural geometry scale for 18 interlocking columns across 2 rows
//                 float2 spacingMatrix = float2(1.5, 1.73205); 
//                 float2 uv = IN.texcoord * float2(18.0 * 1.5, 2.0 * 1.73205);

//                 // Center the layout inside your RectTransform viewport
//                 uv.x += 0.75; 
//                 uv.y += 0.866;

//                 // 2. Generate overlapping offset matrices to cleanly map the interlocking diagonal valleys
//                 float2 gridA = frac(uv / spacingMatrix) * spacingMatrix - spacingMatrix * 0.5;
//                 float2 gridB = frac((uv - spacingMatrix * 0.5) / spacingMatrix) * spacingMatrix - spacingMatrix * 0.5;

//                 // 3. Voronoi Point Evaluation: Bind every individual pixel to its true closest mathematical hexagon center
//                 float2 cellUV, cellID;
//                 if (length(gridA) < length(gridB))
//                 {
//                     cellUV = gridA;
//                     cellID = floor(uv / spacingMatrix);
//                 }
//                 else
//                 {
//                     cellUV = gridB;
//                     cellID = floor((uv - spacingMatrix * 0.5) / spacingMatrix) + 0.5;
//                 }

//                 // 4. Resolve absolute static tracking indexes (0-17 columns, 0-1 rows)
//                 // We lock these coordinates down BEFORE scrolling so that the health bar drain
//                 // mapping (hexID) and brightness hash seeds remain completely stable.
//                 float colIndex = cellID.x;
//                 float rowIndex = floor(cellID.y + 0.5);

//                 // HARD HUD FRAME GRID LIMIT: Discard any rendering elements trying to clip outside our 18x2 container scope
//                 if (colIndex < 1.0 || colIndex > 18.0 || rowIndex < 1.0 || rowIndex > 2.0)
//                 {
//                     discard;
//                 }

//                 float finalCol = clamp(colIndex - 1.0, 0.0, 17.0);
//                 float finalRow = clamp(rowIndex - 1.0, 0.0, 1.0);

//                 float totalHexes = 36.0;
//                 float hexID = (finalCol * 2.0) + finalRow;
//                 float hexStartThreshold = hexID / totalHexes;

//                 // -----------------------------------------------------------------
//                 // FIX: SEAMLESS GEOMETRIC WRAPPING OFFSET
//                 // -----------------------------------------------------------------
//                 // Instead of moving the entire grid system across global boundaries, 
//                 // we apply a scrolling displacement modifier purely to the localized 
//                 // geometry coordinate tracking field (`cellUV.y`). 
//                 //
//                 // 0.866025 represents the half-height scale ceiling of our pointy hex.
//                 // Wrapping the offset cleanly within this bounding window forces the inner
//                 // details of the shapes to slide perfectly from one hex shell into the next.
//                 float scrollOffset = _Time.y * _ScrollSpeed;
//                 cellUV.y = fmod(cellUV.y + scrollOffset, 0.866025);
                
//                 // Keep the wrapped distance bounds balanced cleanly around the center (0)
//                 if (cellUV.y > 0.4330125)  cellUV.y -= 0.866025;
//                 if (cellUV.y < -0.4330125) cellUV.y += 0.866025;
//                 // -----------------------------------------------------------------

//                 // 5. Draw vector hexagon profiles
//                 float dist = PointyHexDistance(cellUV);
                
//                 float outerRadius = 0.46; 
//                 float innerRadius = outerRadius - _BorderWidth;

//                 float fillMask = smoothstep(innerRadius, innerRadius - 0.005, dist);
//                 float lineMask = smoothstep(innerRadius, innerRadius + 0.005, dist) * smoothstep(outerRadius, outerRadius - 0.005, dist);

//                 // Per-cell brightness variance
//                 float cellSeed = HashGrid(float2(finalCol, finalRow));
//                 float brightnessFactor = lerp(_VarianceRangeMin, 1.0, cellSeed);

//                 // 6. Color Composition Channels
//                 fixed4 fillCol = _StatColor * IN.color;
//                 fillCol.rgb *= brightnessFactor;
//                 fillCol.a *= fillMask;

//                 fixed4 lineCol = _GridLineColor;
//                 lineCol.a *= lineMask;

//                 fixed4 finalColor = lerp(fillCol, lineCol, lineMask);

//                 // Enforce sharp transparency clipping bounds outside the active geometry curves
//                 float compositeVisibilityMask = saturate(fillMask + lineMask);
//                 finalColor.a *= compositeVisibilityMask;

//                 // Health Bar drainage fade math
//                 float hexVisibility = saturate((_Health - hexStartThreshold) / 0.1);
//                 finalColor.a *= lerp(lineMask, 1.0, hexVisibility);
                
//                 return finalColor;
//             }
//             ENDCG
//         }
//     }
// }

Shader "UI/HexagonStatBarSmooth"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint (UI Default)", Color) = (1,1,1,1)
        _StatColor ("Stat Color Accent", Color) = (1,0,0,1)
        _GridLineColor ("Grid Line Color", Color) = (0.05, 0.05, 0.05, 0.8)
        _Health ("Health Percent (0-1)", Range(0,1)) = 1.0
        _BorderWidth ("Line Thickness", Range(0.01, 0.15)) = 0.03
        _VarianceRangeMin ("Min Brightness Offset", Range(0.0, 1.0)) = 0.65
        
        // NEW SEPARATE SCROLL SLIDERS:
        _ScrollSpeedX ("Horizontal Scroll Speed", Range(-2.0, 2.0)) = 0.3
        _ScrollSpeedY ("Vertical Scroll Speed", Range(-2.0, 2.0)) = 0.4
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane" 
            "CanUseSpriteAtlas"="True" 
        }
        
        Cull Off 
        Lighting Off 
        ZWrite Off 
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord  : TEXCOORD0;
            };

            fixed4 _Color;
            fixed4 _StatColor;
            fixed4 _GridLineColor;
            float _Health;
            float _BorderWidth;
            float _VarianceRangeMin;
            float _ScrollSpeedX;
            float _ScrollSpeedY;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            // High-precision distance function for pointy-topped hexagons
            float PointyHexDistance(float2 p)
            {
                float2 q = abs(p);
                return max(q.y * 0.866025 + q.x * 0.5, q.x);
            }

            // High-frequency hash pattern for cell variation brightness
            float HashGrid(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // 1. Establish the precise structural geometry scale for 18 interlocking columns across 2 rows
                float2 spacingMatrix = float2(1.5, 1.73205); 
                float2 uv = IN.texcoord * float2(18.0 * 1.5, 2.0 * 1.73205);

                // Center the layout inside your RectTransform viewport
                uv.x += 0.75; 
                uv.y += 0.866;

                // 2. Generate overlapping offset matrices to cleanly map the interlocking diagonal valleys
                float2 gridA = frac(uv / spacingMatrix) * spacingMatrix - spacingMatrix * 0.5;
                float2 gridB = frac((uv - spacingMatrix * 0.5) / spacingMatrix) * spacingMatrix - spacingMatrix * 0.5;

                // 3. Voronoi Point Evaluation: Bind every individual pixel to its true closest mathematical hexagon center
                float2 cellUV, cellID;
                if (length(gridA) < length(gridB))
                {
                    cellUV = gridA;
                    cellID = floor(uv / spacingMatrix);
                }
                else
                {
                    cellUV = gridB;
                    cellID = floor((uv - spacingMatrix * 0.5) / spacingMatrix) + 0.5;
                }

                // 4. Resolve absolute static tracking indexes (0-17 columns, 0-1 rows)
                // Calculated BEFORE scrolling to keep drainage logic and brightness seeds firmly anchored.
                float colIndex = cellID.x;
                float rowIndex = floor(cellID.y + 0.5);

                // HARD HUD FRAME GRID LIMIT: Discard any rendering elements trying to clip outside our 18x2 container scope
                if (colIndex < 1.0 || colIndex > 18.0 || rowIndex < 1.0 || rowIndex > 2.0)
                {
                    discard;
                }

                float finalCol = clamp(colIndex - 1.0, 0.0, 17.0);
                float finalRow = clamp(rowIndex - 1.0, 0.0, 1.0);

                float totalHexes = 36.0;
                float hexID = (finalCol * 2.0) + finalRow;
                float hexStartThreshold = hexID / totalHexes;

                // -----------------------------------------------------------------
                // FIX: BI-DIRECTIONAL SEAMLESS GEOMETRIC WRAPPING OFFSET
                // -----------------------------------------------------------------
                // Apply a scrolling displacement modifier to both the X and Y local fields.
                // X wraps within the standard flat-width bounds of 1.0.
                // Y wraps within the absolute geometric pointy-top height bounds of 0.866025.
                float scrollOffsetX = _Time.y * _ScrollSpeedX;
                float scrollOffsetY = _Time.y * _ScrollSpeedY;

                cellUV.x = fmod(cellUV.x + scrollOffsetX, 1.0);
                if (cellUV.x > 0.5)  cellUV.x -= 1.0;
                if (cellUV.x < -0.5) cellUV.x += 1.0;

                cellUV.y = fmod(cellUV.y + scrollOffsetY, 0.866025);
                if (cellUV.y > 0.4330125)  cellUV.y -= 0.866025;
                if (cellUV.y < -0.4330125) cellUV.y += 0.866025;
                // -----------------------------------------------------------------

                // 5. Draw vector hexagon profiles
                float dist = PointyHexDistance(cellUV);
                
                float outerRadius = 0.46; 
                float innerRadius = outerRadius - _BorderWidth;

                float fillMask = smoothstep(innerRadius, innerRadius - 0.005, dist);
                float lineMask = smoothstep(innerRadius, innerRadius + 0.005, dist) * smoothstep(outerRadius, outerRadius - 0.005, dist);

                // Per-cell brightness variance
                float cellSeed = HashGrid(float2(finalCol, finalRow));
                float brightnessFactor = lerp(_VarianceRangeMin, 1.0, cellSeed);

                // 6. Color Composition Channels
                fixed4 fillCol = _StatColor * IN.color;
                fillCol.rgb *= brightnessFactor;
                fillCol.a *= fillMask;

                fixed4 lineCol = _GridLineColor;
                lineCol.a *= lineMask;

                fixed4 finalColor = lerp(fillCol, lineCol, lineMask);

                // Enforce sharp transparency clipping bounds outside the active geometry curves
                float compositeVisibilityMask = saturate(fillMask + lineMask);
                finalColor.a *= compositeVisibilityMask;

                // Health Bar drainage fade math
                float hexVisibility = saturate((_Health - hexStartThreshold) / 0.1);
                
                // Keeps your board-game border lines permanently drawn on screen, fading only the filled cores
                finalColor.a *= lerp(lineMask, 1.0, hexVisibility);
                
                return finalColor;
            }
            ENDCG
        }
    }
}

// Shader "UI/HexagonStatBarSmooth"
// {
//     Properties
//     {
//         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
//         _Color ("Tint (UI Default)", Color) = (1,1,1,1)
//         _StatColor ("Stat Color Accent", Color) = (1,0,0,1)
//         _GhostColor ("Ghost Damage Color", Color) = (1, 0.6, 0.0, 0.4) // Faded Amber/Yellow
//         _GridLineColor ("Grid Line Color", Color) = (0.05, 0.05, 0.05, 0.8)
//         _Health ("Health Percent (0-1)", Range(0,1)) = 1.0
//         _GhostHealth ("Ghost Percent (0-1)", Range(0,1)) = 1.0 // NEW: Tracks the delayed bar
//         _BorderWidth ("Line Thickness", Range(0.01, 0.15)) = 0.03
//         _VarianceRangeMin ("Min Brightness Offset", Range(0.0, 1.0)) = 0.65
//         _ScrollSpeedX ("Horizontal Scroll Speed", Range(-2.0, 2.0)) = 0.3
//         _ScrollSpeedY ("Vertical Scroll Speed", Range(-2.0, 2.0)) = 0.4
//     }

//     SubShader
//     {
//         Tags 
//         { 
//             "Queue"="Transparent" 
//             "IgnoreProjector"="True" 
//             "RenderType"="Transparent" 
//             "PreviewType"="Plane" 
//             "CanUseSpriteAtlas"="True" 
//         }
        
//         Cull Off 
//         Lighting Off 
//         ZWrite Off 
//         Blend SrcAlpha OneMinusSrcAlpha

//         Pass
//         {
//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #include "UnityCG.cginc"

//             struct appdata_t
//             {
//                 float4 vertex   : POSITION;
//                 float4 color    : COLOR;
//                 float2 texcoord : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float4 vertex   : SV_POSITION;
//                 fixed4 color    : COLOR;
//                 float2 texcoord  : TEXCOORD0;
//             };

//             fixed4 _Color;
//             fixed4 _StatColor;
//             fixed4 _GhostColor;
//             fixed4 _GridLineColor;
//             float _Health;
//             float _GhostHealth;
//             float _BorderWidth;
//             float _VarianceRangeMin;
//             float _ScrollSpeedX;
//             float _ScrollSpeedY;

//             v2f vert(appdata_t IN)
//             {
//                 v2f OUT;
//                 OUT.vertex = UnityObjectToClipPos(IN.vertex);
//                 OUT.texcoord = IN.texcoord;
//                 OUT.color = IN.color * _Color;
//                 return OUT;
//             }

//             float PointyHexDistance(float2 p)
//             {
//                 float2 q = abs(p);
//                 return max(q.y * 0.866025 + q.x * 0.5, q.x);
//             }

//             float HashGrid(float2 p)
//             {
//                 return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
//             }

//             fixed4 frag(v2f IN) : SV_Target
//             {
//                 float2 spacingMatrix = float2(1.5, 1.73205); 
//                 float2 uv = IN.texcoord * float2(18.0 * 1.5, 2.0 * 1.73205);

//                 uv.x += 0.75; 
//                 uv.y += 0.866;

//                 float2 gridA = frac(uv / spacingMatrix) * spacingMatrix - spacingMatrix * 0.5;
//                 float2 gridB = frac((uv - spacingMatrix * 0.5) / spacingMatrix) * spacingMatrix - spacingMatrix * 0.5;

//                 float2 cellUV, cellID;
//                 if (length(gridA) < length(gridB))
//                 {
//                     cellUV = gridA;
//                     cellID = floor(uv / spacingMatrix);
//                 }
//                 else
//                 {
//                     cellUV = gridB;
//                     cellID = floor((uv - spacingMatrix * 0.5) / spacingMatrix) + 0.5;
//                 }

//                 float colIndex = cellID.x;
//                 float rowIndex = floor(cellID.y + 0.5);

//                 if (colIndex < 1.0 || colIndex > 18.0 || rowIndex < 1.0 || rowIndex > 2.0)
//                 {
//                     discard;
//                 }

//                 float finalCol = clamp(colIndex - 1.0, 0.0, 17.0);
//                 float finalRow = clamp(rowIndex - 1.0, 0.0, 1.0);

//                 float totalHexes = 36.0;
//                 float hexID = (finalCol * 2.0) + finalRow;
//                 float hexStartThreshold = hexID / totalHexes;

//                 float scrollOffsetX = _Time.y * _ScrollSpeedX;
//                 float scrollOffsetY = _Time.y * _ScrollSpeedY;

//                 cellUV.x = fmod(cellUV.x + scrollOffsetX, 1.0);
//                 if (cellUV.x > 0.5)  cellUV.x -= 1.0;
//                 if (cellUV.x < -0.5) cellUV.x += 1.0;

//                 cellUV.y = fmod(cellUV.y + scrollOffsetY, 0.866025);
//                 if (cellUV.y > 0.4330125)  cellUV.y -= 0.866025;
//                 if (cellUV.y < -0.4330125) cellUV.y += 0.866025;

//                 float dist = PointyHexDistance(cellUV);
                
//                 float outerRadius = 0.46; 
//                 float innerRadius = outerRadius - _BorderWidth;

//                 float fillMask = smoothstep(innerRadius, innerRadius - 0.005, dist);
//                 float lineMask = smoothstep(innerRadius, innerRadius + 0.005, dist) * smoothstep(outerRadius, outerRadius - 0.005, dist);

//                 float cellSeed = HashGrid(float2(finalCol, finalRow));
//                 float brightnessFactor = lerp(_VarianceRangeMin, 1.0, cellSeed);

//                 // Calculate individual visibility steps for both the current health and ghost damage bars
//                 float mainVisibility = saturate((_Health - hexStartThreshold) / 0.1);
//                 float ghostVisibility = saturate((_GhostHealth - hexStartThreshold) / 0.1);

//                 // Base color profiles
//                 fixed4 mainFill = _StatColor * IN.color;
//                 mainFill.rgb *= brightnessFactor;

//                 fixed4 ghostFill = _GhostColor * IN.color;
//                 ghostFill.rgb *= brightnessFactor; // Keep the same grid variance consistency

//                 // NEW MIX MATRIX: 
//                 // Hexes below current health get the Main Color.
//                 // Hexes above current health but below Ghost Health get the Ghost Color.
//                 fixed4 currentActiveFill = lerp(ghostFill * ghostVisibility, mainFill, mainVisibility);

//                 fixed4 lineCol = _GridLineColor;
//                 lineCol.a *= lineMask;

//                 // Combine the active fills with the tabletop border lines
//                 fixed4 finalColor = lerp(currentActiveFill * fillMask, lineCol, lineMask);

//                 float compositeVisibilityMask = saturate(fillMask + lineMask);
//                 finalColor.a *= compositeVisibilityMask;
                
//                 return finalColor;
//             }
//             ENDCG
//         }
//     }
// }
