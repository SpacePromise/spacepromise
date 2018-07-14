// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SP/Water/OceanStaticShader"
{
	Properties
	{
		_DeepWaterColor("Deep Water Color", Color) = (0.02319421,0.08088237,0.07849526,0)
		_ShallowWaterColor("Shallow Water Color", Color) = (0.1128893,0.5294118,0.4949409,0)
		_WaterFresnel("Water Fresnel", Float) = 1.337
		_WaterDepth("Water Depth", Float) = 0
		_DepthFalloff("Depth Falloff", Float) = 0
		_Distortiondepth("Distortion depth", Float) = 0.2
		_SmallWavesTexture("Small Waves Texture", 2D) = "bump" {}
		_SmallWavesTiling("Small Waves Tiling", Float) = 3
		_SmallWavesSpeed("Small Waves Speed", Float) = 30
		_SmallWavesRefraction("Small Waves Refraction", Float) = 0.88
		_MediumWavesTexture("Medium Waves Texture", 2D) = "bump" {}
		_MediumWavesTiling("Medium Waves Tiling", Float) = 3
		_MediumWavesSpeed("Medium Waves Speed", Float) = -30
		_MediumWavesRefraction("Medium Waves Refraction", Float) = 1.18
		_LargeWavesTexture("Large Waves Texture", 2D) = "bump" {}
		_LargeWavesTiling("Large Waves Tiling", Float) = 0.5
		_LargeWavesSpeed("Large Waves Speed", Float) = -10
		_LargeWavesRefraction("Large Waves Refraction", Float) = 1.7
		_DistanceWavesTiling("Distance Waves Tiling", Float) = 0.5
		_DistanceWavesSpeed("Distance Waves Speed", Float) = -1
		_DistanceWavesRefraction("Distance Waves Refraction", Float) = 0.1
		_DistanceWavesDistance("Distance Waves Distance", Float) = 800
		_DistanceWavesFalloff("Distance Waves Falloff", Float) = 0.41
		_FoamTexture("Foam Texture", 2D) = "white" {}
		_AmbientOcclusion("Ambient Occlusion", Float) = 0
		_Coloramount("Color amount", Range( 0 , 1)) = 0
		_FoamTiling("Foam Tiling", Float) = 0
		_FoamSpeed("Foam Speed", Float) = 0
		_FoamDistance("Foam Distance", Float) = 0
		_FoamColor("Foam Color", Color) = (1,1,1,0)
		_FoamDepthFade("Foam Depth Fade", Float) = 1
		_FoamActivity("Foam Activity", Float) = 50
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
			INTERNAL_DATA
			float3 worldNormal;
			float2 uv_texcoord;
		};

		uniform sampler2D _SmallWavesTexture;
		uniform float _SmallWavesRefraction;
		uniform float _SmallWavesSpeed;
		uniform float _SmallWavesTiling;
		uniform sampler2D _MediumWavesTexture;
		uniform float _MediumWavesRefraction;
		uniform float _MediumWavesSpeed;
		uniform float _MediumWavesTiling;
		uniform sampler2D _LargeWavesTexture;
		uniform float _LargeWavesRefraction;
		uniform float _LargeWavesSpeed;
		uniform float _LargeWavesTiling;
		uniform float _DistanceWavesRefraction;
		uniform float _DistanceWavesSpeed;
		uniform float _DistanceWavesTiling;
		uniform float _DistanceWavesDistance;
		uniform float _DistanceWavesFalloff;
		uniform sampler2D _GrabTexture;
		uniform sampler2D _CameraDepthTexture;
		uniform float _WaterDepth;
		uniform float _Distortiondepth;
		uniform float4 _ShallowWaterColor;
		uniform float4 _DeepWaterColor;
		uniform float _WaterFresnel;
		uniform float _DepthFalloff;
		uniform float _Coloramount;
		uniform float4 _FoamColor;
		uniform sampler2D _FoamTexture;
		uniform float _FoamSpeed;
		uniform float _FoamTiling;
		uniform float _FoamActivity;
		uniform float _FoamDistance;
		uniform float _FoamDepthFade;
		uniform float _AmbientOcclusion;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float _smallWavesRefraction17 = _SmallWavesRefraction;
			float _smallWavesSpeed16 = _SmallWavesSpeed;
			float2 temp_cast_0 = (( _smallWavesSpeed16 * 0.01 )).xx;
			float3 ase_worldPos = i.worldPos;
			float2 appendResult36 = (float2(ase_worldPos.x , ase_worldPos.z));
			float2 _worldUV31 = ( appendResult36 / 20.0 );
			float _smallWavesTiling15 = _SmallWavesTiling;
			float2 panner7_g21 = ( ( _worldUV31 * _smallWavesTiling15 ) + _Time.x * temp_cast_0);
			float3 _normalSmallWaves50 = UnpackScaleNormal( tex2D( _SmallWavesTexture, panner7_g21 ) ,_smallWavesRefraction17 );
			float _mediumWavesRefraction21 = _MediumWavesRefraction;
			float _mediumWavesSpeed20 = _MediumWavesSpeed;
			float2 temp_cast_1 = (( _mediumWavesSpeed20 * 0.01 )).xx;
			float _mediumWavesTiling19 = _MediumWavesTiling;
			float2 panner7_g22 = ( ( _worldUV31 * _mediumWavesTiling19 ) + _Time.x * temp_cast_1);
			float3 _normalMediumWaves72 = UnpackScaleNormal( tex2D( _MediumWavesTexture, panner7_g22 ) ,_mediumWavesRefraction21 );
			float3 lerpResult75 = lerp( _normalSmallWaves50 , _normalMediumWaves72 , 0.5);
			float _largeWavesRefraction25 = _LargeWavesRefraction;
			float _largeWavesSpeed24 = _LargeWavesSpeed;
			float2 temp_cast_2 = (( _largeWavesSpeed24 * 0.01 )).xx;
			float _largeWavesTiling23 = _LargeWavesTiling;
			float2 panner7_g23 = ( ( _worldUV31 * _largeWavesTiling23 ) + _Time.x * temp_cast_2);
			float3 _normalLargeWaves87 = UnpackScaleNormal( tex2D( _LargeWavesTexture, panner7_g23 ) ,_largeWavesRefraction25 );
			float3 lerpResult89 = lerp( lerpResult75 , _normalLargeWaves87 , 0.5);
			float _distanceWavesRefraction144 = _DistanceWavesRefraction;
			float _distanceWavesSpeed135 = _DistanceWavesSpeed;
			float2 temp_cast_3 = (( _distanceWavesSpeed135 * 0.01 )).xx;
			float _distanceWavesTiling134 = _DistanceWavesTiling;
			float2 panner7_g24 = ( ( _worldUV31 * _distanceWavesTiling134 ) + _Time.x * temp_cast_3);
			float3 _normalDistanceWaves142 = UnpackScaleNormal( tex2D( _LargeWavesTexture, panner7_g24 ) ,_distanceWavesRefraction144 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float _distanceWavesDistance157 = _DistanceWavesDistance;
			float _distanceWavesFalloff158 = _DistanceWavesFalloff;
			float clampResult147 = clamp( pow( ( ase_screenPos.y / _distanceWavesDistance157 ) , _distanceWavesFalloff158 ) , 0 , 1 );
			float3 lerpResult146 = lerp( lerpResult89 , _normalDistanceWaves142 , clampResult147);
			float3 _outNormals77 = lerpResult146;
			o.Normal = _outNormals77;
			float2 appendResult94 = (float2(ase_screenPos.x , ase_screenPos.y));
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth109 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth109 = abs( ( screenDepth109 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _WaterDepth ) );
			float _waterDepthFade184 = distanceDepth109;
			float clampResult194 = clamp( pow( _waterDepthFade184 , _Distortiondepth ) , 0 , 1 );
			float4 screenColor100 = tex2D( _GrabTexture, ( float3( ( appendResult94 / ase_screenPos.w ) ,  0.0 ) + ( _outNormals77 * clampResult194 ) ).xy );
			float4 _surfaceDistortion102 = screenColor100;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNDotV47 = dot( WorldNormalVector( i , _outNormals77 ), ase_worldViewDir );
			float fresnelNode47 = ( 0 + 1 * pow( 1.0 - fresnelNDotV47, _WaterFresnel ) );
			float4 lerpResult46 = lerp( _ShallowWaterColor , _DeepWaterColor , saturate( ( fresnelNode47 + pow( _waterDepthFade184 , 0.0 ) ) ));
			float4 _surfaceColor51 = lerpResult46;
			float4 lerpResult105 = lerp( _surfaceDistortion102 , _surfaceColor51 , saturate( ( pow( _waterDepthFade184 , _DepthFalloff ) + _Coloramount ) ));
			float4 _outAlbedo106 = lerpResult105;
			o.Albedo = _outAlbedo106.rgb;
			float _foamSpeed209 = _FoamSpeed;
			float2 temp_cast_7 = (( _foamSpeed209 * 0.01 )).xx;
			float _foamTiling204 = _FoamTiling;
			float2 panner7_g25 = ( ( _worldUV31 * _foamTiling204 ) + _Time.x * temp_cast_7);
			float2 temp_output_206_0 = panner7_g25;
			float2 temp_output_27_0_g26 = temp_output_206_0;
			float _foamActivity245 = _FoamActivity;
			float2 temp_cast_8 = (_foamActivity245).xx;
			float2 temp_output_28_0_g26 = temp_cast_8;
			float2 uv_TexCoord3_g26 = i.uv_texcoord * temp_output_27_0_g26 + temp_output_28_0_g26;
			float simplePerlin2D7_g26 = snoise( uv_TexCoord3_g26 );
			float2 uv_TexCoord5_g26 = i.uv_texcoord * ( temp_output_27_0_g26 * float2( 2,2 ) ) + temp_output_28_0_g26;
			float simplePerlin2D10_g26 = snoise( uv_TexCoord5_g26 );
			float layeredBlendVar17_g26 = 0.1;
			float layeredBlend17_g26 = ( lerp( (simplePerlin2D7_g26*0.06181384 + 0),(simplePerlin2D10_g26*0.3648041 + 0) , layeredBlendVar17_g26 ) );
			float2 uv_TexCoord11_g26 = i.uv_texcoord * ( temp_output_27_0_g26 * float2( 6,6 ) ) + temp_output_28_0_g26;
			float simplePerlin2D16_g26 = snoise( uv_TexCoord11_g26 );
			float layeredBlendVar21_g26 = 0.2;
			float layeredBlend21_g26 = ( lerp( layeredBlend17_g26,(simplePerlin2D16_g26*0.05737751 + 0) , layeredBlendVar21_g26 ) );
			float2 uv_TexCoord15_g26 = i.uv_texcoord * ( temp_output_27_0_g26 * float2( 12,12 ) ) + temp_output_28_0_g26;
			float simplePerlin2D18_g26 = snoise( uv_TexCoord15_g26 );
			float layeredBlendVar25_g26 = 0.5;
			float layeredBlend25_g26 = ( lerp( layeredBlend21_g26,(simplePerlin2D18_g26*0.01 + 0) , layeredBlendVar25_g26 ) );
			float clampResult235 = clamp( (layeredBlend25_g26*1.470588 + 0.05) , 0 , 1 );
			float _foamDistance247 = _FoamDistance;
			float screenDepth227 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD(ase_screenPos))));
			float distanceDepth227 = abs( ( screenDepth227 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _foamDistance247 ) );
			float _foamDepthFade246 = _FoamDepthFade;
			float clampResult229 = clamp( ( clampResult235 * pow( distanceDepth227 , _foamDepthFade246 ) ) , 0 , 1 );
			float4 lerpResult225 = lerp( ( _FoamColor * tex2D( _FoamTexture, temp_output_206_0 ) ) , float4(0,0,0,0) , saturate( clampResult229 ));
			float4 _outFoam201 = ( lerpResult225 * _LightColor0.a );
			o.Emission = _outFoam201.rgb;
			o.Metallic = 0.1;
			o.Smoothness = 0.85;
			o.Occlusion = _AmbientOcclusion;
			o.Alpha = 1.0;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14501
