Shader "Custom/Gradient" {
	Properties{
		_TopColor("Top Color" , Color) = (1 , 1 , 1 , 1)
		_MiddleColor("Middle Color" , Color) = (1 , 1 , 1 , 1)
		_BottomColor("Bottom Color" , Color) = (1 , 1 , 1 , 1)
		_Scale("Scale" , Range(0.1 , 0.9)) = 0.5
	}

	SubShader{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType" = "Plane"}
		LOD 100

		Pass{
			CGPROGRAM

				#pragma vertex vert
				#pragma fragment frag

				// variables
				fixed4 _BottomColor , _MiddleColor , _TopColor;
				fixed _Scale;

				struct appdata
				{
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
				};

				// Vertex
				v2f vert(appdata input) {
					v2f o;
					o.vertex = UnityObjectToClipPos(input.vertex);
					o.texcoord = input.texcoord;
					return o;
				}

				// Fragment
				fixed4 frag(v2f input) : SV_Target{
					fixed4 col = lerp(_BottomColor , _MiddleColor , input.texcoord.y / _Scale) * step(input.texcoord.y , _Scale);
					col += lerp(_MiddleColor , _TopColor , (input.texcoord.y - _Scale) / (1 - _Scale)) * step(_Scale , input.texcoord.y);
					col.a = 1;
					return col;
				}

			ENDCG
		}
	}

	FallBack "Diffuse"
}