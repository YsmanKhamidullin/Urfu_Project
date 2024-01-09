Shader "UI/BlurStep"
{
    Properties
    {
        [NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
        _Offset("Offset", Range(0, 0.01)) = 0.001
    }
        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            LOD 100
            ZTest Always

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float _Offset;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col;

                    col  = tex2D(_MainTex, i.uv + float2( 0.000,  1.000) * _Offset);
                    col += tex2D(_MainTex, i.uv + float2(+0.951, +0.309) * _Offset);
                    col += tex2D(_MainTex, i.uv + float2(+0.588, -0.809) * _Offset);
                    col += tex2D(_MainTex, i.uv + float2(-0.588, -0.809) * _Offset);
                    col += tex2D(_MainTex, i.uv + float2(-0.951, +0.309) * _Offset);

                    return col * 0.199;
                }
                ENDCG
            }
        }
}
