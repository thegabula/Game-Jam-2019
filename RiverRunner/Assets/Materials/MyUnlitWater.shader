Shader "Unlit/MyUnlitWater"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,1)
		//_Transparency ("Transparency",Range(0.0, 1.0)) = 0.25 
		_Distance("Distance", Float) = 1
		_Amplitude("Amplitude", Float)= 1
		_Speed("Speed", Float) =1 
		_Amount("Amount", Range(0.0,1.0)) = 1 
		

	}
	SubShader
	{
		Tags {"RenderType"="Opaque" }
		LOD 100
	//	ZWrite Off
	//	Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			 #include "UnityLightingCommon.cginc" // for _LightColor0

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				half3 objNormal : TEXCOORD0;
				float3 coords : TEXCOORD1;
				float2 uv : TEXCOORD2;
				float4 vertex : SV_POSITION;	
				fixed4 diff : COLOR0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			//float _Transparency;
			float4 _Tint;
			
			float _Distance;
			float _Amplitude;
			float _Speed;
			float _Amount;
			
			v2f vert (appdata_base v)
			{
				v2f o;
				v.vertex.y += sin(_Time.y * _Speed+ v.vertex.x * _Amplitude) * _Distance * _Amount;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);

				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				half n1 = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				
				o.objNormal = v.normal;				
				// factor in the light colour
				o.diff = n1 * _LightColor0;	
				o.diff.rgb += ShadeSH9(half4(worldNormal,1));
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) ;//+ _Tint;
				//col.a = _Transparency;
				
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				col *= i.diff;
				return col;
			}
			ENDCG
		}
	}
}
