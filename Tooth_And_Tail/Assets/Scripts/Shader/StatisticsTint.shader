Shader "Custom/StatisticsTint"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_TintTex("Albedo (RGB)", 2D) = "white" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
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
#pragma surface surf Lambert alpha:fade noforwardadd novertexlights noshadow noambient nolightmap 



			// Use shader model 3.0 target, to get nicer looking lighting

			#pragma target 2.0
				sampler2D _MainTex;
				sampler2D _TintTex;
				half4 _Color;
				struct Input
				{
					float2 uv_MainTex;
					float2 uv_TintTex;
				};
				UNITY_INSTANCING_BUFFER_START(Props)

				UNITY_INSTANCING_BUFFER_END(Props)
				void surf(Input IN, inout SurfaceOutput o)
				{
						half4 t1 = (tex2D(_MainTex, IN.uv_MainTex));
						half4 t2 = (tex2D(_TintTex, IN.uv_TintTex));

						o.Emission = lerp(t1, (t1.b * _Color) * _Color, t2.a);
						o.Alpha = t1.a * _Color.a;			
				}
				ENDCG
		}
			FallBack "Diffuse"
}
