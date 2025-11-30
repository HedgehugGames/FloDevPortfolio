Shader "Custom/OutlineBehindObject"
{
    Properties
    {
        _OutlineColor("Outline Color", Color) = (1, 0, 0, 1 )
        _OutlineWidth("Outline Width", Range(0.001, 0.1)) = 0.03
        _MainTex("Base (RGB)", 2D) = "white" { }
    }
        SubShader
    {
        Tags { "Queue" = "Overlay" "RenderType" = "Opaque" }

        // The outline pass (renders the outline behind the object)
        Pass
        {
            Name "OUTLINE"
            Tags { "RenderType" = "Opaque" }
            ZWrite On
            ZTest LEqual
            Offset 10, 10 // Push the outline behind the object

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            // Properties
            uniform float _OutlineWidth;
            uniform float4 _OutlineColor;

            v2f vert(appdata v)
            {
                v2f o;
                // Displace vertices along their normals to create the outline
                o.pos = UnityObjectToClipPos(v.vertex);
                o.pos.xy += v.normal.xy * _OutlineWidth; // Apply outline width
                o.color = _OutlineColor;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return i.color; // Render the outline color
            }
            ENDCG
        }

        // Main object pass
       Pass
       {
           Name "MAIN"
           Cull Back
           ZWrite On
           ZTest LEqual

           CGPROGRAM
           #pragma vertex vert
           #pragma fragment fragMain
           #include "UnityCG.cginc"

           sampler2D _MainTex;

           struct appdata
           {
               float4 vertex : POSITION;
               float2 uv : TEXCOORD0;
           };

           struct v2f
           {
               float4 pos : SV_POSITION;
               float2 uv : TEXCOORD0;
           };

           v2f vert(appdata v)
           {
               v2f o;
               o.pos = UnityObjectToClipPos(v.vertex);
               o.uv = v.uv;
               return o;
           }

           fixed4 fragMain(v2f i) : SV_Target
           {
               return tex2D(_MainTex, i.uv);
           }
           ENDCG
       }
    }

        FallBack "Diffuse"
}
