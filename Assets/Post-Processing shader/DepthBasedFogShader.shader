Shader "Hidden/DepthBasedFogShader"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 interpolatedRay : TEXCOORD1;
            };
            
            sampler2D _MainTex;
            sampler2D _CameraDepthTexture;
            
            float4x4 _FrustumCornersWS;
            float3 _CameraWS;
            float4 _FogColor;
            float _FogDensity;
            float _FogStartHeight;
            float _FogHeightFalloff;
            float _Linear01Depth;
            float _LinearEyeDepth;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                
                int index = 0;
                if (v.uv.x < 0.5 && v.uv.y < 0.5) index = 0;
                else if (v.uv.x > 0.5 && v.uv.y < 0.5) index = 1;
                else if (v.uv.x > 0.5 && v.uv.y > 0.5) index = 2;
                else index = 3;
                
                o.interpolatedRay = _FrustumCornersWS[index];
                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
                depth = Linear01Depth(depth);
                
                float4 worldPos = float4(_CameraWS, 1.0) + depth * i.interpolatedRay;
                
                float distance = length(worldPos.xyz - _CameraWS);
                
                float heightFactor = saturate((worldPos.y - _FogStartHeight) * _FogHeightFalloff);
                float fogDensity = _FogDensity * exp(-heightFactor);
                
                float fogFactor = exp(-distance * fogDensity);
                fogFactor = saturate(1.0 - fogFactor);
                
                half4 col = tex2D(_MainTex, i.uv);
                
                return lerp(col, _FogColor, fogFactor);
            }
            ENDCG
        }
    }
    
    FallBack Off
}
