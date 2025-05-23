Shader "Custom/BulletHoleShader"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BulletHoleMask ("Bullet Hole Mask", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="AlphaTest" } // Changed RenderType and added Queue
        
        // This makes sure the backfaces are also rendered for the hole
        Cull Off 

        CGPROGRAM
        #pragma surface surf Standard alpha:blend // Added alpha:blend for transparency

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

            // If on the mask there's a hole (mask.r is high), make it transparent
            // The 1.0 - mask.r will make the alpha 0 where the mask is 1 (hole)
            // and 1 where the mask is 0 (no hole).
            o.Albedo = col.rgb;
            o.Alpha = 1.0 - mask.r; // Use the mask to control transparency
        }
        ENDCG
    }
    FallBack "Diffuse"
}