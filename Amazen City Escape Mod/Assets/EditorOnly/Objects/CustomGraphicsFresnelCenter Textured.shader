Shader "Custom/Graphics/Fresnel/Center Textured" {
	Properties {
	    _MainTex("Texture", 2D) = "white" {}
	    _RimPower("Rim Power", Range(0, 5)) = 1
		_RimThreshold("Rim Threshold", Range(0, 5)) = 1
		_InvFade("Soft Particles Factor", Range(0.01, 3)) = 1.0
	}
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend One One
		Lighting Off
		ZWrite Off

		CGPROGRAM
		#pragma surface surf NoLighting vertex:vert nofog
		#pragma multi_compile _ SOFTPARTICLES_ON

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) {
			return fixed4(0, 0, 0, s.Alpha);
		}
		
		sampler2D_float _CameraDepthTexture;
		sampler2D _MainTex;
		half _RimPower, _RimThreshold, _InvFade;

		struct Input {
			float2 uv_MainTex;
			float3 viewDir;
			float4 color : COLOR;
			#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
			#endif
		};

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			#ifdef SOFTPARTICLES_ON
				float4 hpos = UnityObjectToClipPos(v.vertex);
				o.projPos = ComputeGrabScreenPos(hpos);
				COMPUTE_EYEDEPTH(o.projPos.z);
			#endif
		}

		void surf (Input IN, inout SurfaceOutput o) {
			#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(IN.projPos)));
				float partZ = IN.projPos.z;
				float fade = saturate(_InvFade * (sceneZ - partZ));
				IN.color.a *= fade;
			#endif
		
			half Fresnel = saturate(dot(normalize(IN.viewDir), normalize(o.Normal)));
			Fresnel = pow(Fresnel, _RimPower);

			half4 col = tex2D(_MainTex, IN.uv_MainTex);
			o.Emission = (col.rgb * col.a * Fresnel * _RimThreshold * IN.color.rgb * IN.color.a);
			o.Alpha = IN.color.a;
		}
		ENDCG
	} 
	FallBack "Transparent"
}