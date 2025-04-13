Shader "Custom/BlurTilemapShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0.0, 2.0)) = 0.5
        _Saturation ("Saturation", Range(0.0, 1.0)) = 0.5
        _Brightness ("Brightness", Range(0.5, 1.5)) = 0.85
        _Contrast ("Contrast", Range(0.5, 1.5)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _BlurAmount;
            float _Saturation;
            float _Brightness;
            float _Contrast;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Original color
                fixed4 originalColor = tex2D(_MainTex, i.uv);
                
                // Very gentle blur - just enough to soften details
                fixed4 blurredColor = fixed4(0, 0, 0, 0);
                float totalWeight = 0;
                
                const int samples = 2; // Very small sample count
                for (int x = -samples; x <= samples; x++)
                {
                    for (int y = -samples; y <= samples; y++)
                    {
                        float weight = 1.0 / (1.0 + x*x + y*y);
                        float2 offset = float2(x, y) * _MainTex_TexelSize.xy * _BlurAmount;
                        fixed4 sampleColor = tex2D(_MainTex, i.uv + offset);
                        
                        blurredColor += sampleColor * weight;
                        totalWeight += weight;
                    }
                }
                
                blurredColor /= totalWeight;
                
                // Preserve original alpha
                blurredColor.a = originalColor.a;
                
                // Convert to grayscale with adjustable saturation
                float luminance = dot(blurredColor.rgb, float3(0.299, 0.587, 0.114));
                fixed3 desaturatedColor = lerp(float3(luminance, luminance, luminance), blurredColor.rgb, _Saturation);
                
                // Apply brightness and contrast
                fixed3 finalColor = (desaturatedColor - 0.5) * _Contrast + 0.5;
                finalColor *= _Brightness;
                
                return fixed4(finalColor, blurredColor.a) * i.color;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}