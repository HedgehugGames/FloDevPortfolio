Shader "hda/Water"
{
    Properties
    {
        _Color ("Color", Color) = (0,0,0.5,1)
        _Power("Power", float)  = 1
        _WaveSpeed("Wave Speed", float) = 1
        _WaveHeight("Wave Height", float) = 0.1
        _WaveFrequency ("Wave Frequency", float) = 1
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" }

      Pass
      {
        Zwrite Off
        Blend SrcAlpha OneMinusSrcAlpha
      
        CGPROGRAM

        #pragma vertex Vert
        #pragma fragment Pix

        uniform float4 _Color;
        uniform float _Power;
        uniform float _WaveSpeed = 1;
        uniform float _WaveHeight = 0.1;
        uniform float _WaveFrequency = 1;

      
        struct vertexData
        {
          float4 pos: POSITION;
          float3 normal : NORMAL;
        };

        struct vertexOut
        {
          float4 pos: POSITION;
          float3 normal : TEXCOORD0;
          float3 viewDir : TEXCOORD1;
        };

        vertexOut Vert(vertexData input)
        {
          vertexOut output;

          //unity:ObjectTOWorld
          //unity_WorldToOject  <-- INVERSE World
          
          output.normal = normalize(mul(float4(input.normal,1), unity_WorldToObject).xyz);
          output.viewDir = normalize(_WorldSpaceCameraPos - mul(unity_ObjectToWorld, input.pos).xyz);

          // vertex position manipulation
          float3 worldPos = mul(unity_ObjectToWorld, input.pos).xyz;
          float wave = sin(worldPos.x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveHeight;
          float4 newPos = input.pos;
          newPos.y += wave;

          output.pos = UnityObjectToClipPos(newPos);
          
          return output;
         }

        float4 Pix(vertexOut input) : COLOR
        {
          float3 normalDir = normalize(input.normal);
          float3 viewDir = normalize(input.viewDir);

          float newAlpha = min (1, _Color.a / abs(pow(dot(viewDir, normalDir), _Power)));  // abs() -> makes floats always positive
          
          return float4(_Color.rgb, newAlpha);
        }

        ENDCG
      }
   }
}
