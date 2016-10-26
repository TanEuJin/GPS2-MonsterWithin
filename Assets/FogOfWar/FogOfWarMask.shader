Shader "Custom/FogOfWarMask" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BlurPower("BlurPower", float) = 0.002
	}

	SubShader {
		Tags {"Queue"="Transparent" "RenderType"="Transparent" "LightMode"="ForwardBase"}
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 200

		CGPROGRAM
		#pragma surface surf NoLighting Lambert noambient alpha:blend

		fixed4 LightingNoLighting(SurfaceOutput s, fixed lightDir, float aten)
		{
			fixed4 color;
			color.rgb = s.Albedo;
			color.a = s.Alpha;
			return color;
		}

		sampler2D _MainTex;
		fixed4 _Color;
		float _BlurPower;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			//fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			half4 baseColor1 = tex2D (_MainTex, IN.uv_MainTex + float2(-_BlurPower, 0));
			half4 baseColor2 = tex2D (_MainTex, IN.uv_MainTex + float2(0, -_BlurPower));
			half4 baseColor3 = tex2D (_MainTex, IN.uv_MainTex + float2(_BlurPower, 0));
			half4 baseColor4 = tex2D (_MainTex, IN.uv_MainTex + float2(0, _BlurPower));
			half4 baseColor = 0.25 * (baseColor1 + baseColor2 + baseColor3 + baseColor4);

			o.Albedo = _Color.rgb * baseColor.b;
			o.Alpha = _Color.a - baseColor.g; //green - color of aperture 
		}
		ENDCG
	}

	Fallback "Legacy Shaders/Transparent/VertexLit"
}
