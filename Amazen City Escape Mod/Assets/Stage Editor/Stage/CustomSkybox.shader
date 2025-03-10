Shader "Custom/Skybox" {
	Properties {
		_TintColor("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("Particle Texture", 2D) = "white" {}
		_ScrollX("Scroll X", Range(-1, 1)) = 0
		_ScrollY("Scroll Y", Range(-1, 1)) = 0
		_Cull("Cull", Float) = 2
		[Toggle(USE_NOISE)] _UseNoise("Use Noise", Float) = 0
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_NoiseScrollX("Noise Scroll X", Range(-1, 1)) = 0
		_NoiseScrollY("Noise Scroll Y", Range(-1, 1)) = 0
		_NoiseInt("Noise Intensity", Range(0, 5)) = 1
		[PerRendererData] _Alpha("Alpha", Range(0, 1)) = 1
	}
	SubShader {
		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType"="Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
		Cull [_Cull]
		Lighting Off
		ZWrite Off
	
		Pass {  
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma shader_feature USE_NOISE
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			sampler2D _MainTex, _NoiseTex;
			fixed4 _TintColor;
			float4 _MainTex_ST, _NoiseTex_ST;
			half _ScrollX, _ScrollY, _NoiseScrollX, _NoiseScrollY, _NoiseInt, _Alpha;
			
			v2f vert(appdata_t v) {
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color * _TintColor;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target {
				fixed4 noise = tex2D(_NoiseTex, i.texcoord + fixed2(frac(_NoiseScrollX * _Time.y), frac(_NoiseScrollY * _Time.y)));

				fixed4 col;
				#ifdef USE_NOISE
					col = 2 * i.color * tex2D(_MainTex, i.texcoord + fixed2(frac(_ScrollX * _Time.y), frac(_ScrollY * _Time.y)) + float2(noise.r * noise.r, 0) * _NoiseInt);
				#else
					col = 2 * i.color * tex2D(_MainTex, i.texcoord + fixed2(frac(_ScrollX * _Time.y), frac(_ScrollY * _Time.y)));
				#endif
				col.a *= _Alpha;
				return col;
			}
			ENDCG
		}
	}
}