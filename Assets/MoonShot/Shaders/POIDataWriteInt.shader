Shader "Unlit/POIDataWriteInt"
{
    Properties
    {
        _WriteInt ("WriteInt", Int) = 0
		_Transparent ("Transparent", Int) = 0
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
                float4 vertex : SV_POSITION;
            };

            int _WriteInt;
			int _Transparent;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            int frag (v2f i) : SV_Target
            {
				if (_WriteInt == 0 && _Transparent != 0)
				{
					discard;
				}
                return _WriteInt;
            }
            ENDCG
        }
    }
}
