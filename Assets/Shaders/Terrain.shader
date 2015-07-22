Shader "Custom/Terrain" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader {
			Tags { "RenderType" = "Opaque" }
			CGPROGRAM

			#pragma surface surf Lambert fullforwardshadows

			#include "UnityCG.cginc"

			struct Input {
					float2 uv_MainTex;
			};

			uniform float3 playerPosition;
			uniform float4 _Color;

			void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = _Color.rgb;
      }

			ENDCG

	}
	FallBack "Diffuse"
}
