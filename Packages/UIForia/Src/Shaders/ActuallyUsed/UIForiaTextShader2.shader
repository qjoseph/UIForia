Shader "UIForia/UIForiaText2"
{
    Properties
    {
        _FaceTex ("Face Texture", 2D) = "white" {}
        _OutlineTex	("Outline Texture", 2D) = "white" {}
        _MainTex ("Texture", 2D) = "white" {}
        // required for UI.Mask
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15
        
        _FaceUVSpeedX ("Face UV Speed X", Range(-5, 5)) = 0.0
	    _FaceUVSpeedY ("Face UV Speed Y", Range(-5, 5)) = 0.0
	    _OutlineUVSpeedX ("Outline UV Speed X", Range(-5, 5)) = 0.0
	    _OutlineUVSpeedY ("Outline UV Speed Y", Range(-5, 5)) = 0.0
	  
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "UIFoira::SupportClipRectBuffer"="TEXCOORD1.x"
            "UIForia::SupportMaterialBuffer"="8"
        }
        LOD 100

        Pass {
            Cull off
            ColorMask [_ColorMask]
            Blend One OneMinusSrcAlpha
            Stencil {
                Ref [_Stencil]
                Comp [_StencilComp]
                Pass [_StencilOp] 
                ReadMask [_StencilReadMask]
                WriteMask [_StencilWriteMask]
            }
            
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.5
            
            #include "UnityCG.cginc"
            #include "UIForia.cginc"
            #include "Quaternion.cginc"
           
             struct appdata {
                uint vid : SV_VertexID;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float4 debug : TEXCOORD0;
                float2 texCoord0 : TEXCOORD1;
                float2 texCoord1 : TEXCOORD2;
                float4 param : TEXCOORD3;
                int2 indices : TEXCOORD4;
            };
            
            struct UIForiaVertex {
                float2 position;
                float2 texCoord0; // sdf uvs (inset by half a pixel)
                float2 texCoord1; // texture uvs
                int2 indices;     // lookup indices in buffers packed into ushorts
            };
             
            sampler2D _MainTex;
            sampler2D _FontTexture;
            sampler2D _OutlineTex;
            
            float _SpriteAtlasPadding;
            float _UIForiaDPIScale;
            float4x4 _UIForiaOriginMatrix;


            StructuredBuffer<float4x4> _UIForiaMatrixBuffer;            
            StructuredBuffer<UIForiaVertex> _UIForiaVertexBuffer;            
            StructuredBuffer<AxisAlignedBounds2D> _UIForiaClipRectBuffer;            
            StructuredBuffer<TextMaterialInfo> _UIForiaMaterialBuffer; 
            StructuredBuffer<GPUGlyphInfo> _UIForiaGlyphBuffer; 
            StructuredBuffer<GPUFontInfo> _UIForiaFontBuffer;
            
            #define TOP_LEFT 0
            #define TOP_RIGHT 1
            #define BOTTOM_RIGHT 2
            #define BOTTOM_LEFT 3
                     
          
            v2f vert (appdata v) {
                v2f o;
                
                int vertexId = v.vid & 0xffffff;
                int cornerIdx = (v.vid >> 24) & (1 << 24) - 1;
                
                UIForiaVertex vertex = _UIForiaVertexBuffer[vertexId];

                // todo --
                    // font index
                    // material buffer
                    // compute character scale
                    
                uint glyphIndex = (uint)(vertex.texCoord0.x);
                uint fontIndex = (uint)(vertex.texCoord0.y);
                
                GPUGlyphInfo glyphInfo = _UIForiaGlyphBuffer[glyphIndex];
                GPUFontInfo fontInfo = _UIForiaFontBuffer[fontIndex];
                
                TextMaterialInfo materialInfo;
                
                materialInfo.zPosition = 0;
                
                materialInfo.faceColor = 0;
                materialInfo.outlineColor = 0;
                materialInfo.glowColor = 0;
                materialInfo.underlayColor = 0;
                
                materialInfo.opacity = 1;
                
                materialInfo.outlineWidth = 0;
                materialInfo.outlineSoftness = 0;
                materialInfo.faceDilate = 0;
                
                materialInfo.glowOffset = 0;
                materialInfo.glowOuter = 0;
                materialInfo.glowInner = 0;
                materialInfo.glowPower = 0;
                
                materialInfo.underlayX = 0;
                materialInfo.underlayY = 0;
                materialInfo.underlaySoftness = 0;
                materialInfo.underlayDilate = 0;
                
                float left = glyphInfo.atlasX / fontInfo.atlasWidth;
                float top = glyphInfo.atlasY / fontInfo.atlasHeight;
                float right = left + (glyphInfo.atlasWidth / fontInfo.atlasWidth);
                float bottom = top + (glyphInfo.atlasHeight / fontInfo.atlasHeight);
                
                float3 ratios = ComputeSDFTextScaleRatios(fontInfo, materialInfo);
                float padding = GetTextSDFPadding(fontInfo.gradientScale, materialInfo, ratios);
                int isBold = 0;
                
                float weight = 0;
                float stylePadding = 0;
               
                if (isBold != 0) {
                    weight = fontInfo.weightBold;
                    stylePadding = fontInfo.boldStyle / 4.0 * fontInfo.gradientScale * ratios.x;
                }
                else {
                    weight = fontInfo.weightNormal;
                    stylePadding = fontInfo.normalStyle / 4.0 * fontInfo.gradientScale;
                }
                
                if (stylePadding + padding > fontInfo.gradientScale) {
                    padding = fontInfo.gradientScale - stylePadding;
                }
                
                // treat position as baseline position to rest on (so lower left corner)
                                
                float fontSize = 36; // todo -- pass via material or character directly
 
                float smallCapsMultiplier = 1;
                float m_fontScale = fontSize * smallCapsMultiplier / fontInfo.pointSize * fontInfo.scale;
                float elementScale = m_fontScale * 1; // font scale multiplier
                
                padding += stylePadding;
                float scaledPaddingWidth = padding / fontInfo.atlasWidth;
                float scaledPaddingHeight = padding / fontInfo.atlasHeight;
                
                float charWidth = glyphInfo.width * elementScale;
                float charHeight = glyphInfo.height * elementScale;
                float offsetY = glyphInfo.yOffset * elementScale;
                
                float2 center = vertex.position + float2(charWidth, charHeight) * 0.5;
                
                float zPosition = 0;

                float3 vpos = float3(vertex.position.xy, zPosition);
    
                vpos.y += offsetY;
                
                if(cornerIdx == TOP_LEFT) {
                    vpos.x -= padding * elementScale;
                    vpos.y += padding * elementScale;
                    o.texCoord0 = float2(left - scaledPaddingWidth, bottom  + scaledPaddingHeight);
                }
                else if(cornerIdx == TOP_RIGHT) {
                    vpos.x += charWidth; 
                    vpos.x +=  padding * elementScale;
                    vpos.y += padding * elementScale;
                    o.texCoord0 = float2(right + scaledPaddingWidth, bottom + scaledPaddingHeight);
                }
                else if(cornerIdx == BOTTOM_RIGHT) {
                    vpos.x += charWidth; 
                    vpos.y -= charHeight;
                    vpos.x += padding * elementScale;
                    vpos.y -= padding * elementScale;
                    o.texCoord0 = float2(right + scaledPaddingWidth, top - scaledPaddingHeight);
                }
                else {
                    vpos.y -= charHeight;
                    vpos.x -= padding * elementScale;
                    vpos.y -= padding * elementScale;
                    o.texCoord0 = float2(left - scaledPaddingWidth, top - scaledPaddingHeight);
                }
                
                // vertex.position = RotateUV(vertex.position, frac(_Time.y) * (2 * PI), vertex.position + float2(vertex.texCoord1.x * 0.5, vertex.texCoord1.y * 0.5));
                            //  float sin = Mathf.Sin(frequency * animatorTime+ charIndex * waveSize) * amplitude * effectIntensity;
                            //
                            //            //bottom, torwards one direction
                            //            data.vertices[0] += Vector3.right * sin;
                            //            data.vertices[3] += Vector3.right * sin;
                            //            //top, torwards the opposite dir
                            //            data.vertices[1] += Vector3.right * -sin;
                            //            data.vertices[2] += Vector3.right * -sin;
                //if(vertexId == 0) {
                    float effectIntensity = 1;
                    float amp = 1;
                    float waveSize = 25;
                    float frequency = 150;
                    float s = sin(frequency * frac(_Time.y) + vertexId * waveSize) * amp * effectIntensity;  
                    // offset model
                    //  1 base position (bottom left or center)
                    //  4 override positions which are added to each vertex as an offset
                    //  should handle all transformations just fine. if you fuck with uniformity of vertices thats fine but its your problem if it looks goofy
                    //  size scale needs to handled somehow in a way that makes sense. i can provide 
                    //  its computable though so probably fine
                    
                      if(cornerIdx == 0 || cornerIdx == 2) {
                            vpos.x += s;
                            vpos.y -= s;  
                            //vpos.x += lerp(0, charWidth * 0.5, 1-frac(_Time.y));
                        }
                        else {
                            vpos.x -= s;
                            vpos.y += s;  
                            //vpos.x -= lerp(0, charWidth * 0.5, 1- frac(_Time.y));
                        }

                    // cpu transform makes sense, upload in float4 buffer
                    // how do I handle font size nicely?
                    // just making vertices bigger sucks
                    // just provide a scale and offset
                    // then the user can supply pivots.
                    // values should be the same between effect buffer and what I compute based on font size except for sdf adjustment which should be transparent
                    
                    // or they transform the the point with a matrix
                    // and i have a some sort of 
                    // if effects run after layout, I should have a layout matrix for the text info
                    
                    // matrix enough? scale inwards do the trick? with different pivot maybe
                    
                    // vpos.xy = RotateUV(vpos.xy, frac(_Time.y) * 2 * (PI), center);
                    // float4 q = rotate_angle_axis(frac(_Time.y * 0.5) * 2 * (PI), float3(1, 1, 1));
                    // float4 midpoint = float4(center.xy, 0, 1);
                    // float3 offset = vpos - float3(center.xy, 0);
                    // vpos = rotate_vector(offset, q) + float3(center.xy, 0);
                    
                    //if(cornerIdx == 0 || cornerIdx == 1) {
                    //    vpos.y -= sin(frac(_Time.y)) * charHeight * 0.2;
                    //}
                    
                //}
                
                int matrixIndex = UnpackMatrixId(vertex.indices);
                int materialIndex = UnpackMaterialId(vertex.indices);
             
                float4x4 transform = mul(_UIForiaMatrixBuffer[matrixIndex], _UIForiaOriginMatrix);
                
                o.vertex = mul(UNITY_MATRIX_VP, mul(transform, float4(vpos, 1.0)));
                
                float2 pixelSize = o.vertex.w;
                float2 scaleXY = float2(1, 1);
                float _Sharpness = 0;
                float charScale = fontSize / fontInfo.pointSize;
                o.debug = float4(vertex.position.xy, charScale, scaledPaddingWidth);

                pixelSize /= scaleXY * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));
                float scale = rsqrt(dot(pixelSize, pixelSize));
                
                scale *= abs(charScale) * fontInfo.gradientScale * (_Sharpness + 1);
                
			    weight = (weight + materialInfo.faceDilate) * ratios.x * 0.5;

		    	float bias = (0.5 - weight) + (0.5 / scale);
	    		float alphaClip = (1.0 - materialInfo.outlineWidth * ratios.x - materialInfo.outlineSoftness * ratios.x);

                if(materialInfo.glowOffset != 0 || materialInfo.glowInner != 0 || materialInfo.glowOuter != 0 || materialInfo.glowPower != 0) {
 				//    alphaClip = min(alphaClip, 1.0 - materialInfo.glowOffset * ratios.y - materialInfo.glowOuter * ratios.y);
                } 
                
				alphaClip = alphaClip / 2.0 - (0.5 / scale) - weight;
                
                o.texCoord1 = vertex.texCoord1;
                o.indices = vertex.indices;
                o.param = float4(alphaClip, scale, bias, ratios.x);
                
                return o;
            }
            
            fixed4 GetColor(half d, fixed4 faceColor, fixed4 outlineColor, half outline, half softness) {
                half faceAlpha = 1 - saturate((d - outline * 0.5 + softness * 0.5) / (1.0 + softness));
                half outlineAlpha = saturate((d + outline * 0.5)) * sqrt(min(1.0, outline));
            
                faceColor.rgb *= faceColor.a;
                outlineColor.rgb *= outlineColor.a;
            
                faceColor = lerp(faceColor, outlineColor, outlineAlpha);
            
                faceColor *= faceAlpha;
            
                return faceColor;
            }
            
            fixed4 frag (v2f i) : SV_Target {
            
                TextMaterialInfo materialInfo;
                
                materialInfo.zPosition = 0;
                
                materialInfo.faceColor = 0;
                materialInfo.outlineColor = 0;
                materialInfo.glowColor = 0;
                materialInfo.underlayColor = 0;
                
                materialInfo.opacity = 1;
                
                materialInfo.outlineWidth = 0;
                materialInfo.outlineSoftness = 0;
                materialInfo.faceDilate = 0;
                
                materialInfo.glowOffset = 0;
                materialInfo.glowOuter = 0;
                materialInfo.glowInner = 0;
                materialInfo.glowPower = 0;
                
                materialInfo.underlayX = 0;
                materialInfo.underlayY = 0;
                materialInfo.underlaySoftness = 0;
                materialInfo.underlayDilate = 0;
                
                float c = tex2Dlod(_FontTexture, float4(i.texCoord0, 0, 0)).a;
                
                float alphaClip = i.param.x;
                float scale = i.param.y;
                float bias = i.param.z;
                float ratioA = i.param.w;
                float sd = (bias - c) * scale;
                
                float outline = (materialInfo.outlineWidth * ratioA) * scale;
			    float softness = (materialInfo.outlineSoftness * ratioA) * scale;
			    
                half4 faceColor = fixed4(0, 0, 0, 1); // UnpackColor(asuint(materialInfo.faceColor));
                half4 outlineColor = fixed4(0, 0, 0, 1); //UnpackColor(asuint(materialInfo.outlineColor));
    
                faceColor = GetColor(sd, faceColor, outlineColor, outline, softness);

                // if not using underlay
                clip(c - alphaClip);
                
                return faceColor;
                
                // faceColor *= tex2D(_FaceTex, i.texCoord1 + float2(0, 0) * _Time.y);
                // outlineColor *= tex2D(_OutlineTex, i.texCoord1 + float2(0, 0) * _Time.y);
            
            }
            
            ENDCG
        }
    }
}
