Shader "Custom/Skeleton_Hands" // ���̴� �̸�
{
    Properties // ��Ƽ���󿡼� ���� ������ ��
    {
        _Color ("Main Color", Color) = (1, 0, 0, 1) // ������ �⺻��
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
                float4 vertex : POSITION; // ���� ��ġ
            };

            struct v2f
            {
                float4 pos : SV_POSITION; // ��� ��ġ
            };

            float4 _Color; // ���� �Ӽ�

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex); // ��ġ ��ȯ
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color; // �ܻ� ���
            }
            ENDCG
        }
    }
}
