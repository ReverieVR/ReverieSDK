Shader "FX/MirrorReflection" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		[HideInInspector] _ReflectionTex ("", 2DArray) = "white" {}
		[HideInInspector] _FlipY ("Flip Y", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata {
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 refl : TEXCOORD1;
				float4 pos : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float4 _MainTex_ST;

			v2f vert(appdata v) {
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.pos = UnityObjectToClipPos (v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.refl = ComputeNonStereoScreenPos(o.pos);
				return o;
			}

			sampler2D _MainTex;
			Texture2DArray _ReflectionTex;
			SamplerState sampler_ReflectionTex;
			float4 _ReflectionTex_TexelSize;
			float _FlipY;

			fixed4 frag(v2f i) : SV_Target {
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				float3 uvRefl = float3(i.refl.xy / i.refl.w, unity_StereoEyeIndex);
				//if (_ProjectionParams.x < 0) uvRefl.y = 1 - uvRefl.y;
				#if UNITY_UV_STARTS_AT_TOP
					//if (_ReflectionTex_TexelSize.y < 0) uvRefl.y = 1 - uvRefl.y;
				#endif
				if (_FlipY > 0.5f) uvRefl.y = 1 - uvRefl.y;
				fixed4 tex = tex2D(_MainTex, i.uv);
				fixed4 refl = _ReflectionTex.SampleLevel(sampler_ReflectionTex, uvRefl, 0);
				return tex * refl;
			}
			ENDCG
		}
	}
}
