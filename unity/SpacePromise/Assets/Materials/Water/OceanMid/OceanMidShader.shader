// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SP/Water/OceanMid"
{
	Properties
	{
		_NormalMap("Normal Map", 2D) = "bump" {}
		_WaveSmallUV1Scale("Wave Small UV 1 Scale", Range( 0 , 1)) = 1
		[Header(Refraction)]
		_ChromaticAberration("Chromatic Aberration", Range( 0 , 0.3)) = 0.1
		_WaveSmallUV2Scale("Wave Small UV 2 Scale", Range( 0 , 1)) = 1
		_WaveLargeUVScale("Wave Large UV Scale", Range( 0 , 2)) = 1
		_UVScale("UV Scale", Range( 0.0001 , 10000)) = 1
		_WaveLargeUVTiling("Wave Large UV Tiling", Float) = 1
		_WaveSmallUV1Tiling("Wave Small UV 1 Tiling", Float) = 1
		_WaveSmallUV2Tiling("Wave Small UV 2 Tiling", Float) = 1
		_WaveSmallUV1AnimationSpeed("Wave Small UV 1 Animation Speed", Vector) = (1,2,0,0)
		_WaveUVLargeAnimationSpeed("Wave UV Large Animation Speed", Vector) = (1,2,0,0)
		_WaveSmallUV2AnimationSpeed("Wave Small UV 2 Animation Speed", Vector) = (2,1,0,0)
		_ColorFresnelStrength("Color Fresnel Strength", Range( 0 , 10)) = 1.336
		_DepthFade("Depth Fade", Range( 0 , 10)) = 0
		_AColor("A Color", Range( 0 , 3)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma multi_compile _ALPHAPREMULTIPLY_ON
		#pragma only_renderers d3d11 xboxone 
		#pragma surface surf Standard alpha:fade keepalpha finalcolor:RefractionF noshadow exclude_path:deferred 
		struct Input
		{
			float3 worldPos;
			INTERNAL_DATA
			float3 worldNormal;
			float4 screenPos;
		};

		uniform sampler2D _NormalMap;
		uniform float _WaveLargeUVScale;
		uniform float2 _WaveUVLargeAnimationSpeed;
		uniform float _UVScale;
		uniform float _WaveLargeUVTiling;
		uniform float _WaveSmallUV1Scale;
		uniform float2 _WaveSmallUV1AnimationSpeed;
		uniform float _WaveSmallUV1Tiling;
		uniform float _WaveSmallUV2Scale;
		uniform float2 _WaveSmallUV2AnimationSpeed;
		uniform float _WaveSmallUV2Tiling;
		uniform float _ColorFresnelStrength;
		uniform sampler2D _CameraDepthTexture;
		uniform float _DepthFade;
		uniform float _AColor;
		uniform sampler2D _GrabTexture;
		uniform float _ChromaticAberration;

		inline float4 Refraction( Input i, SurfaceOutputStandard o, float indexOfRefraction, float chomaticAberration ) {
			float3 worldNormal = o.Normal;
			float4 screenPos = i.screenPos;
			#if UNITY_UV_STARTS_AT_TOP
				float scale = -1.0;
			#else
				float scale = 1.0;
			#endif
			float halfPosW = screenPos.w * 0.5;
			screenPos.y = ( screenPos.y - halfPosW ) * _ProjectionParams.x * scale + halfPosW;
			#if SHADER_API_D3D9 || SHADER_API_D3D11
				screenPos.w += 0.00000000001;
			#endif
			float2 projScreenPos = ( screenPos / screenPos.w ).xy;
			float3 worldViewDir = normalize( UnityWorldSpaceViewDir( i.worldPos ) );
			float3 refractionOffset = ( ( ( ( indexOfRefraction - 1.0 ) * mul( UNITY_MATRIX_V, float4( worldNormal, 0.0 ) ) ) * ( 1.0 / ( screenPos.z + 1.0 ) ) ) * ( 1.0 - dot( worldNormal, worldViewDir ) ) );
			float2 cameraRefraction = float2( refractionOffset.x, -( refractionOffset.y * _ProjectionParams.x ) );
			float4 redAlpha = tex2D( _GrabTexture, ( projScreenPos + cameraRefraction ) );
			float green = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 - chomaticAberration ) ) ) ).g;
			float blue = tex2D( _GrabTexture, ( projScreenPos + ( cameraRefraction * ( 1.0 + chomaticAberration ) ) ) ).b;
			return float4( redAlpha.r, green, blue, redAlpha.a );
		}

		void RefractionF( Input i, SurfaceOutputStandard o, inout fixed4 color )
		{
			#ifdef UNITY_PASS_FORWARDBASE
			color.rgb = color.rgb + Refraction( i, o, 1.0, _ChromaticAberration ) * ( 1 - color.a );
			color.a = 1;
			#endif
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float3 ase_worldPos = i.worldPos;
			float2 appendResult164 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 WorldUV5 = ( appendResult164 / _UVScale );
			float2 panner144 = ( ( WorldUV5 * _WaveLargeUVTiling ) + _Time.x * _WaveUVLargeAnimationSpeed);
			float2 _WaveLargeUV1159 = panner144;
			float3 LargeWavesNormal152 = UnpackScaleNormal( tex2D( _NormalMap, _WaveLargeUV1159 ) ,_WaveLargeUVScale );
			float2 panner34 = ( ( WorldUV5 * _WaveSmallUV1Tiling ) + _Time.x * _WaveSmallUV1AnimationSpeed);
			float2 _WaveSmallUV123 = panner34;
			float2 panner35 = ( ( WorldUV5 * _WaveSmallUV2Tiling ) + _Time.x * _WaveSmallUV2AnimationSpeed);
			float2 _WaveSmallUV224 = panner35;
			float3 SmallWavesNormal11 = BlendNormals( UnpackScaleNormal( tex2D( _NormalMap, _WaveSmallUV123 ) ,_WaveSmallUV1Scale ) , UnpackScaleNormal( tex2D( _NormalMap, _WaveSmallUV224 ) ,_WaveSmallUV2Scale ) );
			float3 Norma154 = BlendNormals( LargeWavesNormal152 , SmallWavesNormal11 );
			o.Normal = Norma154;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNDotV30 = dot( WorldNormalVector( i , Norma154 ), ase_worldViewDir );
			float fresnelNode30 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNDotV30, _ColorFresnelStrength ) );
			float4 lerpResult18 = lerp( float4(0.01849048,0.04411763,0.04093633,0) , float4(0.009731836,0.0655515,0.07352942,0) , fresnelNode30);
			float4 _AlbedoColor21 = lerpResult18;
			o.Albedo = _AlbedoColor21.rgb;
			o.Metallic = 0.0;
			o.Smoothness = 1.0;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth66 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth66 = abs( ( screenDepth66 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFade ) );
			float Opacity93 = saturate( pow( distanceDepth66 , _AColor ) );
			o.Alpha = Opacity93;
			o.Normal = o.Normal + 0.00001 * i.screenPos * i.worldPos;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
