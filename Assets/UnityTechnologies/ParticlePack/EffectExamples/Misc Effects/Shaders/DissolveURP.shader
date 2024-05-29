Shader "Custom/DissolveURP"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}
        _NoiseTex("Noise", 2D) = "white" {}
        _Cutoff("Cutoff", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _Color;
            float _Cutoff;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.position = TransformObjectToHClip(input.position.xyz);
                output.uv = input.uv;
                output.worldPos = TransformObjectToWorld(input.position.xyz);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 albedo = tex2D(_MainTex, input.uv) * _Color;
                half noise = tex2D(_NoiseTex, input.uv).r;

                if (noise < _Cutoff)
                    discard;

                return albedo;
            }
            ENDHLSL
        }
    }
}
