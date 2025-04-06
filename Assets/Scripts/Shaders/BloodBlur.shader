Shader "Custom/UI/VignetteBlurURP"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurAmount ("Blur Amount", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            sampler2D _MainTex;
            float _BlurAmount;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionHCS = TransformObjectToHClip(v.positionOS);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                float2 uv = i.uv;
                float blur = _BlurAmount * 0.01;

                // Простое размытие
                half4 color = tex2D(_MainTex, uv);
                color += tex2D(_MainTex, uv + float2(blur, 0));
                color += tex2D(_MainTex, uv - float2(blur, 0));
                color += tex2D(_MainTex, uv + float2(0, blur));
                color += tex2D(_MainTex, uv - float2(0, blur));
                color /= 5;

                return color;
            }
            ENDHLSL
        }
    }
}