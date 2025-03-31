Shader "Custom/BulletHoleShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BulletHoleMask ("Bullet Hole Mask", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Standard

        sampler2D _MainTex;
        sampler2D _BulletHoleMask;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 col = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 mask = tex2D(_BulletHoleMask, IN.uv_MainTex);

            // Jeœli na masce jest dziura, zmniejsz jasnoœæ
            col.rgb *= (1.0 - mask.r * 0.7); 

            o.Albedo = col.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
