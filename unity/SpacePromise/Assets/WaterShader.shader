// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "WaterShader"
{
	Properties
	{
		_DeepColor("DeepColor", Color) = (0.0480104,0.3388318,0.5441177,0)
		_ShallowColor("ShallowColor", Color) = (0.08455882,0.6029921,0.6764706,0)
		_Float1("Float 1", Float) = 16
		_Vector0("Vector 0", Vector) = (0,1,0,0)
		_Float0("Float 0", Range( 0 , 4)) = 0
		_TextureSample2("Texture Sample 2", 2D) = "bump" {}
		_WaveNormalLerp("WaveNormalLerp", Float) = 0
		_Float2("Float 2", Range( 0 , 10)) = 0
		_Vector1("Vector 1", Vector) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			INTERNAL_DATA
			float3 worldNormal;
		};

		uniform sampler2D _TextureSample2;
		uniform float2 _Vector1;
		uniform float _Float1;
		uniform float _Float2;
		uniform float _WaveNormalLerp;
		uniform float4 _ShallowColor;
		uniform float4 _DeepColor;
		uniform float3 _Vector0;
		uniform float _Float0;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 temp_cast_0 = (_Float1).xx;
			float2 uv_TexCoord166 = i.uv_texcoord * temp_cast_0 + float2( 0,0 );
			float2 panner163 = ( uv_TexCoord166 + _Time.x * _Vector1);
			float2 panner164 = ( uv_TexCoord166 + _Time.y * _Vector1);
			float3 lerpResult156 = lerp( UnpackNormal( tex2D( _TextureSample2, panner163 ) ) , UnpackScaleNormal( tex2D( _TextureSample2, panner164 ) ,_Float2 ) , _WaveNormalLerp);
			float3 Normal160 = lerpResult156;
			o.Normal = Normal160;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float fresnelNDotV150 = dot( WorldNormalVector( i , _Vector0 ), ase_worldViewDir );
			float fresnelNode150 = ( 0.0 + 1.0 * pow( 1.0 - fresnelNDotV150, _Float0 ) );
			float4 lerpResult3 = lerp( _ShallowColor , _DeepColor , fresnelNode150);
			o.Albedo = lerpResult3.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14201
694;100;939;910;3998.146;2873.907;3.471338;True;False
Node;AmplifyShaderEditor.RangedFloatNode;167;-3214.365,-1573.769;Float;False;Property;_Float1;Float 1;9;0;Create;16;16;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;166;-2998.368,-1559.736;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;168;-2966.32,-1406.084;Float;False;Property;_Vector1;Vector 1;16;0;Create;0,0;0.1,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TimeNode;165;-2989.991,-1239.995;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;163;-2644.224,-1541.481;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;162;-2465.158,-1224.426;Float;False;Property;_Float2;Float 2;15;0;Create;0;1.75933;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;164;-2646.05,-1382.664;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;159;-1963.303,-1068.12;Float;False;Property;_WaveNormalLerp;WaveNormalLerp;14;0;Create;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;157;-2047.84,-1496.646;Float;True;Property;_TextureSample1;Texture Sample 1;13;0;Create;None;None;True;0;False;white;Auto;True;Instance;158;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;153;-1911.806,-375.7766;Float;False;Property;_Vector0;Vector 0;11;0;Create;0,1,0;0,0,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;154;-1998.806,-148.7766;Float;False;Property;_Float0;Float 0;12;0;Create;0;1.336;0;4;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;158;-2049.847,-1273.357;Float;True;Property;_TextureSample2;Texture Sample 2;13;0;Create;None;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;150;-1674.752,-370.2794;Float;True;Tangent;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.0;False;2;FLOAT;1.0;False;3;FLOAT;5.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-1632.381,-724.8846;Float;False;Property;_ShallowColor;ShallowColor;1;0;Create;0.08455882,0.6029921,0.6764706,0;0.184256,0.338981,0.3529412,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;2;-1627.319,-548.4467;Float;False;Property;_DeepColor;DeepColor;0;0;Create;0.0480104,0.3388318,0.5441177,0;0.09299307,0.1776041,0.2941176,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;156;-1590.932,-1477.679;Float;False;3;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0.0,0,0,0;False;2;FLOAT;0.0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;152;-231.8059,160.2234;Float;False;Constant;_Metalic;Metalic;9;0;Create;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;75;-865.5286,526.9158;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0.0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;161;-61.16016,-149.0494;Float;False;160;0;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;122;-2197.415,1006.106;Float;False;2;2;0;FLOAT;1.0;False;1;FLOAT;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;3;-1254.235,-514.8824;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0.0,0,0,0;False;2;FLOAT;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;119;-2314.731,-381.3221;Float;False;DepthFull;-1;True;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;160;-1344.896,-1483.536;Float;False;Normal;-1;True;1;0;FLOAT3;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.BlendNormalsNode;67;-1154.926,840.0511;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;74;-735.5289,528.2158;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;103;-2542.842,1099.885;Float;False;101;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-1840.659,979.7979;Float;False;Property;_WaveScale;WaveScale;5;0;Create;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;121;-838.8026,99.9423;Float;False;119;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-864.2287,649.1159;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT3;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;-1863.74,-2143.092;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;58;-1850.733,703.5548;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;20;-3148.732,-355.515;Float;False;Property;_DepthFalloff;DepthFalloff;3;0;Create;0.25;0.25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;101;-1583.247,-2073.188;Float;False;WaveSpeedNormalized;-1;True;1;0;FLOAT;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-2175.84,-2024.73;Float;False;Property;_WaveSpeed;WaveSpeed;2;0;Create;1.117647;0.5;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;62;-2175.399,706.8046;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;59;-1858.209,844.9261;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1.0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;27;-3123.204,-456.5042;Float;False;Property;_Depth;Depth;4;0;Create;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;77;-1233.428,477.5157;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;76;-1011.129,498.3155;Float;False;FLOAT4;4;0;FLOAT;0.0;False;1;FLOAT;0.0;False;2;FLOAT;0.0;False;3;FLOAT;0.0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ScreenColorNode;78;-571.3226,519.7881;Float;False;Global;_GrabScreen1;Grab Screen 1;7;0;Create;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PowerNode;54;-2677.693,-374.6878;Float;False;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;55;-2489.49,-373.5479;Float;False;1;0;FLOAT;0.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;40;-580.7164,45.30162;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DepthFade;53;-2919.401,-453.143;Float;False;True;1;0;FLOAT;1.0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;151;-354.8059,250.2234;Float;False;Property;_Glossiness;Glossiness;10;0;Create;0;0.95;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-1085.028,644.7158;Float;False;Constant;_WaveDistortion;WaveDistortion;7;0;Create;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenColorNode;39;-841.6456,-143.5676;Float;False;Global;_GrabScreen0;Grab Screen 0;0;0;Create;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;66;-1544.917,898.5496;Float;True;Property;_WaveTexture;WaveTexture;6;0;Create;None;dd2fd2df93418444c8e280f1d34deeb5;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;70;-2467.079,942.1393;Float;False;Property;_WaveDirection;WaveDirection;7;0;Create;1,1;0.2,0.2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;71;-2406.879,729.7393;Float;False;Property;_WaveDensity;WaveDensity;8;0;Create;16;16;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TimeNode;125;-2105.74,-2193.092;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;65;-1541.667,698.6801;Float;True;Property;_TextureSample0;Texture Sample 0;10;0;Create;None;None;True;0;False;white;Auto;False;Instance;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;WaterShader;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;Back;0;0;False;0;0;Transparent;0.5;True;False;0;False;Transparent;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;False;2;SrcAlpha;OneMinusSrcAlpha;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;5;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;FLOAT;0.0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
Node;AmplifyShaderEditor.CommentaryNode;97;-2220.839,-2254.263;Float;False;914.394;362.5317;Comment;0;Wave Speed;1,1,1,1;0;0
WireConnection;166;0;167;0
WireConnection;163;0;166;0
WireConnection;163;2;168;0
WireConnection;163;1;165;1
WireConnection;164;0;166;0
WireConnection;164;2;168;0
WireConnection;164;1;165;2
WireConnection;157;1;163;0
WireConnection;158;1;164;0
WireConnection;158;5;162;0
WireConnection;150;0;153;0
WireConnection;150;3;154;0
WireConnection;156;0;157;0
WireConnection;156;1;158;0
WireConnection;156;2;159;0
WireConnection;75;0;76;0
WireConnection;75;1;77;4
WireConnection;122;1;103;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;3;2;150;0
WireConnection;119;0;55;0
WireConnection;160;0;156;0
WireConnection;67;0;65;0
WireConnection;67;1;66;0
WireConnection;74;0;75;0
WireConnection;74;1;72;0
WireConnection;72;0;73;0
WireConnection;72;1;67;0
WireConnection;123;0;125;2
WireConnection;123;1;99;0
WireConnection;58;0;62;0
WireConnection;58;2;70;0
WireConnection;58;1;122;0
WireConnection;101;0;123;0
WireConnection;62;0;71;0
WireConnection;59;0;62;0
WireConnection;59;2;70;0
WireConnection;59;1;122;0
WireConnection;76;0;77;1
WireConnection;76;1;77;2
WireConnection;78;0;74;0
WireConnection;54;0;53;0
WireConnection;54;1;20;0
WireConnection;55;0;54;0
WireConnection;40;0;39;0
WireConnection;40;2;121;0
WireConnection;53;0;27;0
WireConnection;66;1;59;0
WireConnection;66;5;63;0
WireConnection;65;1;58;0
WireConnection;0;0;3;0
WireConnection;0;1;161;0
ASEEND*/
//CHKSM=3D86E17D633D7D44E9817D551737CFC2BB95A34A