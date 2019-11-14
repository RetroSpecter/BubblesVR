Shader "Custom/RefractionShader"
{
	Properties
	{
		_Offset("Offset", Vector) = (0,0,0,0)
		_Tint("Tint", Color) = (.34, .85, .92, 1)
		_Smoothness("Smoothness", Range(0, 1)) = 0.5
		_Fresnel("Fresnel Coefficient", float) = 5.0
		_FadeOutDistance("Fade Distance", float) = 1.0
		_Ambient("Ambient Color", Color) = (.34, .85, .92, 1)
    }
    SubShader
    {
		Tags { "RenderType" = "Transparent" 
		"Queue" = "Transparent"
		}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
			#include "UnityStandardBRDF.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float4 screenUV : TEXCOORD1;
				float3 normal : NORMAL;
				float3 viewT : TEXCOORD2;
				float3 worldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _CameraOpaqueTexture;
			float4 _Offset;
			float _FadeOutDistance;
			float _Smoothness;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
				o.screenUV = ComputeGrabScreenPos(o.vertex);
				o.normal = v.normal;
				o.viewT = normalize(WorldSpaceViewDir(v.vertex));
				o.worldPos = mul(unity_ObjectToWorld,  fixed4(0,0,0,1));
                return o;
            }

			float4 _Tint;
			half _Fresnel;
			float4 _Ambient;

            fixed4 frag (v2f i) : SV_Target
            {	
				#if UNITY_SINGLE_PASS_STEREO
				i.screenUV.xy = TransformStereoScreenSpaceTex(i.screenUV.xy, i.screenUV.w);
				#endif
				i.normal = normalize(i.normal);

			
				// distance
				float camDist = abs(distance(i.worldPos, _WorldSpaceCameraPos));
				float distLerp = lerp(0, 1, (camDist / _FadeOutDistance) - 1);
				distLerp = min(distLerp, 1);
				distLerp = max(distLerp, 0);

				// refraction
				half3 n = normalize(i.normal);
				half3 v = normalize(i.viewT);
				half fr = pow(1.0f - dot(v, n), _Fresnel) * distLerp;
				half reverse_fr = 1 - fr;
				sampler2D screenTex = _CameraOpaqueTexture;
				//return lerp(fixed4(1,0,0,1),fixed4(0,1,0,1),distLerp);

				// BlinnPhong Lighting
				float3 lightDir = _WorldSpaceLightPos0.xyz;
				float3 lightColor = _LightColor0.rgb;
				float3 diffuse = lightColor * DotClamped(lightDir, i.normal) + _Ambient;
				diffuse = lerp(float4(1, 1, 1, 1), diffuse, distLerp);

				float3 reflectionDir = reflect(-lightDir, i.normal);
				float4 specular = pow(DotClamped(i.viewT, reflectionDir), _Smoothness * 100);
				specular = lerp(float4(0, 0, 0, 1), specular, distLerp);
				float4 tint = lerp(float4(0,0,0,1), _Tint,distLerp);

                UNITY_APPLY_FOG(i.fogCoord, col);
				fixed4 grab = tex2Dproj(screenTex, i.screenUV + fr) * float4(diffuse, 1);
				return grab + specular + tint;			
            }

            ENDCG
        }
    }
}
