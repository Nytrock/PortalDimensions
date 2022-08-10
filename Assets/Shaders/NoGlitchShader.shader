Shader "Custom/Glitchy Footage Shader No Blend"{
	Properties{
		_MainTex("Main Texture", 2D) = "white" {}
		_GlitchColor("Glitch Color", Color) = (1.0,1.0,1.0,1.0)
		_GlitchInterval("Glitch Interval", Float) = 5.0	
		_GlitchRate("Glitch Rate", Range(0,1)) = 0.9
		_ResHorizontal("Horizontal Resolution", Float) = 640
		_ResVertical("Vertical Resolution", Float) = 480
		_WhiteNoiseIntensity("White Noise Intensity", Float) = 1.0	
		_WaveNoiseIntensity("Wave Noise Intensity", Float) = 1.0
		_RGBShiftIntensity("RGB Shift Intensity", Float) = 1.0
		_BlockGlitchEnabled("Block Glitch", Float) = 1.0
		_ShakeGlitchEnabled("Shake Glitch", Float) = 1.0
		_ScanlineGlitchEnabled("Scanline Glitch", Float) = 1.0
	}
	SubShader
	{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		BlendOp Max
		LOD 200

		Pass
		{
			CGPROGRAM
			#pragma vertex vert alpha
			#pragma fragment frag alpha
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc" 
			#include "noiseSimplex.cginc"
			#pragma target 3.0
			
			struct appdata {
				float4 vertex: POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 pos :SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = float2(v.texcoord.xy);
				return o;
			}

			uniform sampler2D _MainTex;
			uniform float _GlitchInterval;
			uniform float _GlitchRate;
			uniform float _ResHorizontal;
			uniform float _ResVertical;
			uniform fixed _WhiteNoiseIntensity;
			uniform fixed _WaveNoiseIntensity;
			uniform fixed4 _GlitchColor;
			uniform float _BlockGlitchEnabled;
			uniform float _ScanlineGlitchEnabled;
			uniform float _ShakeGlitchEnabled;
			uniform fixed _RGBShiftIntensity;

			float random(float2 c) {
				return frac(sin(dot(c.xy, float2(12.9898, 78.233))) * 43758.5453);
			}
			float mod(float x, float y)
			{
				return x - y * floor(x / y);
			}
			

			fixed4 frag(v2f i) : SV_Target 
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}     
	}
}