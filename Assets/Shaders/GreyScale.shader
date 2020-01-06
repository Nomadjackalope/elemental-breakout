// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//https://www.reddit.com/r/Unity2D/comments/3e3r3u/greyscale_sprites/
// see also https://answers.unity.com/questions/980924/ui-mask-with-shader.html

Shader "Custom/GreyScale" {
Properties
{
	[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
	_Color ("Tint", Color) = (1,1,1,1)
	[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	_GrayScale("Grayscale Percent Fill", Range(0.0, 1.0)) = 0.0

	[HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
	[HideInInspector]_Stencil ("Stencil ID", Float) = 0
	[HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
	[HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
	[HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255
	[HideInInspector]_ColorMask ("Color Mask", Float) = 15
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

	Cull Off
	Lighting Off
	ZWrite Off
	Blend One OneMinusSrcAlpha

	Pass
	{
	CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile _ PIXELSNAP_ON
		#pragma multi_compile _ GRAYSCALE_ON GRAYSCALE_OFF
		#include "UnityCG.cginc"
		
		struct appdata_t
		{
			float4 vertex   : POSITION;
			float4 color    : COLOR;
			float2 texcoord : TEXCOORD0;
			
		};

		struct v2f
		{
			float4 vertex   : SV_POSITION;
			fixed4 color    : COLOR;
			half2 texcoord  : TEXCOORD0;
		};
		
		fixed4 _Color;
		float _GrayScale;

		v2f vert(appdata_t IN)
		{
			v2f OUT;
			OUT.vertex = UnityObjectToClipPos(IN.vertex);
			OUT.texcoord = IN.texcoord;
			OUT.color = IN.color * _Color;
			#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
			#endif

			return OUT;
		}

		sampler2D _MainTex;

		fixed4 frag(v2f IN) : SV_Target
		{
			fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
			c.rgb *= c.a;
			if (IN.texcoord.y > _GrayScale) {
				fixed avg = (c.r + c.g + c.b)/3;
				c.rgb = avg;
			}
			return c;
		}
	ENDCG
	}
}
}