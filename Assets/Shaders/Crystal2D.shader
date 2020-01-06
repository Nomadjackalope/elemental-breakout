Shader "Custom/Crystal2D"
{
    Properties
    {

        [PreRenderedData] _MainTex ("Sprite Texture", 2D) = "black" {}
        _Color ("Tint", Color) = (1,0,1,1)
        _PassthroughTex ("PassthroughTex", 2D) = "white" {}
        _Passthrough ("Passthrough", Range (0, 1)) = 0.5
        _RandomObjectId ("RandomObjectId", int) = 1
    }
    SubShader
    {
        Tags 
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
        LOD 100

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        GrabPass
        {
            "_BG"
        }

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenUV: TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenUV = ComputeGrabScreenPos(o.vertex);
                o.uv = v.uv;//TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            sampler2D _BG;
            sampler2D _AlphaTex;
            float _AlphaSplitEnabled;
            float4 _Color;
            float _Passthrough;
            float _RandomObjectId;

            sampler2D _PassthroughTex;

            fixed4 SampleSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_MainTex, uv);

                // #if UNITY_TEXTURE_ALPHASPLIT_ALLOWED
                // if (_AlphaSplitEnabled)
                //     color.a = tex2D (_AlphaTex, uv).r;
                // #endif

                return color;
            }

            float rand(float2 co) {
                float x = sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453;
                return x - floor(x);
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                // i.grabPos = float4(i.grabPos.xyz, i.grabPos.w + i.color.x);
                // half4 bgcolor = tex2Dproj(_BG, i.grabPos);
                // bgcolor = bgcolor * _Color;

                float4 centercoord = UNITY_PROJ_COORD(i.screenUV);
                
                //_Passthrough hex is a normal map where x-axis is green and y-axis is red
                float4 pasCol = tex2D(_PassthroughTex, i.uv); //SampleSpriteTexture(_PassthroughTex, i.uv);//
                pasCol.rgb *= pasCol.a;
                //pasCol = pasCol * 1.66 - 1.15;// pasCol.y - 0.0 * 10; // saturate((abs(i.color.x)) * 1000 + 0.1);
                pasCol = pasCol * 2.0 - 1.0;
                float past = saturate(abs(pasCol.x) + abs(pasCol.y));

                //float3 baseWorldPos = mul ( unity_ObjectToWorld, float4(0,0,0,1.0) ).xyz;
                pasCol *= sin(_Time.y / 4 + _RandomObjectId);

                // Get an adjusted position for grabpass
                centercoord = centercoord * float4(0.8, 0.8, 1, 1) + float4(0.3 * pasCol.x, 0.3 * pasCol.y, 0, 0);
                //i.screenUV = float4(i.screenUV.xyz, i.screenUV.w + pasCol.x - pasCol.y);

                fixed4 col = SampleSpriteTexture(i.uv);// * _Color; // tex2D(_MainTex, i.uv); //
                col.rgb *= col.a;
                fixed4 grab = tex2Dproj(_BG, centercoord);


                // make 0 = 0 and  anything > 0.001 = 1
                // Make passthrough apply to only areas where normal is not 0
                // _Passthrough *= (1 - abs(pasCol.x));


                past *= _Passthrough;

                fixed4 returnable = (1 - past) * col + saturate(past + 1) * grab;
                returnable.rgb *= col.a;
                returnable.a = col.a;
                return returnable;

                // return (grab * (1 - _Passthrough + 0.5) + col * (_Passthrough + 0.1)) * _Color;
            }
            ENDCG
        }
    }
}
