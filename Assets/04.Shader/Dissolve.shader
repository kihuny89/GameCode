Shader "Custom/Dissolve"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap ("Normalmap", 2D) = "bump" {}
		_NoiseTex ("Noisetex" , 2D) ="white" {}
		_Cut ("Alpha Cut" , Range(0,1))=0
		[HDR] _OutColor ("_OutColor", Color) =(1,1,1,1)
		_OutThinkness ("_OutThinkness", Range(0,1.5))= 1.15
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        

		zwrite on
		blend SrcAlpha OneminusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert keepalpha
		     
        sampler2D _MainTex;
		sampler2D _BumpMap;
        sampler2D _NoiseTex;
		float _Cut;
		float4 _OutColor;
		float _OutThinkness;

        struct Input
        {
            float2 uv_MainTex;
			float2 uv_BumpMap;
            float2 uv_NoiseTex;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            fixed4 noise = tex2D (_NoiseTex, IN.uv_NoiseTex);
            o.Albedo = c.rgb;			
			float _cutTime = _Cut + _Time.x;

			float alpha;
			if(noise.r >= _Cut)
			alpha =1;
			else
			alpha =0;
			
			float outline;
			if(noise.r >= _Cut * _OutThinkness)
			outline=0;
			else
			outline=1;
			o.Emission = outline * _OutColor.rgb;
            o.Alpha = alpha;
        }
        ENDCG
    }
    FallBack  "Diffuse"//"Legacy Shaders/Transparent/VertexLit"
}
