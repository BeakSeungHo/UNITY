Shader "Sprites/UnitShader"
{
	Properties
	{
		 _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)

		_TintTex("Tint Texture", 2D) = "white" {}
		// Add values to determine if outlining is enabled and outline color.
		_TeamColor("Team Color", Color) = (1,1,1,1)
		_SpriteColor("Sprite Color", Color) = (1,1,1,1)
		_RenderState("Render State", int) = 0
		[PerRendererData] _Outline("Outline", Float) = 0
		[PerRendererData] _OutlineColor("Outline Color", Color) = (1,1,1,1)
		[PerRendererData] _OutlineSize("Outline Size", int) = 1
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


			Pass
			{
			CGPROGRAM
				#pragma vertex SpriteVert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_instancing
				#pragma multi_compile _ PIXELSNAP_ON
				#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
				#include "UnitySprites.cginc"

				float _Outline;
				fixed4 _OutlineColor;
				int _OutlineSize;
				float4 _MainTex_TexelSize;

				fixed4 frag(v2f IN) : SV_Target
				{
					fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;

				// If outline is enabled and there is a pixel, try to draw an outline.
				if (_Outline > 0 && c.a == 0)
				{
					float totalAlpha = 0.0;

					[unroll(16)]
					for (int i = 1; i < _OutlineSize + 1; i++) {
						fixed4 pixelUp = tex2D(_MainTex, IN.texcoord + fixed2(0, i * _MainTex_TexelSize.y));
						fixed4 pixelDown = tex2D(_MainTex, IN.texcoord - fixed2(0,i *  _MainTex_TexelSize.y));
						fixed4 pixelRight = tex2D(_MainTex, IN.texcoord + fixed2(i * _MainTex_TexelSize.x, 0));
						fixed4 pixelLeft = tex2D(_MainTex, IN.texcoord - fixed2(i * _MainTex_TexelSize.x, 0));

						totalAlpha = totalAlpha + pixelUp.a + pixelDown.a + pixelRight.a + pixelLeft.a;
					}

					if (totalAlpha > 0)
					{
						c.rgba = fixed4(1, 1, 1, 1) * _OutlineColor;
					}
				}

				c.rgb *= c.a;

				return c;
			}
		ENDCG
		}


			CGPROGRAM

				// Physically based Standard lighting model, and enable shadows on all light types

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
				fixed4 _TeamColor;
				fixed4 _SpriteColor;
				int _RenderState;
				UNITY_INSTANCING_BUFFER_START(Props)

				UNITY_INSTANCING_BUFFER_END(Props)
				void surf(Input IN, inout SurfaceOutput o)
				{
					if (_RenderState == 1)
					{
						//half4 t = (tex2D(_TintTex, IN.uv_TintTex));
						//o.Emission = (t.rgb) * _TeamColor.rgb;
						//o.Alpha = t.a * _TeamColor.a;
						half4 t1 = (tex2D(_MainTex, IN.uv_MainTex));
						half4 t2 = (tex2D(_TintTex, IN.uv_TintTex));
						float test = t1.b * _Color;

						o.Emission = lerp(t1, test * _TeamColor, t2.a);
						o.Alpha = t1.a * _TeamColor.a * _SpriteColor.a;
					}
				}

				ENDCG
		}
			FallBack "Diffuse"
}
