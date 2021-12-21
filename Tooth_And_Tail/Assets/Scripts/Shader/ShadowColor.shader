Shader "Custom/ShadowColor"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
	}
		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha


			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
		   #pragma surface surf Lambert alpha:fade noforwardadd novertexlights noshadow noambient nolightmap 



	// Use shader model 3.0 target, to get nicer looking lighting

	#pragma target 2.0
				sampler2D _MainTex;
				half4 _Color;
				half4 _TempColor = (1, 1, 1, 1);
				struct Input
				{
					float2 uv_MainTex;
				};
				UNITY_INSTANCING_BUFFER_START(Props)

				UNITY_INSTANCING_BUFFER_END(Props)
				void surf(Input IN, inout SurfaceOutput o)
				{
					half4 t1 = (tex2D(_MainTex, IN.uv_MainTex));
	
					o.Emission = (t1.rgb + _Color.rgb) * t1.a;

					o.Alpha = t1.a * _Color.a;

				}

				ENDCG
		}
			FallBack "Diffuse"
}
