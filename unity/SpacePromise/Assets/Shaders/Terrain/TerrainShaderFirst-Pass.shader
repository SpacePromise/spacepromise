// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SP/Terrain/Terrain"
{
	Properties
	{
		[Header(Four Splats First Pass Terrain)]
		[HideInInspector]_Control("Control", 2D) = "white" {}
		[HideInInspector]_Splat3("Splat3", 2D) = "white" {}
		[HideInInspector]_Splat2("Splat2", 2D) = "white" {}
		[HideInInspector]_Splat1("Splat1", 2D) = "white" {}
		[HideInInspector]_Splat0("Splat0", 2D) = "white" {}
		[HideInInspector]_Normal0("Normal0", 2D) = "white" {}
		[HideInInspector]_Normal1("Normal1", 2D) = "white" {}
		[HideInInspector]_Normal2("Normal2", 2D) = "white" {}
		[HideInInspector]_Normal3("Normal3", 2D) = "white" {}
		[HideInInspector]_Smoothness3("Smoothness3", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness1("Smoothness1", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness2("Smoothness2", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness0("Smoothness0", Range( 0 , 1)) = 1
		[HideInInspector][Gamma]_Metallic3("Metallic3", Range( 0 , 1)) = 0
		[HideInInspector][Gamma]_Metallic1("Metallic1", Range( 0 , 1)) = 0
		[HideInInspector][Gamma]_Metallic0("Metallic0", Range( 0 , 1)) = 0
		[HideInInspector][Gamma]_Metallic2("Metallic2", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry-100" "SplatCount"="4" }
		Cull Back
		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Control;
		uniform float4 _Control_ST;
		uniform sampler2D _Normal0;
		uniform float4 _Normal0_ST;
		uniform sampler2D _Normal1;
		uniform float4 _Normal1_ST;
		uniform sampler2D _Normal2;
		uniform float4 _Normal2_ST;
		uniform sampler2D _Normal3;
		uniform float4 _Normal3_ST;
		uniform float _Smoothness0;
		uniform sampler2D _Splat0;
		uniform float4 _Splat0_ST;
		uniform float _Smoothness1;
		uniform sampler2D _Splat1;
		uniform float4 _Splat1_ST;
		uniform float _Smoothness2;
		uniform sampler2D _Splat2;
		uniform float4 _Splat2_ST;
		uniform float _Smoothness3;
		uniform sampler2D _Splat3;
		uniform float4 _Splat3_ST;
		uniform float _Metallic0;
		uniform float _Metallic1;
		uniform float _Metallic2;
		uniform float _Metallic3;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Control = i.uv_texcoord * _Control_ST.xy + _Control_ST.zw;
			float4 tex2DNode5_g1 = tex2D( _Control, uv_Control );
			float dotResult20_g1 = dot( tex2DNode5_g1 , float4(1,1,1,1) );
			float SplatWeight22_g1 = dotResult20_g1;
			float4 SplatControl26_g1 = ( tex2DNode5_g1 / ( SplatWeight22_g1 + 0.001 ) );
			float4 temp_output_59_0_g1 = SplatControl26_g1;
			float2 uv_Normal0 = i.uv_texcoord * _Normal0_ST.xy + _Normal0_ST.zw;
			float2 uv_Normal1 = i.uv_texcoord * _Normal1_ST.xy + _Normal1_ST.zw;
			float2 uv_Normal2 = i.uv_texcoord * _Normal2_ST.xy + _Normal2_ST.zw;
			float2 uv_Normal3 = i.uv_texcoord * _Normal3_ST.xy + _Normal3_ST.zw;
			float4 weightedBlendVar8_g1 = temp_output_59_0_g1;
			float4 weightedBlend8_g1 = ( weightedBlendVar8_g1.x*tex2D( _Normal0, uv_Normal0 ) + weightedBlendVar8_g1.y*tex2D( _Normal1, uv_Normal1 ) + weightedBlendVar8_g1.z*tex2D( _Normal2, uv_Normal2 ) + weightedBlendVar8_g1.w*tex2D( _Normal3, uv_Normal3 ) );
			o.Normal = UnpackNormal( weightedBlend8_g1 );
			float4 appendResult33_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness0));
			float2 uv_Splat0 = i.uv_texcoord * _Splat0_ST.xy + _Splat0_ST.zw;
			float4 appendResult36_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness1));
			float2 uv_Splat1 = i.uv_texcoord * _Splat1_ST.xy + _Splat1_ST.zw;
			float4 appendResult39_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness2));
			float2 uv_Splat2 = i.uv_texcoord * _Splat2_ST.xy + _Splat2_ST.zw;
			float4 appendResult42_g1 = (float4(1.0 , 1.0 , 1.0 , _Smoothness3));
			float2 uv_Splat3 = i.uv_texcoord * _Splat3_ST.xy + _Splat3_ST.zw;
			float4 weightedBlendVar9_g1 = temp_output_59_0_g1;
			float4 weightedBlend9_g1 = ( weightedBlendVar9_g1.x*( appendResult33_g1 * tex2D( _Splat0, uv_Splat0 ) ) + weightedBlendVar9_g1.y*( appendResult36_g1 * tex2D( _Splat1, uv_Splat1 ) ) + weightedBlendVar9_g1.z*( appendResult39_g1 * tex2D( _Splat2, uv_Splat2 ) ) + weightedBlendVar9_g1.w*( appendResult42_g1 * tex2D( _Splat3, uv_Splat3 ) ) );
			float4 MixDiffuse28_g1 = weightedBlend9_g1;
			o.Albedo = MixDiffuse28_g1.xyz;
			float4 appendResult55_g1 = (float4(_Metallic0 , _Metallic1 , _Metallic2 , _Metallic3));
			float dotResult53_g1 = dot( SplatControl26_g1 , appendResult55_g1 );
			o.Metallic = dotResult53_g1;
			o.Smoothness = (MixDiffuse28_g1).w;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
-1433;75;1426;936;2240.815;1052.248;2.749874;True;False
Node;AmplifyShaderEditor.FunctionNode;5;-614.0111,108.0996;Float;False;Four Splats First Pass Terrain;0;;1;37452fdfb732e1443b7e39720d05b708;6;59;FLOAT4;0,0,0,0;False;60;FLOAT4;0,0,0,0;False;61;FLOAT3;0,0,0;False;57;FLOAT;0.0;False;58;FLOAT;0.0;False;62;FLOAT;0.0;False;6;FLOAT4;0;FLOAT3;14;FLOAT;56;FLOAT;45;FLOAT;19;FLOAT;17
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SP/Terrain/Terrain;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;-100;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;1;SplatCount=4;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;0;0;5;0
WireConnection;0;1;5;14
WireConnection;0;3;5;56
WireConnection;0;4;5;45
ASEEND*/
//CHKSM=3B3B5163D849DD24B312B0F8B766C7A3D94F91A4