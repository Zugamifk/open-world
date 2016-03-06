Shader "Sprites/AlbedoMood"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
		_DayCycle ("DayCycle", Range(0,1)) = 1
		_Brightness ("Brightness", Range(0,1)) = 1
		_Contrast ("Contrast", RangE(0,1)) = 1
		_Burn("Burn", Range(-1,1)) = 1
		_Overlay("Overlay", Range(0,1))=1
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
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
        Fog { Mode Off }
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON
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
                half2 texcoord  : TEXCOORD0;
            };

            fixed4 _Color;
			float _DayCycle;
			float _Brightness;
			float _Contrast;
			float _Burn;
			float _Overlay;

			float3 ContrastSaturationBrightness( float3 color, float brt, float sat, float con)
			{
				//RGB Color Channels
				float AvgLumR = 0.5;
				float AvgLumG = 0.5;
				float AvgLumB = 0.5;

				//Luminace Coefficients for brightness of image
				float3 LuminaceCoeff = float3(0.2125,0.7154,0.0721);

				//Brigntess calculations
				float3 AvgLumin = float3(AvgLumR,AvgLumG,AvgLumB);
				float3 brtColor = color * brt;
				float intensityf = dot(brtColor, LuminaceCoeff);
				float3 intensity = float3(intensityf, intensityf, intensityf);

				//Saturation calculation
				float3 satColor = lerp(intensity, brtColor, sat);

				//Contrast calculations
				float3 conColor = lerp(AvgLumin, satColor, con);

				return conColor;

			}
			#define BlendColorBurnf(base, blend) 	((blend == 0.0) ? blend : max((1.0 - ((1.0 - base) / blend)), 0.0))
			#define BlendColorDodgef(base, blend) 	((blend == 1.0) ? blend : min(base / (1.0 - blend), 1.0))

			float3 DodgeBurn(float3 color, float param) {
				if(param>0) {
					return lerp(color, float3(BlendColorBurnf(color.r, color.r), BlendColorBurnf(color.g, color.g), BlendColorBurnf(color.b, color.b)), param);
				} else {
					param = -param;
					return lerp(color, float3(BlendColorDodgef(color.r, color.r), BlendColorDodgef(color.g, color.g), BlendColorDodgef(color.b, color.b)), param);
				}
			}

			#define BlendOverlayf(base, blend) 	(base < 0.5 ? (2.0 * base * blend) : (1.0 - 2.0 * (1.0 - base) * (1.0 - blend)))
			float3 Overlay(float3 color, float param) {
				return lerp(color, float3(BlendOverlayf(color.r, color.r), BlendOverlayf(color.g, color.g), BlendOverlayf(color.b, color.b)), param);
			}

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
				c.rgb = ContrastSaturationBrightness( c.rgb, _Brightness, _DayCycle, _Contrast);
				c.rgb=DodgeBurn(c.rgb,_Burn);
				c.rgb=Overlay(c.rgb,_Overlay);
                c.rgb *= c.a;
                return c;
            }
        ENDCG
        }
    }
}
