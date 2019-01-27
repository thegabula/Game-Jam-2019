Shader "Unlit/Outline"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OutlineColor("Outline color", Color) = (0,0,0,1)
		_OutlineWidth("Outline width", Range(1.0,5.0)) = 1.01
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
	}

	SubShader
	{
		Tags { "Queue" = "Transparent+1" "RenderType"="Transparent" }
		LOD 100
		Pass //Outline
		{
			ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc" // for UnityObjectToWorldNormal
            #include "UnityLightingCommon.cginc" // for _LightColor0
			
			struct appdata
			{
				float4 vertex: POSITION;
				float3 normal: NORMAL;
			};

			struct v2f
			{
				float4 pos : POSITION;					
				float3 normal: NORMAL;							
			};
			
			float _OutlineWidth;
			float4 _OutlineColor;

			

			v2f vert (appdata v)
			{
				v.vertex.xyz *= _OutlineWidth;

				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) :COLOR
			{
				return _OutlineColor;
			}
			ENDCG
			
		} 

		Pass //Normal
		{
			ZWrite On

			Material
			{
				Diffuse[_Color]
				Ambient[_Color]
			}

			Lighting On

			SetTexture[_MainTex]
			{
				ConstantColor[_Color]
			}

			SetTexture[_MainTex]
			{
				Combine previous * primary DOUBLE
			}
		} 
		
			
	}
}
