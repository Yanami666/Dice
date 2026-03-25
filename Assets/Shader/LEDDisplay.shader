Shader "Custom/LEDDisplay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DotSize ("Dot Size", Range(0.1, 1.0)) = 0.8
        _GridSize ("Grid Size", Float) = 8.0
        [HDR] _EmissionColor ("Emission Color", Color) = (1, 0.4, 0, 1)
        _BgColor ("Background Color", Color) = (0.05, 0.02, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _DotSize;
            float _GridSize;
            float4 _EmissionColor;
            float4 _BgColor;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 grid = frac(i.uv * _GridSize);
                float2 center = grid - 0.5;
                float dist = length(center);
                float circle = step(dist, _DotSize * 0.5);
                float2 snappedUV = floor(i.uv * _GridSize) / _GridSize + 0.5 / _GridSize;
                fixed4 texColor = tex2D(_MainTex, snappedUV);
                float brightness = dot(texColor.rgb, float3(0.299, 0.587, 0.114));
                fixed4 col = lerp(_BgColor, _EmissionColor * texColor, circle * brightness);
                col.a = 1.0;
                return col;
            }
            ENDCG
        }
    }
}