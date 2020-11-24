Shader "Unlit/RuinUI_MSDF"
{
    Properties
    {
        _Image ("_Image", 2D) = "white" {}
		[HDR] _ActiveColor("_ActiveColor", Color) = (1,1,1,1)
		[HDR] _InactiveColor("_InactiveColor", Color) = (1,1,1,0.1)
		_IsActive("_IsActive", Int) = 1
		_PixelRange("_PixelRange", Float) = 2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
		ZWrite Off
		Blend SrcAlpha One
		Cull back

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

            sampler2D _Image;
            float4 _Image_TexelSize;
			float4 _ActiveColor;
			float4 _InactiveColor;
			int _IsActive;
			float _PixelRange;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			float median(float x, float y, float z)
			{
				return max(min(x,y), min(max(x,y), z));
			}

            fixed4 frag (v2f i) : SV_Target
            {
				float2 units = _PixelRange * _Image_TexelSize.xy;

				float3 sampleValue = tex2D(_Image, i.uv);

				float medianSample = median(sampleValue.r, sampleValue.g, sampleValue.b);
				float signedDistance = (medianSample - 0.5) * dot(units, 0.5/fwidth(i.uv));

				float value = saturate(signedDistance + 0.5);

				float4 col = (_IsActive == 1 ? _ActiveColor : _InactiveColor);
				col.a *= value;

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
