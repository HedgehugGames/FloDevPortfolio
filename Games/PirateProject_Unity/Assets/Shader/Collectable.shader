Shader "hda/MultiTexture" // Multi Texture Shader with transparency
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "black" {}
        _SecTex("Texture Flicker", 2D) = "white" {}
        _Color("Color Filter", Color) = (1,1,1,1)
        _Cutoff ("Cutoff", Float) = 0.1
        _Glossiness ("Smoothness", Range(0,1)) = 0.5

    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            uniform sampler2D _MainTex;
            uniform sampler2D _SecTex;
            uniform float4 _MainTex_ST;
            uniform float4 _SecTex_ST;
            uniform float4 _Color;
            uniform float _Cutoff;
            uniform float _Glossiness;

            float4 _LightColor0;

            struct vertexInput
            {
                float4 vertex : POSITION;
                float3 normal: NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct vertexOutput
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD1;
                float3 worldNormal : TEXCOORD2;
                float2 uv : TEXCOORD0;
            };

            vertexOutput vert(vertexInput input)
            {
                vertexOutput output;

                // Position for rasterizer
                output.pos = UnityObjectToClipPos(input.vertex);
                // Pass world position to fragment
                output.worldPos = mul(unity_ObjectToWorld, input.vertex).xyz;
                // Convert normal to world space
                output.worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, input.normal));

                output.uv = input.uv;

                return output;
            }

            float4 frag(vertexOutput input) : COLOR
            {
                float3 normalDir = normalize(mul(float4(input.worldNormal, 1), unity_WorldToObject).xyz);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float3 viewDir = normalize(_WorldSpaceCameraPos - input.worldPos);

                float NdotL = max(0, dot(normalDir, lightDir));
                float3 diffRefl = _LightColor0.rgb * _Color.rgb * max(0, dot(normalDir, lightDir));

                float3 halfDir = normalize(lightDir + viewDir);
                float NdotH = max(0, dot(normalDir, halfDir));
                float3 specRefl = _LightColor0.rgb * pow(NdotH, _Glossiness * 128);
                
                float4 tex = tex2D(_MainTex, _MainTex_ST.xy * input.uv.xy + _MainTex_ST.zw);
                float4 flicker = _Color * tex2D(_SecTex, _SecTex_ST.xy * input.uv.xy + _SecTex_ST.zw);

                // Apply _Cutoff only to the flicker texture
                float alphaBlend = saturate((flicker.a - _Cutoff) * 10); // soft threshold
                flicker.rgb *= alphaBlend;

                float3 litColor = tex.rgb * diffRefl + flicker.rgb + specRefl; 
                return float4(litColor, 1.0);
            }
            ENDCG
        }

    }

}