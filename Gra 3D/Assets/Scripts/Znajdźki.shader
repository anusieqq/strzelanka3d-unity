Shader "Custom/Znajdźki_Emisja"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionStrength ("Emission Strength", Float) = 1.0
        _PulseSpeed ("Pulse Speed", Float) = 3.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        fixed4 _EmissionColor;
        float _EmissionStrength;
        float _PulseSpeed;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            // --- PULSUJĄCA EMISJA ---
            float pulse = sin(_Time.y * _PulseSpeed) * 0.5 + 0.5; // wynik 0..1
            o.Emission = _EmissionColor.rgb * (_EmissionStrength * pulse);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
