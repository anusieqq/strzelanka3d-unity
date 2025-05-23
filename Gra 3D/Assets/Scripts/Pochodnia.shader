Shader "Custom/TorchEffect"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {} 
        _Color ("Base Color", Color) = (1, 0.5, 0.2, 1)
        _Intensity ("Intensity", Range(0, 10)) = 5.0
        _NoiseScale ("Noise Scale", Range(0.1, 5.0)) = 1.0
        _TimeSpeed ("Time Speed", Range(0.1, 5.0)) = 1.0
        _CustomTime ("Custom Time", Float) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            Tags { "LightMode" = "ForwardBase" }
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;         
            float4 _Color;
            float _Intensity;
            float _NoiseScale;
            float _TimeSpeed;
            float _CustomTime;

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

            float rand(float2 co)
            {
                return frac(sin(dot(co, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = rand(i);
                float b = rand(i + float2(1, 0));
                float c = rand(i + float2(0, 1));
                float d = rand(i + float2(1, 1));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            v2f vert(appdata v)
            {
                v2f o;
                float time = _CustomTime * _TimeSpeed;
                v.vertex.y += sin(v.vertex.x * 10.0 + time * 3.0) * 0.05;
                v.vertex.x += sin(v.vertex.y * 5.0 + time * 2.0) * 0.02;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv * _NoiseScale;
                uv.y += _CustomTime * _TimeSpeed;
                uv.x += sin(_CustomTime * _TimeSpeed) * 0.1;

                float n1 = noise(uv);
                float n2 = noise(uv * 2.0 + 10.0);
                float n3 = noise(uv * 4.0 + 20.0);
                float n = (n1 + n2 * 0.5 + n3 * 0.25) / 1.75;

                float flicker = 0.6 + 0.4 * noise(uv + float2(_CustomTime * 2.5, 0.0));
                flicker *= 0.8 + 0.2 * sin(_CustomTime * 12.0 + uv.y * 10.0);

                fixed4 texColor = tex2D(_MainTex, i.uv); 

                fixed4 col = _Color * texColor;
                col.rgb *= flicker * _Intensity;

                col.r += 0.05 * sin(_CustomTime * 8.0 + uv.x * 5.0);
                col.g -= 0.03 * sin(_CustomTime * 6.0 + uv.y * 3.0);

                col.rgb = clamp(col.rgb, 0.0, 1.0);

                // Zachowanie przezroczystoœci z tekstury (wa¿ne!)
                col.a *= texColor.a;

                return col;
            }
            ENDCG
        }
    }

    Fallback "Transparent/Diffuse"
}
