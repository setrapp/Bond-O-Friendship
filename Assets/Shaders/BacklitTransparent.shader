Shader "Backlit/Backlit Transparent" {
	Properties {
		_Color ("Main Color (RGB) Alpha (A)", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		_BackLitTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
    SubShader {
   		Tags{"Queue" = "Transparent" "RenderType" = "Transparent"}
        //Lighting Off
        //ZTest LEqual
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha
		
		// First light pass
        Pass 
		{		
			Tags{"LightMode" = "ForwardBase"}
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			uniform float4 _Color;
			uniform sampler2D _MainTex;
			uniform sampler2D _BackLitTex;
			
			struct vertexInput
			{
				float4 pos	: POSITION;
				float4 uv	: TEXCOORD0;
				float3 norm	: NORMAL;
			};
			
			struct vertexOutput
			{
				float4 pos 		: POSITION;
				float4 uv		: TEXCOORD0;
				float4 worldPos	: TEXCOORD1;
				float3 norm		: NORMAL;
			};
			
			vertexOutput vert(vertexInput input) : POSITION
			{
				vertexOutput output;
				
				output.uv = input.uv;
				output.pos = mul(UNITY_MATRIX_MVP, input.pos);
				output.worldPos = mul(_Object2World, input.pos);
								
				output.norm = normalize(mul(float4(input.norm, 0.0), _World2Object).xyz);
				
				return output;
			}
			
			float4 frag(vertexOutput input) : COLOR
			{
				float4 lightPos = _WorldSpaceLightPos0;
				
				float attenuation = 1.0;
				float3 lightDir = normalize(lightPos.xyz);
				
				if (lightPos.w != 0)
				{
					float3 toLight = lightPos.xyz - input.worldPos.xyz;
					float dist = length(toLight);
					lightDir = toLight / dist;
					attenuation = 1.0 / dist;
				}
				
				
				float normDotLight = dot(input.norm, lightDir.xyz);
			
			
				float4 texColor = tex2D(_MainTex, input.uv.xy);
				
				float4 backLitColor = tex2D(_BackLitTex, input.uv.xy);
				
				backLitColor += (float4(1, 1, 1, 1) - backLitColor) * (normDotLight > 0);
				
				float4 diffuseColor = float4(normDotLight * _Color.r, normDotLight * _Color.g, normDotLight * _Color.b, _Color.a);
				float4 finalColor = float4(diffuseColor.r * texColor.r * backLitColor.r, diffuseColor.g * texColor.g * backLitColor.g, diffuseColor.b * texColor.b * backLitColor.b, diffuseColor.a * texColor.a * backLitColor.a);
				
				return finalColor;
			}
			
			ENDCG
		}
		
		// All later light pass
        Pass 
		{		
			Tags{"LightMode" = "ForwardAdd"}
			Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			uniform float4 _Color;
			uniform sampler2D _MainTex;
			uniform sampler2D _BackLitTex;
			
			struct vertexInput
			{
				float4 pos	: POSITION;
				float4 uv	: TEXCOORD0;
				float3 norm	: NORMAL;
			};
			
			struct vertexOutput
			{
				float4 pos 		: POSITION;
				float4 uv		: TEXCOORD0;
				float4 worldPos	: TEXCOORD1;
				float3 norm		: NORMAL;
			};
			
			vertexOutput vert(vertexInput input) : POSITION
			{
				vertexOutput output;
				
				output.uv = input.uv;
				output.pos = mul(UNITY_MATRIX_MVP, input.pos);
				output.worldPos = mul(_Object2World, input.pos);
								
				output.norm = normalize(mul(float4(input.norm, 0.0), _World2Object).xyz);
				
				return output;
			}
			
			float4 frag(vertexOutput input) : COLOR
			{
				float4 lightPos = _WorldSpaceLightPos0;
				
				float attenuation = 1.0;
				float3 lightDir = normalize(lightPos.xyz);
				
				if (lightPos.w != 0)
				{
					float3 toLight = lightPos.xyz - input.worldPos.xyz;
					float dist = length(toLight);
					lightDir = toLight / dist;
					attenuation = 1.0 / dist;
				}
				
				
				float normDotLight = dot(input.norm, lightDir.xyz);
				
				float4 backLitColor = tex2D(_BackLitTex, input.uv.xy);
				
				//backLitColor += (float4(1, 1, 1, 1) - backLitColor) * (normDotLight > 0);
				if (normDotLight > 0)
				{
					backLitColor = float4(1, 1, 1, 1);
				}
				else
				{
					normDotLight = -normDotLight;
				}
				
				float4 diffuseColor = float4(normDotLight * _Color.r, normDotLight * _Color.g, normDotLight * _Color.b, _Color.a);
				float4 finalColor = float4(diffuseColor.r * backLitColor.r, diffuseColor.g * backLitColor.g, diffuseColor.b * backLitColor.b, diffuseColor.a * backLitColor.a);
				
				return finalColor;
			}
			
			ENDCG
		}
	}
	Fallback "Transparent/Diffuse"
}