-1418;79;1303;926;4253.459;1270.272;1.314942;True;False
Node;AmplifyShaderEditor.CommentaryNode;51;-3735.057,-1086.93;Float;False;974.2006;388.7992;;5;1;4;3;5;164;World UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;1;-3685.057,-1035.631;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;164;-3395.186,-1033.307;Float;False;FLOAT2;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-3493.057,-813.1312;Float;False;Property;_UVScale;UV Scale;6;0;Create;1;10;0.0001;10000;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;163;-2652.068,-1084.382;Float;False;1047.365;1107.019;;17;25;33;36;39;34;35;144;24;23;159;145;146;41;40;147;38;37;UV Animation;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;3;-3172.856,-1031.73;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT;0.0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;25;-2533.836,-878.5872;Float;False;5;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-2602.068,-792.2935;Float;False;Property;_WaveSmallUV1Tiling;Wave Small UV 1 Tiling;8;0;Create;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;38;-2601.931,-521.2899;Float;False;Property;_WaveSmallUV2Tiling;Wave Small UV 2 Tiling;9;0;Create;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;5;-3003.856,-1036.93;Float;False;WorldUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;33;-2559.802,-1034.382;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-2303.739,-807.3601;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;147;-2587.985,-240.8761;Float;False;Property;_WaveLargeUVTiling;Wave Large UV Tiling;7;0;Create;1;0.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;40;-2497.56,-701.0361;Float;False;Property;_WaveSmallUV1AnimationSpeed;Wave Small UV 1 Animation Speed;10;0;Create;1,2;1,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-2305.039,-539.8176;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;41;-2497.213,-422.3205;Float;False;Property;_WaveSmallUV2AnimationSpeed;Wave Small UV 2 Animation Speed;12;0;Create;2,1;2,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;145;-2304.538,-257.9274;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;146;-2486.589,-138.3624;Float;False;Property;_WaveUVLargeAnimationSpeed;Wave UV Large Animation Speed;11;0;Create;1,2;-0.2,-0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.CommentaryNode;162;-1468.891,-1094.032;Float;False;1273.496;814.3037;;14;27;10;9;11;29;28;142;47;46;161;141;14;152;166;Wave Normals;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;35;-2114.78,-539.419;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;34;-2111.182,-806.7899;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-1866.295,-543.8397;Float;False;_WaveSmallUV2;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-1417.327,-836.4111;Float;False;Property;_WaveSmallUV1Scale;Wave Small UV 1 Scale;1;0;Create;1;0.7;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;144;-2115.758,-254.8342;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;46;-1375.627,-562.0621;Float;False;23;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-1863.702,-812.9138;Float;False;_WaveSmallUV1;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;47;-1373.725,-479.562;Float;False;24;0;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;27;-1364.402,-1041.911;Float;True;Property;_NormalMap;Normal Map;0;0;Create;None;dd2fd2df93418444c8e280f1d34deeb5;True;bump;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;28;-1417.327,-748.0057;Float;False;Property;_WaveSmallUV2Scale;Wave Small UV 2 Scale;3;0;Create;1;0.9;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;159;-1885.866,-247.0631;Float;False;_WaveLargeUV1;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;10;-1043.103,-841.5198;Float;True;Property;_TextureSample1;Texture Sample 1;1;0;Create;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;9;-1043.103,-1043.861;Float;True;Property;_Texture;Texture;1;0;Create;dd2fd2df93418444c8e280f1d34deeb5;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;161;-1374.121,-394.7275;Float;False;159;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;142;-1418.891,-658.7202;Float;False;Property;_WaveLargeUVScale;Wave Large UV Scale;4;0;Create;1;1.2;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendNormalsNode;166;-711.2119,-1040.055;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;141;-1044.898,-553.9395;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;dd2fd2df93418444c8e280f1d34deeb5;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;158;-94.25845,-1094.211;Float;False;935.1998;258.7001;;4;153;150;151;154;Normals;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;153;-44.25869,-1044.211;Float;False;152;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;11;-486.7185,-1045.332;Float;False;SmallWavesNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;151;-40.95864,-950.5109;Float;False;11;0;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;95;-1354.703,1314.789;Float;False;1181.327;256.2783;;6;66;67;89;91;92;93;Opacity;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-474.3952,-557.9062;Float;False;LargeWavesNormal;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;45;-1359.534,363.6321;Float;False;1156.732;698.7744;;7;16;17;18;30;31;21;50;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.BlendNormalsNode;150;319.4408,-1039.512;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-1304.703,1364.789;Float;False;Property;_DepthFade;Depth Fade;14;0;Create;0;8;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;66;-1002.729,1369.997;Float;False;True;1;0;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;154;597.9406,-1043.411;Float;False;Norma;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-1302.982,1456.068;Float;False;Property;_AColor;A Color;15;0;Create;1;0.5;0;3;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-1336.778,883.3715;Float;False;Property;_ColorFresnelStrength;Color Fresnel Strength;13;0;Create;1.336;1.336;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;31;-1330.481,802.5475;Float;False;154;0;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;91;-770.2299,1369.31;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;17;-1019.515,414.8507;Float;False;Constant;_ColorDeep;Color (Deep);3;0;Create;0.01849048,0.04411763,0.04093633,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;30;-1050.272,810.4068;Float;True;Tangent;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;16;-1018.714,586.9472;Float;False;Constant;_ColorShallow;Color (Shallow);3;0;Create;0.009731836,0.0655515,0.07352942,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;92;-587.7161,1370.871;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;18;-741.7786,420.3457;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;64;926.2911,7.209966;Float;False;21;0;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;93;-416.3769,1366.764;Float;False;Opacity;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;65;991.0145,181.4911;Float;False;Constant;_Metallic;Metallic;10;0;Create;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;977.5905,268.0414;Float;False;Constant;_Glossiness;Glossiness;9;0;Create;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;21;-445.8037,413.632;Float;False;_AlbedoColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;78;996.3945,353.9826;Float;False;Constant;_Float1;Float 1;14;0;Create;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;94;956.2424,438.0285;Float;False;93;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;902.2434,99.44939;Float;False;154;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-1036.2,-639.3968;Float;False;Property;_NormalBlendStrength;Normal Blend Strength;5;0;Create;0.5;0.355;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1263.362,81.33418;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SP/Water/OceanMid;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;ForwardOnly;False;True;False;False;False;False;False;False;True;False;False;False;False;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;2;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;164;0;1;1
WireConnection;164;1;1;3
WireConnection;3;0;164;0
WireConnection;3;1;4;0
WireConnection;5;0;3;0
WireConnection;36;0;25;0
WireConnection;36;1;37;0
WireConnection;39;0;25;0
WireConnection;39;1;38;0
WireConnection;145;0;25;0
WireConnection;145;1;147;0
WireConnection;35;0;39;0
WireConnection;35;2;41;0
WireConnection;35;1;33;1
WireConnection;34;0;36;0
WireConnection;34;2;40;0
WireConnection;34;1;33;1
WireConnection;24;0;35;0
WireConnection;144;0;145;0
WireConnection;144;2;146;0
WireConnection;144;1;33;1
WireConnection;23;0;34;0
WireConnection;159;0;144;0
WireConnection;10;0;27;0
WireConnection;10;1;47;0
WireConnection;10;5;28;0
WireConnection;9;0;27;0
WireConnection;9;1;46;0
WireConnection;9;5;29;0
WireConnection;166;0;9;0
WireConnection;166;1;10;0
WireConnection;141;0;27;0
WireConnection;141;1;161;0
WireConnection;141;5;142;0
WireConnection;11;0;166;0
WireConnection;152;0;141;0
WireConnection;150;0;153;0
WireConnection;150;1;151;0
WireConnection;66;0;67;0
WireConnection;154;0;150;0
WireConnection;91;0;66;0
WireConnection;91;1;89;0
WireConnection;30;0;31;0
WireConnection;30;3;50;0
WireConnection;92;0;91;0
WireConnection;18;0;17;0
WireConnection;18;1;16;0
WireConnection;18;2;30;0
WireConnection;93;0;92;0
WireConnection;21;0;18;0
WireConnection;0;0;64;0
WireConnection;0;1;12;0
WireConnection;0;3;65;0
WireConnection;0;4;49;0
WireConnection;0;8;78;0
WireConnection;0;9;94;0
ASEEND*/
//CHKSM=1978225E1AB1CF9F5888B1FD6AB99822C4ED03C1