-1433;75;1426;936;4661.963;2409.185;6.487071;True;False
Node;AmplifyShaderEditor.CommentaryNode;34;-1638.572,-1785.004;Float;False;974.2006;388.7992;;5;39;36;35;31;32;World UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;35;-1588.572,-1733.705;Float;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;129;-1619.785,-535.8259;Float;False;659.924;326.0319;;6;9;10;20;19;11;21;Medium Waves Parameters;1,1,1,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;36;-1298.701,-1700.181;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;128;-1615.078,-1012.867;Float;False;649.563;326.1995;;6;8;6;15;16;17;7;Small Waves Parameters;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;32;-1290.377,-1521.949;Float;False;Constant;_WorldUVScale;World UV Scale;13;0;Create;True;0;20;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1565.078,-962.7554;Float;False;Property;_SmallWavesTiling;Small Waves Tiling;7;0;Create;True;0;3;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;130;-1605.892,-39.69918;Float;False;641.124;326.1994;;6;13;12;23;24;14;25;Large Waves Parameters;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;39;-1075.071,-1698.604;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;168;-481.597,-1780.606;Float;False;1559.845;2146.551;;31;68;58;28;67;69;33;165;81;2;1;80;85;84;86;166;164;136;138;3;82;27;71;137;83;72;140;163;50;87;141;142;Normals Animation;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;10;-1568.184,-405.6251;Float;False;Property;_MediumWavesSpeed;Medium Waves Speed;12;0;Create;True;0;-30;-15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-1569.785,-485.8259;Float;False;Property;_MediumWavesTiling;Medium Waves Tiling;11;0;Create;True;0;3;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;8;-1563.477,-882.5546;Float;False;Property;_SmallWavesSpeed;Small Waves Speed;8;0;Create;True;0;30;15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;16;-1248.513,-881.6669;Float;False;_smallWavesSpeed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;-1554.291,90.66962;Float;False;Property;_LargeWavesSpeed;Large Waves Speed;16;0;Create;True;0;-10;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-1256.861,-404.7937;Float;False;_mediumWavesSpeed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;31;-888.4729,-1702.649;Float;False;_worldUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;69;-411.2479,-796.5191;Float;False;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;28;-380.7896,-1435.919;Float;False;15;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-1555.892,10.46882;Float;False;Property;_LargeWavesTiling;Large Waves Tiling;15;0;Create;True;0;0.5;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;68;-406.9699,-896.3425;Float;False;19;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-390.8882,-1342.862;Float;False;16;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;15;-1245.313,-962.8666;Float;False;_smallWavesTiling;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;167;-1618.909,468.3901;Float;False;661.124;492.0364;;10;150;133;132;156;157;143;135;134;144;158;Distance Waves Parameters;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;-321.3347,-1531.533;Float;False;31;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;19;-1253.46,-484.9934;Float;False;_mediumWavesTiling;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;67;-332.8152,-996.1659;Float;False;31;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-1568.785,-325.426;Float;False;Property;_MediumWavesRefraction;Medium Waves Refraction;13;0;Create;True;0;1.18;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;7;-1564.078,-802.3555;Float;False;Property;_SmallWavesRefraction;Small Waves Refraction;9;0;Create;True;0;0.88;1.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;21;-1283.862,-324.794;Float;False;_mediumWavesRefraction;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;132;-1568.909,518.5581;Float;False;Property;_DistanceWavesTiling;Distance Waves Tiling;18;0;Create;True;0;0.5;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;23;-1240.366,10.30081;Float;False;_largeWavesTiling;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;-334.8043,-439.4544;Float;False;31;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-1272.515,-801.6672;Float;False;_smallWavesRefraction;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;1;-365.4624,-1730.194;Float;True;Property;_SmallWavesTexture;Small Waves Texture;6;0;Create;True;0;None;ecfc88f5990da574d9cc49f445004aa9;True;bump;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.FunctionNode;80;20.84472,-991.8878;Float;False;WaterNormalAnimator;-1;;22;d1a09ea6051e067409434c06db4176c7;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;14;-1554.892,170.8687;Float;False;Property;_LargeWavesRefraction;Large Waves Refraction;17;0;Create;True;0;1.7;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;-401.5782,-259.5337;Float;False;24;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;165;-23.52949,-799.3806;Float;False;21;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;24;-1244.766,91.5006;Float;False;_largeWavesSpeed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;150;-1567.368,762.9294;Float;False;Property;_DistanceWavesDistance;Distance Waves Distance;21;0;Create;True;0;800;1400;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;166;-12.40679,-1344.406;Float;False;17;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;133;-1567.308,598.7589;Float;False;Property;_DistanceWavesSpeed;Distance Waves Speed;19;0;Create;True;0;-1;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;85;-396.4888,-352.3353;Float;False;23;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;81;13.87512,-1527.781;Float;False;WaterNormalAnimator;-1;;21;d1a09ea6051e067409434c06db4176c7;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;161;1269.678,-1776.412;Float;False;1565.897;926.8165;;15;76;73;74;88;75;148;89;146;77;145;149;160;159;155;147;Normals Blend;1,1,1,1;0;0
Node;AmplifyShaderEditor.TexturePropertyNode;2;-382.3819,-1205.738;Float;True;Property;_MediumWavesTexture;Medium Waves Texture;10;0;Create;True;0;None;f8a00a19488d121478b84869f34bc2f0;True;bump;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;27;379.1183,-1730.606;Float;True;Property;_TextureSample0;Texture Sample 0;13;0;Create;True;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;157;-1272.055,762.9297;Float;False;_distanceWavesDistance;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;135;-1257.783,599.5899;Float;False;_distanceWavesSpeed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;134;-1253.383,518.3901;Float;False;_distanceWavesTiling;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;71;378.7829,-1202.943;Float;True;Property;_TextureSample1;Texture Sample 1;15;0;Create;True;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;3;-372.7544,-646.1754;Float;True;Property;_LargeWavesTexture;Large Waves Texture;14;0;Create;True;0;None;46bc4fbd372093b4e9eee49dd0bbb8fb;True;bump;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;25;-1270.768,171.5002;Float;False;_largeWavesRefraction;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;145;1411.262,-1238.181;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;143;-1567.909,678.9581;Float;False;Property;_DistanceWavesRefraction;Distance Waves Refraction;20;0;Create;True;0;0.1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;138;-426.5075,160.7006;Float;False;134;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;137;-347.8231,72.5816;Float;False;31;0;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;164;-12.40672,-260.711;Float;False;25;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;-1565.013,845.4266;Float;False;Property;_DistanceWavesFalloff;Distance Waves Falloff;22;0;Create;True;0;0.41;0.4117647;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;160;1319.678,-1064.596;Float;False;157;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;82;16.17068,-435.4085;Float;False;WaterNormalAnimator;-1;;23;d1a09ea6051e067409434c06db4176c7;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;136;-431.597,250.5023;Float;False;135;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;158;-1259.055,844.9297;Float;False;_distanceWavesFalloff;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;144;-1283.785,679.5895;Float;False;_distanceWavesRefraction;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;83;381.157,-645.7059;Float;True;Property;_TextureSample2;Texture Sample 2;15;0;Create;True;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;252;-1610.671,1032.354;Float;False;633.4097;517.7004;;10;208;203;228;242;209;204;238;245;247;246;Foam Parameters;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;73;1505.031,-1635.913;Float;False;72;0;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;140;3.151994,76.62749;Float;False;WaterNormalAnimator;-1;;24;d1a09ea6051e067409434c06db4176c7;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;72;777.247,-1202.437;Float;False;_normalMediumWaves;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;50;785.3995,-1730.118;Float;False;_normalSmallWaves;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;149;1729.008,-1186.76;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;159;1584.677,-964.5957;Float;False;158;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;1610.64,-1344.576;Float;False;Constant;_Float2;Float 2;15;0;Create;True;0;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;1519.602,-1726.412;Float;False;50;0;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;163;-52.13123,250.9453;Float;False;144;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;199;-1617.167,1646.364;Float;False;806.5243;165.8138;;3;110;109;184;Water Depth;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;203;-1522.063,1083.364;Float;False;Property;_FoamTiling;Foam Tiling;26;0;Create;True;0;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;88;1517.944,-1540.081;Float;False;87;0;1;FLOAT3;0
Node;AmplifyShaderEditor.CommentaryNode;251;-722.9402,461.7953;Float;False;2248.615;1050.56;;23;221;200;223;224;4;205;210;207;206;240;248;235;233;229;237;227;250;249;201;225;255;256;258;Foam;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;141;368.1386,-133.6696;Float;True;Property;_TextureSample3;Texture Sample 3;15;0;Create;True;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;75;1919.79,-1723.554;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;87;785.1673,-646.5328;Float;False;_normalLargeWaves;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;110;-1567.167,1696.759;Float;False;Property;_WaterDepth;Water Depth;3;0;Create;True;0;0;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;155;1932.303,-1188.301;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;208;-1524.902,1170.225;Float;False;Property;_FoamSpeed;Foam Speed;27;0;Create;True;0;0;150;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;228;-1541.72,1346.97;Float;False;Property;_FoamDistance;Foam Distance;28;0;Create;True;0;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;205;-652.5842,994.2018;Float;False;204;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;210;-657.1312,1086.307;Float;False;209;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;89;2122.585,-1585.871;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;207;-632.2255,911.6097;Float;False;31;0;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;209;-1242.302,1169.427;Float;False;_foamSpeed;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;147;2136.74,-1187.01;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;109;-1332.527,1702.178;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;196;-1607.966,2923.378;Float;False;1749.895;510.7468;;12;92;94;98;97;95;99;100;102;194;195;192;96;Distortion;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;148;1499.162,-1448.78;Float;False;142;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;142;772.1487,-134.4964;Float;False;_normalDistanceWaves;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;242;-1535.016,1258.865;Float;False;Property;_FoamActivity;Foam Activity;31;0;Create;True;0;50;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;204;-1243.677,1082.354;Float;False;_foamTiling;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;146;2336.163,-1348.781;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;245;-1244.06,1257.746;Float;False;_foamActivity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;238;-1560.671,1435.054;Float;False;Property;_FoamDepthFade;Foam Depth Fade;30;0;Create;True;0;1;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;249;-251.1009,1284.905;Float;False;247;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;247;-1244.06,1345.927;Float;False;_foamDistance;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;206;-316.4778,917.19;Float;False;WaterNormalAnimator;-1;;25;d1a09ea6051e067409434c06db4176c7;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;192;-1526.977,3232.037;Float;False;184;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;253;-1609.41,1929.335;Float;False;1650.793;855.8782;;12;45;43;46;51;185;49;48;47;186;191;187;118;Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;184;-1078.643,1696.364;Float;False;_waterDepthFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;96;-1487.766,3320.425;Float;False;Property;_Distortiondepth;Distortion depth;5;0;Create;True;0;0.2;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;248;-243.6178,1143.808;Float;False;245;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;48;-1493.194,2352.585;Float;False;77;0;1;FLOAT3;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;92;-1387.181,2973.378;Float;False;1;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;240;27.27836,1148.443;Float;False;Noise4;-1;;26;977cfc3455fc1ea46bf9474f4b252364;0;2;27;FLOAT2;0,0;False;28;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;250;2.297788,1397.356;Float;False;246;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-1472.194,2422.585;Float;False;Property;_WaterFresnel;Water Fresnel;2;0;Create;True;0;1.337;1.337;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;2592.575,-1353.292;Float;True;_outNormals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;246;-1241.261,1434.108;Float;False;_foamDepthFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;191;-1527.413,2578.909;Float;False;184;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;195;-1253.977,3238.037;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;187;-1559.41,2670.214;Float;False;Constant;_Float3;Float 3;26;0;Create;True;0;0;1;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;227;40.95115,1289.491;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;194;-1084.977,3239.037;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;94;-1084.665,3000.679;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FresnelNode;47;-1240.195,2357.585;Float;True;Tangent;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;98;-1156.781,3146.276;Float;False;77;0;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;186;-1143.82,2582.848;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;198;209.4109,1873.955;Float;False;1620.144;539.2937;;10;112;180;103;120;104;105;106;111;197;181;Color Distortion Blend;1,1,1,1;0;0
Node;AmplifyShaderEditor.PowerNode;237;320.1151,1287.112;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;235;335.5571,1149.473;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;-889.4374,2359.223;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;233;564.8401,1149.157;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;197;293.2996,2120.387;Float;False;184;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;97;-871.2792,3151.176;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;111;357.816,2207.732;Float;False;Property;_DepthFalloff;Depth Falloff;4;0;Create;True;0;0;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;95;-856.1208,3002.17;Float;False;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;4;-672.9402,709.8762;Float;True;Property;_FoamTexture;Foam Texture;23;0;Create;True;0;None;None;True;white;Auto;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SamplerNode;200;36.02439,707.5018;Float;True;Property;_TextureSample4;Texture Sample 4;27;0;Create;True;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;118;-726.1177,2359.605;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;112;640.802,2124.98;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;45;-1008.853,1979.335;Float;False;Property;_ShallowWaterColor;Shallow Water Color;1;0;Create;True;0;0.1128893,0.5294118,0.4949409,0;0,0.6985294,0.6696247,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;221;83.44057,511.7953;Float;False;Property;_FoamColor;Foam Color;29;0;Create;True;0;1,1,1,0;0.1544118,0.1544118,0.1544118,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;181;517.3168,2298.249;Float;False;Property;_Coloramount;Color amount;25;0;Create;True;0;0;0.4;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;43;-1010.176,2153.749;Float;False;Property;_DeepWaterColor;Deep Water Color;0;0;Create;True;0;0.02319421,0.08088237,0.07849526,0;0,0.1175452,0.2794113,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;99;-621.979,3001.975;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;229;769.0356,1150.363;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;258;827.416,1045.683;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;224;465.1711,687.9551;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;223;123.6163,928.2388;Float;False;Constant;_Color1;Color 1;29;0;Create;True;0;0,0,0,0;0,0,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;180;856.3168,2125.449;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;100;-398.9789,2998.675;Float;False;Global;_WaterGrab;WaterGrab;15;0;Create;True;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;46;-500.6526,1984.635;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;102;-132.0718,2998.568;Float;False;_surfaceDistortion;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;51;-201.6167,1979.394;Float;False;_surfaceColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;103;968.5987,1923.955;Float;False;102;0;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;225;964.1371,910.4149;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;1002.306,2019.254;Float;False;51;0;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;120;1038.727,2126.648;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;255;953.5188,1148.775;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;256;1137.906,910.2848;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;105;1293.345,1930.033;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;202;1962.366,663.0884;Float;True;201;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;52;1963.907,236.7753;Float;True;106;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;53;1962.721,446.7591;Float;True;77;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;127;1942.903,1053.783;Float;False;Property;_AmbientOcclusion;Ambient Occlusion;24;0;Create;True;0;0;0.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;201;1294.674,906.0399;Float;False;_outFoam;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;55;1996.773,956.7152;Float;False;Constant;_Float1;Float 1;15;0;Create;True;0;0.85;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;2009.772,872.3024;Float;False;Constant;_Float0;Float 0;15;0;Create;True;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;170;1999.165,1145.345;Float;False;Constant;_Opacity;Opacity;26;0;Create;True;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;106;1586.554,1924.423;Float;False;_outAlbedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2288.082,427.5931;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;SP/Water/OceanStaticShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;Back;0;0;False;0;0;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;36;0;35;1
WireConnection;36;1;35;3
WireConnection;39;0;36;0
WireConnection;39;1;32;0
WireConnection;16;0;8;0
WireConnection;20;0;10;0
WireConnection;31;0;39;0
WireConnection;15;0;6;0
WireConnection;19;0;9;0
WireConnection;21;0;11;0
WireConnection;23;0;12;0
WireConnection;17;0;7;0
WireConnection;80;1;67;0
WireConnection;80;2;68;0
WireConnection;80;4;69;0
WireConnection;24;0;13;0
WireConnection;81;1;33;0
WireConnection;81;2;28;0
WireConnection;81;4;58;0
WireConnection;27;0;1;0
WireConnection;27;1;81;0
WireConnection;27;5;166;0
WireConnection;157;0;150;0
WireConnection;135;0;133;0
WireConnection;134;0;132;0
WireConnection;71;0;2;0
WireConnection;71;1;80;0
WireConnection;71;5;165;0
WireConnection;25;0;14;0
WireConnection;82;1;84;0
WireConnection;82;2;85;0
WireConnection;82;4;86;0
WireConnection;158;0;156;0
WireConnection;144;0;143;0
WireConnection;83;0;3;0
WireConnection;83;1;82;0
WireConnection;83;5;164;0
WireConnection;140;1;137;0
WireConnection;140;2;138;0
WireConnection;140;4;136;0
WireConnection;72;0;71;0
WireConnection;50;0;27;0
WireConnection;149;0;145;2
WireConnection;149;1;160;0
WireConnection;141;0;3;0
WireConnection;141;1;140;0
WireConnection;141;5;163;0
WireConnection;75;0;74;0
WireConnection;75;1;73;0
WireConnection;75;2;76;0
WireConnection;87;0;83;0
WireConnection;155;0;149;0
WireConnection;155;1;159;0
WireConnection;89;0;75;0
WireConnection;89;1;88;0
WireConnection;89;2;76;0
WireConnection;209;0;208;0
WireConnection;147;0;155;0
WireConnection;109;0;110;0
WireConnection;142;0;141;0
WireConnection;204;0;203;0
WireConnection;146;0;89;0
WireConnection;146;1;148;0
WireConnection;146;2;147;0
WireConnection;245;0;242;0
WireConnection;247;0;228;0
WireConnection;206;1;207;0
WireConnection;206;2;205;0
WireConnection;206;4;210;0
WireConnection;184;0;109;0
WireConnection;240;27;206;0
WireConnection;240;28;248;0
WireConnection;77;0;146;0
WireConnection;246;0;238;0
WireConnection;195;0;192;0
WireConnection;195;1;96;0
WireConnection;227;0;249;0
WireConnection;194;0;195;0
WireConnection;94;0;92;1
WireConnection;94;1;92;2
WireConnection;47;0;48;0
WireConnection;47;3;49;0
WireConnection;186;0;191;0
WireConnection;186;1;187;0
WireConnection;237;0;227;0
WireConnection;237;1;250;0
WireConnection;235;0;240;0
WireConnection;185;0;47;0
WireConnection;185;1;186;0
WireConnection;233;0;235;0
WireConnection;233;1;237;0
WireConnection;97;0;98;0
WireConnection;97;1;194;0
WireConnection;95;0;94;0
WireConnection;95;1;92;4
WireConnection;200;0;4;0
WireConnection;200;1;206;0
WireConnection;118;0;185;0
WireConnection;112;0;197;0
WireConnection;112;1;111;0
WireConnection;99;0;95;0
WireConnection;99;1;97;0
WireConnection;229;0;233;0
WireConnection;258;0;229;0
WireConnection;224;0;221;0
WireConnection;224;1;200;0
WireConnection;180;0;112;0
WireConnection;180;1;181;0
WireConnection;100;0;99;0
WireConnection;46;0;45;0
WireConnection;46;1;43;0
WireConnection;46;2;118;0
WireConnection;102;0;100;0
WireConnection;51;0;46;0
WireConnection;225;0;224;0
WireConnection;225;1;223;0
WireConnection;225;2;258;0
WireConnection;120;0;180;0
WireConnection;256;0;225;0
WireConnection;256;1;255;2
WireConnection;105;0;103;0
WireConnection;105;1;104;0
WireConnection;105;2;120;0
WireConnection;201;0;256;0
WireConnection;106;0;105;0
WireConnection;0;0;52;0
WireConnection;0;1;53;0
WireConnection;0;2;202;0
WireConnection;0;3;54;0
WireConnection;0;4;55;0
WireConnection;0;5;127;0
WireConnection;0;9;170;0
ASEEND*/
//CHKSM=9420CB4E67649C1AA58B631B9B7DBEC6033BA434