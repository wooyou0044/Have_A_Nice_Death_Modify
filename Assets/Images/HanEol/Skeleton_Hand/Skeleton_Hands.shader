Shader "Custom/Skeleton_Hands" // 쉐이더 이름
{
    Properties // 머티리얼에서 조작 가능한 값
    {
        _Color ("Main Color", Color) = (1, 0, 0, 1) // 빨간색 기본값
        _MainTex("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata_t
            {
                float4 vertex : POSITION; // 정점 위치
            };

            struct v2f
            {
                float4 pos : SV_POSITION; // 출력 위치
            };

            float4 _Color; // 색상 속성

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); // 위치 변환
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color; // 단색 출력
            }
            ENDCG
        }
    }
}
