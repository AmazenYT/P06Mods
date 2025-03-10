Shader "Custom/Vertex Group/Vertex Color GUI" {
	Properties{
		_MainColor("Main Color", Color) = (5,5,5,5)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_BlendColor("Blend Color", Color) = (1,1,1,1)
		_BlendTex("Blend Texture", 2D) = "white" {}
		_BlendNormal("Blend Normal", 2D) = "bump" {}
		_Specular("Specular", Range(0, 1)) = 0
		_Gloss("Gloss", Range(0.01, 1)) = 0.01

		_MainScrollX("Main Scroll X", Range(0, 1)) = 0
		_MainScrollY("Main Scroll Y", Range(0, 1)) = 0
		_BlendScrollX("Blend Scroll X", Range(0, 1)) = 0
		_BlendScrollY("Blend Scroll Y", Range(0, 1)) = 0

		_CutoffThre("Cutoff Threshold", Range(0, 1)) = 0.1
		_TransInt("Transparency Intensity", Range(0, 1)) = 1

		_CubeInt("Cubemap Intensity", Range(0, 1)) = 0
		_CubeNrmSgt("Normal Strength", Range(0, 1)) = 4

		_RenderingMode("Rendering Mode", Float) = 0
		_SrcBlend("Source Blend", Float) = 0
		_DstBlend("Distance Blend", Float) = 0
		_Cull("Cull Mode", Float) = 2
		_ZWrite("Z Write", Float) = 0
		_Cutoff("Alpha cutoff", Range(0, 1)) = 0.5

		[PerRendererData] _ExtFresColor("Extra Fresnel Color", Color) = (7,7,7,7)
		[PerRendererData] _ExtFresPower("Extra Fresnel Power", Range(0, 10)) = 0
		[PerRendererData] _ExtFresThre("Extra Fresnel Threshold", Range(0, 5)) = 0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		Blend [_SrcBlend] [_DstBlend]
		Cull [_Cull]
		ZWrite [_ZWrite]
		LOD 200

		CGPROGRAM
		#pragma surface surf ColoredSpecular addshadow exclude_path:prepass alphatest:_Cutoff keepalpha fullforwardshadows
		#pragma shader_feature BLEND_TEXTURE
		#pragma shader_feature TRANSPARENT_BLEND

		#pragma shader_feature CUTOFF_FACTOR
		#pragma shader_feature TRANSPARENT_INT

		#pragma shader_feature USE_SCROLLING
		#pragma shader_feature USE_CUBEMAP

		#pragma shader_feature CUBE_ALPHAMULT

		#pragma target 3.0

		sampler2D _MainTex, _Normal;
		#if (BLEND_TEXTURE)
			sampler2D _BlendTex, _BlendNormal;
		#endif
		#if (USE_CUBEMAP)
			samplerCUBE _Cube;
			half _CubeInt, _CubeNrmSgt;
		#endif
		fixed4 _MainColor, _BlendColor, _ExtFresColor;
		half _Specular, _Gloss, _CutoffThre, _TransInt, _ExtFresPower, _ExtFresThre;
		#if (USE_SCROLLING)
			half _MainScrollX, _MainScrollY, _BlendScrollX, _BlendScrollY;
		#endif

		struct Input {
			float2 uv_MainTex;
			#if (BLEND_TEXTURE)
				float2 uv_BlendTex;
			#endif
			float4 color : COLOR;
			float3 viewDir;
			#if (USE_CUBEMAP)
				float3 worldRefl;
				INTERNAL_DATA
			#endif
		};

		inline half4 LightingColoredSpecular(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten) {
			half3 n = normalize(s.Normal);
			half3 h = normalize(lightDir + viewDir);
			half diff = max(0, dot(n, lightDir));
			float nh = max(0, dot(n, h));
			float spec = pow(nh, s.Gloss * 128.0) * s.Specular;

			half4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff + _LightColor0.rgb * spec) * (atten);
			c.a = s.Alpha;
			return c;
		}

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf(Input IN, inout SurfaceOutput o) {
			half ExtFresnel = pow(1 - saturate(dot(normalize(IN.viewDir), normalize(o.Normal))), _ExtFresPower);
			ExtFresnel = clamp(ExtFresnel, 0.01, ExtFresnel);

			fixed4 output;
			fixed3 normaloutput; // Make switchable in future changes

			fixed2 MainUV = IN.uv_MainTex;
			#if (BLEND_TEXTURE)
				fixed2 BlendUV = IN.uv_BlendTex;
			#endif

			#if (USE_SCROLLING)
				MainUV += fixed2(frac(_MainScrollX * _Time.y), frac(_MainScrollY * _Time.y));
				#if (BLEND_TEXTURE)
					BlendUV += fixed2(frac(_BlendScrollX * _Time.y), frac(_BlendScrollY * _Time.y));
				#endif
			#endif

			#if (BLEND_TEXTURE)
				output = lerp(tex2D(_BlendTex, MainUV) * _BlendColor, tex2D(_MainTex, BlendUV) * _MainColor, IN.color.a);
				normaloutput = lerp(UnpackNormal(tex2D(_BlendNormal, MainUV)), UnpackNormal(tex2D(_Normal, BlendUV)), IN.color.a);
			#else
				output = tex2D(_MainTex, MainUV) * _MainColor;
				normaloutput = UnpackNormal(tex2D(_Normal, MainUV));
			#endif

			o.Albedo = output.rgb * IN.color.rgb;
			o.Specular = _Specular * IN.color.rgb;
			o.Gloss = _Gloss;
			o.Gloss = clamp(o.Gloss, 0.01, 1);
			o.Normal = normaloutput;
			#if (USE_CUBEMAP)
				#if (CUBE_ALPHAMULT)
					o.Albedo += _CubeInt * texCUBE(_Cube, WorldReflectionVector(IN, o.Normal * _CubeNrmSgt)) * IN.color.rgb * output.a;
				#else
					o.Albedo += _CubeInt * texCUBE(_Cube, WorldReflectionVector(IN, o.Normal * _CubeNrmSgt)) * IN.color.rgb;
				#endif
			#endif
			o.Emission = ExtFresnel * _ExtFresColor * _ExtFresThre;
			#if (TRANSPARENT_BLEND)
				o.Alpha = output.a * IN.color.a;
				#if (TRANSPARENT_INT)
					o.Alpha *= _TransInt;
				#endif
			#else
				#if (CUTOFF_FACTOR)
					o.Alpha = IN.color.a;
					float Factor;
					if (output.a > _CutoffThre) {
						Factor = 1;
					} else {
						Factor = 0;
					}
					o.Alpha *= Factor;
				#else
					o.Alpha = output.a * IN.color.a;
				#endif
			#endif
		}
		ENDCG
	}
	FallBack "Specular"
	CustomEditor "CustomShaderGUI"
}