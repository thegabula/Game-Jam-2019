// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Lit/MyTriPlanar"
{
	Properties
	{
		[NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
		_Tiling ("Tiling", Float)= 1.0
		_OcclusionMap("Occlusion", 2D) = "White" {}
		
		
	}
	SubShader
	{
		//Tags { "RenderType"="Opaque"}
		Tags { "LightMode"="ForwardBase"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" // for UnityObjectToWorldNormal
            #include "UnityLightingCommon.cginc" // for _LightColor0
			
			struct v2f
			{
				half3 objNormal : TEXCOORD0;
				float3 coords : TEXCOORD1;
				float2 uv : TEXCOORD2;
				float4 vertex : SV_POSITION;	
				fixed4 diff : COLOR0;
							
			};
			
			float _Tiling;
			
			v2f vert (float4 pos : POSITION, float3 normal : NORMAL, float2 uv : TEXCOORD0)
			//v2f vert (appdata_base v)
			{
				v2f o;
				//o.pos = mul(UNITY_MATRIX_MVP, pos);
				o.vertex = UnityObjectToClipPos(pos);
				o.coords = pos.xyz * _Tiling;
								
				// get vertex normal in world space
			//	half3 worldNormal = UnityObjectToWorldNormal(normal);
				// dot product between normal and light direction for standard diffuse lighting
				//half n1 = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				
				o.objNormal = normal;
				o.uv = uv;//v.texcoord;
				// factor in the light colour
			//	o.diff = normal * _LightColor0;				
				return o;
			}

			sampler2D _MainTex;
			sampler2D _OcclusionMap;
			
			fixed4 frag (v2f i) :SV_Target
			{
				// use absolute value of normal as texture weights
				half3 blend = abs(i.objNormal);
				// make sure the weights sum up to 1 (divide by sum of x+y+z)
				blend /= dot(blend,1.0);
				// read the three texture projections, for x,y,z axes
				fixed4 cx = tex2D(_MainTex, i.coords.yz);
				fixed4 cy = tex2D(_MainTex, i.coords.xz);
				fixed4 cz = tex2D(_MainTex, i.coords.xy);
				// blend the textures based on weights
				fixed4 c = cx * blend.x + cy * blend.y + cz * blend.z;
				// modulate by regular occlusion map
				c *= tex2D(_OcclusionMap, i.uv);
				return c;				
			}
			ENDCG
		}
	}
}
