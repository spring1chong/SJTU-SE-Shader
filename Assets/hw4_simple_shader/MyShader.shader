Shader "Unlit/MyShader"
{
    Properties
    {
        // Texture settings
        _MainTex ("Main Texture", 2D) = "white" {}
        // Blinn-Phong specific
        _SpecColor ("SpecColor", Color) = (1,1,1,1)
        _Shininess ("Shininess", float) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            
            #pragma vertex MyVertexProgram 
            #pragma fragment MyFragmentProgram

            #pragma shader_feature USE_SPECULAR
            #pragma multi_compile _ MODE_NORMAL_ONLY MODE_BLINN_PHONG

            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"

            // Shader Properties
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Specolor;
            float _Shininess;

            float ComputeSpecular(float3 normal, float3 halfVector, float shininess)
            {
                float specAngle = DotClamped(normal, halfVector);
                return pow(specAngle, shininess);
            }

            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float2 uv: TEXCOORD0;
            };

            struct FragmentData { 
                float4 position : SV_POSITION; 
                float2 uv : TEXCOORD0; 
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            }; 

            FragmentData MyVertexProgram (VertexData v) { 
                FragmentData i; 
                i.position = UnityObjectToClipPos(v.position);
                i.worldPos = mul(unity_ObjectToWorld, v.position);
                i.uv = TRANSFORM_TEX(v.uv, _MainTex);
                i.normal = UnityObjectToWorldNormal(v.normal);
                return i; 
            }

            float4 MyFragmentProgram (FragmentData i) : SV_TARGET {
                // return float4(1, 0, 0, 1);
                // return float4(i.normal, 1);
                // return tex2D(_MainTex, i.uv);

                float3 finalColor = float3(0, 0, 0);

                #if defined(MODE_NORMAL_ONLY)
                    finalColor = i.normal;
                    // finalColor = i.normal * 0.5 + 0.5;
                #endif

                #if defined(MODE_BLINN_PHONG)
                    float3 baseColor = tex2D(_MainTex, i.uv).rgb;
                    float3 lightColor = _LightColor0.rgb;

                    float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                    float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                    float3 halfVector = normalize(lightDir + viewDir);

                    float3 diffuse = lightColor * DotClamped(lightDir, i.normal) * baseColor;
                    fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb * baseColor;
                    float3 specular = (0.0, 0.0, 0.0);
                    #if defined(USE_SPECULAR)
                        specular = _SpecColor * lightColor * ComputeSpecular(i.normal, halfVector, _Shininess);
                    #endif
                    finalColor = ambient + diffuse + specular;
                #endif
                return float4(finalColor, 1);
            } 

            ENDCG
        }
    }
    FallBack "Standard"
    CustomEditor "CustomShaderGUI"
}
