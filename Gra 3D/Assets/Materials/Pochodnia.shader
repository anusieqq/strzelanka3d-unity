Shader "Custom/TorchEffect"
{
    Properties
    {
        _Color ("Base Color", Color) = (1, 0.5, 0.2, 1) // Pomarañczowy
        _Intensity ("Intensity", Range(0, 10)) = 5.0
        _NoiseScale ("Noise Scale", Range(0.1, 5.0)) = 1.0
        _TimeSpeed ("Time Speed", Range(0.1, 2.0)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform float _Intensity;
            uniform float _NoiseScale;
            uniform float _TimeSpeed;
            uniform float4 _Color;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0; // Dodane UV
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0; // Przekazane UV
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; // Przekazanie UV
                return o;
            }

            // Funkcja szumu (bez zmian)
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = rand(i);
                float b = rand(i + float2(1.0, 0.0));
                float c = rand(i + float2(0.0, 1.0));
                float d = rand(i + float2(1.0, 1.0));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Generowanie szumu na podstawie UV modelu (nie ekranu!)
                float n = noise(i.uv * _NoiseScale + _TimeSpeed * _Time.y);
                
                // Tylko pomarañczowy kolor z migotaniem
                fixed4 col = _Color;
                
                // Modyfikacja intensywnoœci (tylko wartoœæ, nie kolor)
                col.rgb *= _Intensity * (0.7 + 0.3 * n); // £agodniejsze migotanie
                
                // Zabezpieczenie przed przekroczeniem zakresu
                col.rgb = clamp(col.rgb, 0.0, 1.0);
                
                return col;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}