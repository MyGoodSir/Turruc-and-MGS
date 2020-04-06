Shader "Unlit/RayMarching"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog

			#include "UnityCG.cginc"


			#define USE_OBJ_COORDS true
			#define MAX_STEPS 250.0
			#define MAX_DIST 100.0
			#define EPSILON .001


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 ro : TEXCOORD1;
				float3 sPos : TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);

				
				o.ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1.0));
				o.sPos = v.vertex;
				

				//o.ro = _WorldSpaceCameraPos;
				//o.sPos = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}
			float sdfSphere(float3 pos, float radius) {

				return length(pos) - radius;
			}
			float sdfScene(float3 pos) {
				return sdfSphere(pos, 0.25);
			}
			float rayMarch(float3 ro, float3 rd) {
				float currDist = 0;

				for (int i = 0; i < MAX_STEPS; i++) {
					float dist = sdfScene(ro + rd * currDist);
					if (dist < EPSILON) {
						return currDist;
					}
					currDist += dist;
					if (currDist >= MAX_DIST) {
						return MAX_DIST;
					}
				}
				return MAX_DIST;
			}
			float3 getNormal(float3 pos) {
				float2 e = float2(EPSILON, 0.0);
				return normalize(sdfScene(pos) - float3(sdfScene(pos - e.xyy), sdfScene(pos - e.yxy), sdfScene(pos - e.yyx)));
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float3 ro = i.ro;
				float3 rd = normalize(i.sPos - ro);
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				float dist = rayMarch(ro, rd);
				if (dist > MAX_DIST) {
					col.rgb = float3(0.0, 0.0, 0.0);
				}
				float3 pos = ro + rd * dist;
				float3 n = getNormal(pos);
				col.rgb = n;

				return col;
			}
			ENDCG
		}
	}
}
