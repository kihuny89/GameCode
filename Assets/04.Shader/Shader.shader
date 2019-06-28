Shader "Custom/Shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		[NoScaleOffset]_PBR("Metallic(R), Smoothness(a)", 2D) = "black"{}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
		[Normal]_Normal("Normal", 2D) ="bump"{}
		_Emission("Emission", 2D) ="white"{}
		[HDR]_EmissionColor("EmissionColor", Color) =(0,0,0,0)

		[Header(RimLight)]
		_RimColor("RimColor", Color) = (1,1,1,1)
		_RimColorPow("RimColorPow" , Range(0,100))=1
		_RimMul("RimMul", Range(0,10))=1
		[PowerSlider]_RimPow("RimPow" ,Range(0,100))=1

		[Header(Dissolve)]		
		[Toggle]_Cutoff("CutOff", Range(0,1)) = 0
		_NoiseTex("NoiseTex", 2D) = "white" {}
		[HDR]_DissolveColor("DissolveColor" , Color) = (1,1,1,1)
		_DissolveColorPow("DissloveColorPow" , Range(0,100))= 1 
		_AlphaTest("AlphaTest" , Range(-1,1)) =-1
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCoutout" "Queue" = "AlphaTest" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alphatest:_Cutoff addshadow

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 2.0

        sampler2D _MainTex;
		sampler2D _PBR;
		sampler2D _Normal;
		sampler2D _Emission;
		sampler2D _NoiseTex;
		


        struct Input
        {
            fixed2 uv_MainTex;
			fixed3 viewDir;
			fixed2 uv_NoiseTex;
        };

        fixed _Glossiness;
        fixed _Metallic;
        fixed4 _Color;
		fixed3 _EmissionColor;

		//RimLight
		fixed3 _RimColor;
		fixed _RimColorPow;
		fixed _RimMul;
		fixed _RimPow;

		//Dissolve
		fixed _AlphaTest;
		fixed3 _DissolveColor;
		fixed _DissolveColorPow;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
			//기본 PBR
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			fixed4 PBR = tex2D(_PBR, IN.uv_MainTex);
			fixed3 Normal = UnpackNormal(tex2D(_Normal, IN.uv_MainTex));
			fixed3 Emission = tex2D(_Emission, IN.uv_MainTex) *_EmissionColor;
			
			//노멀
			o.Normal = Normal;

			//내적
			fixed NdotV = dot(o.Normal, IN.viewDir);
			//림 라이트 처리
			fixed3 RimLight = saturate(pow(((1-saturate(NdotV)) * _RimMul), _RimPow)) * _RimColor * _RimColorPow;
			
			//Dissolve
			fixed DissolveMask = tex2D(_NoiseTex, IN.uv_NoiseTex);
			if (DissolveMask >= _AlphaTest)
			{
				DissolveMask = 1;
			}
			else
			{
				DissolveMask =0;
			}

			fixed DissolveMask2 = tex2D(_NoiseTex, IN.uv_NoiseTex);
			if (DissolveMask2 >= _AlphaTest + 0.03 )
			{
				DissolveMask2 = 1;
			}
			else
			{
				DissolveMask2 =0;
			}

			fixed3 FinalDissolve = (1 - DissolveMask2) * _DissolveColor * _DissolveColorPow;

			//최종 렌더링
            o.Albedo = c.rgb;
            o.Metallic = PBR.r* _Metallic;
            o.Smoothness = PBR.a * _Glossiness;
			o.Emission = Emission + RimLight +FinalDissolve;
            o.Alpha =  DissolveMask;
        }
        ENDCG
    }
    FallBack "Legacy Shaders/Transparent/Cutout/VertexLit"
}
