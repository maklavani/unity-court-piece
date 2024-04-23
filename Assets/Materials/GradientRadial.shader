Shader "Custom/GradientRadial" {
	Properties {
		_ColorA ("Color A", Color) = (1, 1, 1, 1)
		_ColorB ("Color B", Color) = (0, 0, 0, 1)
		_Slide ("Slide", Range(0, 1)) = 0.5
	}

	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType" = "Plane"}
		LOD 100

		Pass {
			CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				// variables
				fixed4 _ColorA, _ColorB;
				float _Slide;

				struct appdata {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					half2 texcoord : TEXCOORD0;
				};

				v2f vert (appdata input)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(input.vertex);
					o.texcoord = input.texcoord;
					return o;
				}

				fixed4 frag (v2f input) : SV_Target
				{
					float t = length(input.texcoord - float2(0.5, 0.5)) * 1.41421356237;
					return lerp(_ColorA, _ColorB, t + (_Slide - 0.5) * 2);
				}

			ENDCG
		}
	}

	FallBack "Diffuse"
}