Shader "Custom/TileSplat"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Tex_Splat("Splat", 2D) = "white" {}
		_Brightness("Brightness", Range(0, 5)) = 1.0
		_Alpha("Alpha", Range(0, 1)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard alpha:fade

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
		sampler2D _Tex_Splat;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_Tex_Splat;
        };

		half _Brightness;
		half _Alpha;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 t = tex2D(_Tex_Splat, IN.uv_Tex_Splat);
			
			o.Albedo = c.rgb * _Brightness;

            o.Alpha = t.a * _Alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
