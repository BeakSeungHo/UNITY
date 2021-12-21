Shader "Custom/LoadingColor"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
	    _TempColor("Temp Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
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

			CGPROGRAM

			// Physically based Standard lighting model, and enable shadows on all light types

#pragma surface surf Lambert alpha:fade noforwardadd novertexlights noshadow noambient nolightmap 



// Use shader model 3.0 target, to get nicer looking lighting


	#pragma target 2.0
				sampler2D _MainTex;
				half4 _Color;
				fixed4 _TempColor;
				struct Input
				{
					float2 uv_MainTex;
				};
				fixed4 _TeamColor;
				UNITY_INSTANCING_BUFFER_START(Props)

				UNITY_INSTANCING_BUFFER_END(Props)
				void surf(Input IN, inout SurfaceOutput o)
				{
					half4 t1 = (tex2D(_MainTex, IN.uv_MainTex));
					if (t1.a != 0)
					{
						o.Emission = _TempColor;
						o.Alpha = t1.a;
					}
				}

				ENDCG
	}
		FallBack "Diffuse"
}
