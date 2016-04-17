Shader "Sprites/Sun"
{
	Properties
	{
		[PerRendererData]_MainTex ("Sun base", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		_Phase ("Phase offset", Range(-1,1)) = 0
		_Noise ("Noise level", Range(0,1)) = 0
		_NoiseMix ("Noise mix", Range(0,1)) = 0
		_Variance ("Variance", Range(0,16)) = 0
		_Variance1 ("Variance1", Range(0,16)) = 0
		_Variance2 ("Variance2", Range(0,16)) = 0
		_Resolution ("Resolution", Float) = 0
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
        Blend SrcAlpha OneMinusSrcAlpha
 
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
            
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif
 
                return OUT;
            }
 
			static const float pi = 3.14159;

            sampler2D _MainTex;
            uniform float _Resolution;
            uniform float _Variance;
            uniform float _Variance1;
            uniform float _Variance2;
            uniform float _Phase;
            uniform float _Noise;
            uniform float _NoiseMix;
			
			fixed2 fix(half2 uv) {
				return floor(uv * _Resolution)/_Resolution;
			}
			
			float rand(half2 myVector)  {
				return frac(sin( dot(myVector ,half2(12.9898,78.233) )) * 43758.5453);
			}

            fixed4 frag(v2f IN) : COLOR
            {
				half2 px = IN.texcoord;
				px -= 0.5;
				px = fix(px);
				float dis = length(px)*2;
				float tog = .5*sin(
					dis*pi*_Variance-_Time[1] + 
						_NoiseMix*pi *
						lerp(min(1,2-dis*2),1,_Noise) *
						(1 - 2*rand( px*_Resolution + floor( rand( px*_Resolution ) + _Time[1] ))));
				fixed ang = atan2(px.x,px.y)+_Phase*pi;
				tog *= max(0, sin(_Time[2]*(.5+_Phase)+ang*pi*2*_Variance1) + 
						sin(-_Time[1]*(.5+_Phase)+ang*pi*2*_Variance2));

				half2 uv = IN.texcoord;

			    half4 texcol = tex2D (_MainTex, uv);              
                texcol = texcol * IN.color;
				texcol.a = min(texcol.a, ceil(tog));
                return texcol;
            }
        ENDCG
        }
    }
}
