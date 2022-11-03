Shader "Custom/Glitchy Footage Shader"{
	Properties{
		_MainTex("Main Texture", 2D) = "white" {}
		_GlitchColor("Glitch Color", Color) = (1.0,1.0,1.0,1.0)
		_NoiseColor("Colored Noise Color", Color) = (1.0,1.0,1.0,1.0)
		_WaveColor("Wave Noise Color", Color) = (1.0,1.0,1.0,1.0)
		_GlitchInterval("Glitch Interval", Float) = 5.0	
		_GlitchRate("Glitch Rate", Range(0,1)) = 0.9
		_ResHorizontal("Horizontal Resolution", Float) = 640
		_ResVertical("Vertical Resolution", Float) = 480
		_ColorNoiseIntensity("Color Noise Intensity", Float) = 1.0
		_WaveNoiseIntensity("Wave Noise Intensity", Float) = 1.0
		_RGBShiftIntensity("RGB Shift Intensity", Float) = 1.0
		_ShakeGlitchEnabled("Shake Glitch", Float) = 1.0
		_ScanlineGlitchEnabled("Scanline Glitch", Float) = 1.0
		_UnscaledTime("Unscaled Time", Float) = 0
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
			uniform fixed _ColorNoiseIntensity;
			uniform fixed _WaveNoiseIntensity;
			uniform fixed4 _GlitchColor;
			uniform fixed4 _NoiseColor;
			uniform fixed4 _WaveColor;
			uniform float _ScanlineGlitchEnabled;
			uniform float _ShakeGlitchEnabled;
			uniform fixed _RGBShiftIntensity;
			uniform float _UnscaledTime;

			float random(float2 c) {
				return frac(sin(dot(c.xy, float2(12.9898, 78.233))) * 43758.5453);
			}
			float mod(float x, float y)
			{
				return x - y * floor(x / y);
			}
			

			fixed4 frag(v2f i) : SV_Target 
			{
				fixed strength = 0.;
				fixed2 shake = fixed2(0., 0.);
				if (_ShakeGlitchEnabled == 1.) {
					 strength = smoothstep(_GlitchInterval * _GlitchRate, _GlitchInterval, _GlitchInterval - mod(1 / _UnscaledTime, _GlitchInterval));
					 shake = fixed2(strength * 8.0 + 0.5, strength * 8.0 + 0.5)* fixed2(random(fixed2(_UnscaledTime, 1 / _UnscaledTime)) * 1.5 - 1.0, random(fixed2(1 / _UnscaledTime * 1.5, 1 / _UnscaledTime * 2.0)) * 1.5 - 1.0) / fixed2(_ResHorizontal, _ResVertical);
				}


				fixed y = i.uv.y * _ResVertical;
				fixed rgbWave = 0.;
				if (_ScanlineGlitchEnabled == 1.) {
					rgbWave = (
						snoise(float2( y * 0.01, 1 / _UnscaledTime * 400.0)) * (2.0 + strength * 32.0) //Time.x was just _Time in original
						* snoise(float2( y * 0.02, 1 / _UnscaledTime * 200.0)) * (1.0 + strength * 4.0) //Time.x was just _Time in original
						+ step(0.9995, sin(y * 0.005 + 1 / _UnscaledTime * 1.6)) * 12.0
						+ step(0.9999, sin(y * 0.005 + 1 / _UnscaledTime * 2.0)) * -18.0
						) / _ResHorizontal;
				}
				fixed rgbDiff = (6.0 + sin(1 / _UnscaledTime * 500.0 + i.uv.y * 40.0) * (20.0 * strength + 1.0)) / _ResHorizontal;
				rgbDiff = rgbDiff * _RGBShiftIntensity;
				fixed rgbUvX = i.uv.x + rgbWave;
				
				fixed g = tex2D(_MainTex, float2(rgbUvX, i.uv.y) + shake).g;
				fixed2 rb = tex2D(_MainTex, float2(rgbUvX + rgbDiff, i.uv.y) + shake).rb;//r and b channels get shifted by rgbDiff amount
				
				fixed colorNoise = (random(i.uv + mod(1 / _UnscaledTime, 10.0)) * 2.0 - 1.0) * (0.15 + strength * 0.15);

				fixed waveNoise = (sin(i.uv.y * 1200.0) + 1.0) / 2.0 * (0.15 + strength * 0.2);

				half4 ret = fixed4(rb.x, g, rb.y, 1.0) *
					(1.0 * _GlitchColor) +
					(colorNoise * _ColorNoiseIntensity) * _NoiseColor - (waveNoise * _WaveNoiseIntensity) * _WaveColor;
				return ret;
			}
			ENDCG
		}     
	}
}