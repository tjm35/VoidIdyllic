Shader "Unlit/POIVisibilityDebug"
{
    Properties
    {
        _POIAnalysis ("POIAnalysis", 2D) = "white" {}
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            Texture2D <int> _POIAnalysis;
			SamplerState sampler_POIAnalysis;
            float4 _POIAnalysis_TexelSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv; //TRANSFORM_TEX(v.uv, _POIAnalysis);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                int id = _POIAnalysis.Load(int3(i.uv * _POIAnalysis_TexelSize.zw, 0));

				int colorType = (id % 7) + 1;
				float r = ((colorType & 1) == 1) ? 1.0f : 0.0f;
				float g = ((colorType & 2) == 2) ? 1.0f : 0.0f;
				float b = ((colorType & 4) == 4) ? 1.0f : 0.0f;

                return id == 0 ? fixed4(0,0,0,0) : fixed4(r,g,b,1);
            }
            ENDCG
        }
    }
}
