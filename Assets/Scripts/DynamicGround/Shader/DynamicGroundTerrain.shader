// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DynamicGroundTerrain"
{
	Properties
	{
		_Tesselation("Tesselation", Float) = 30
		[HideInInspector]_TerrainHolesTexture("_TerrainHolesTexture", 2D) = "white" {}
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
		[HideInInspector]_Smoothness0("Smoothness0", Range( 0 , 1)) = 1
		[HideInInspector]_Smoothness2("Smoothness2", Range( 0 , 1)) = 1
		[HideInInspector][Gamma]_Metallic0("Metallic0", Range( 0 , 1)) = 0
		[HideInInspector][Gamma]_Metallic2("Metallic2", Range( 0 , 1)) = 0
		[HideInInspector][Gamma]_Metallic3("Metallic3", Range( 0 , 1)) = 0
		[HideInInspector][Gamma]_Metallic1("Metallic1", Range( 0 , 1)) = 0
		[HideInInspector]_Mask2("_Mask2", 2D) = "white" {}
		[HideInInspector]_Mask0("_Mask0", 2D) = "white" {}
		[HideInInspector]_Mask1("_Mask1", 2D) = "white" {}
		[HideInInspector]_Mask3("_Mask3", 2D) = "white" {}
		[PerRendererData]_DeformMap("DeformMap", 2D) = "white" {}
		[PerRendererData]_DeformHeight("DeformHeight", Float) = 1
		_MaxEdgeHeight("MaxEdgeHeight", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma multi_compile_local __ _ALPHATEST_ON
		#pragma shader_feature_local _MASKMAP
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Mask1;
		uniform sampler2D _Mask0;
		uniform sampler2D _Mask3;
		uniform sampler2D _Mask2;
		uniform float4 _MaskMapRemapScale1;
		uniform float4 _MaskMapRemapScale0;
		uniform float4 _MaskMapRemapOffset1;
		uniform float4 _MaskMapRemapOffset2;
		uniform float4 _MaskMapRemapScale2;
		uniform float4 _MaskMapRemapScale3;
		uniform float4 _MaskMapRemapOffset0;
		uniform float4 _MaskMapRemapOffset3;
		uniform sampler2D _DeformMap;
		uniform float4 _DeformMap_ST;
		uniform float _MaxEdgeHeight;
		uniform float _DeformHeight;
		uniform sampler2D _Control;
		uniform float4 _Control_ST;
		uniform sampler2D _Normal0;
		uniform sampler2D _Splat0;
		uniform float4 _Splat0_ST;
		uniform sampler2D _Normal1;
		uniform sampler2D _Splat1;
		uniform float4 _Splat1_ST;
		uniform sampler2D _Normal2;
		uniform sampler2D _Splat2;
		uniform float4 _Splat2_ST;
		uniform sampler2D _Normal3;
		uniform sampler2D _Splat3;
		uniform float4 _Splat3_ST;
		uniform float _Smoothness0;
		uniform float _Smoothness1;
		uniform float _Smoothness2;
		uniform float _Smoothness3;
		uniform sampler2D _TerrainHolesTexture;
		uniform float4 _TerrainHolesTexture_ST;
		uniform float _Metallic0;
		uniform float _Metallic1;
		uniform float _Metallic2;
		uniform float _Metallic3;
		uniform float _Tesselation;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, 10.0,25.0,_Tesselation);
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float2 _DeformXZoffset = float2(0,0);
			float2 uv_DeformMap = v.texcoord * _DeformMap_ST.xy + _DeformMap_ST.zw;
			float4 tex2DNode1_g4 = tex2Dlod( _DeformMap, float4( uv_DeformMap, 0, 0.0) );
			float4 appendResult3_g4 = (float4(_DeformXZoffset.x , ( tex2DNode1_g4.r + ( ( tex2DNode1_g4.r * tex2DNode1_g4.g ) * _MaxEdgeHeight ) ) , _DeformXZoffset.y , 0.0));
			float4 temp_output_7_0 = ( appendResult3_g4 * _DeformHeight );
			v.vertex.xyz += temp_output_7_0.xyz;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Control = i.uv_texcoord * _Control_ST.xy + _Control_ST.zw;
			float4 tex2DNode5_g5 = tex2D( _Control, uv_Control );
			float dotResult20_g5 = dot( tex2DNode5_g5 , float4(1,1,1,1) );
			float SplatWeight22_g5 = dotResult20_g5;
			float localSplatClip74_g5 = ( SplatWeight22_g5 );
			float SplatWeight74_g5 = SplatWeight22_g5;
			{
			#if !defined(SHADER_API_MOBILE) && defined(TERRAIN_SPLAT_ADDPASS)
				clip(SplatWeight74_g5 == 0.0f ? -1 : 1);
			#endif
			}
			float4 SplatControl26_g5 = ( tex2DNode5_g5 / ( localSplatClip74_g5 + 0.001 ) );
			float4 temp_output_59_0_g5 = SplatControl26_g5;
			float2 uv_Splat0 = i.uv_texcoord * _Splat0_ST.xy + _Splat0_ST.zw;
			float2 uv_Splat1 = i.uv_texcoord * _Splat1_ST.xy + _Splat1_ST.zw;
			float2 uv_Splat2 = i.uv_texcoord * _Splat2_ST.xy + _Splat2_ST.zw;
			float2 uv_Splat3 = i.uv_texcoord * _Splat3_ST.xy + _Splat3_ST.zw;
			float4 weightedBlendVar8_g5 = temp_output_59_0_g5;
			float4 weightedBlend8_g5 = ( weightedBlendVar8_g5.x*tex2D( _Normal0, uv_Splat0 ) + weightedBlendVar8_g5.y*tex2D( _Normal1, uv_Splat1 ) + weightedBlendVar8_g5.z*tex2D( _Normal2, uv_Splat2 ) + weightedBlendVar8_g5.w*tex2D( _Normal3, uv_Splat3 ) );
			float3 temp_output_61_0_g5 = UnpackNormal( weightedBlend8_g5 );
			o.Normal = temp_output_61_0_g5;
			float4 appendResult33_g5 = (float4(1.0 , 1.0 , 1.0 , _Smoothness0));
			float4 tex2DNode4_g5 = tex2D( _Splat0, uv_Splat0 );
			float4 appendResult36_g5 = (float4(1.0 , 1.0 , 1.0 , _Smoothness1));
			float4 tex2DNode3_g5 = tex2D( _Splat1, uv_Splat1 );
			float4 appendResult39_g5 = (float4(1.0 , 1.0 , 1.0 , _Smoothness2));
			float4 tex2DNode6_g5 = tex2D( _Splat2, uv_Splat2 );
			float4 appendResult42_g5 = (float4(1.0 , 1.0 , 1.0 , _Smoothness3));
			float4 tex2DNode7_g5 = tex2D( _Splat3, uv_Splat3 );
			float4 weightedBlendVar9_g5 = temp_output_59_0_g5;
			float4 weightedBlend9_g5 = ( weightedBlendVar9_g5.x*( appendResult33_g5 * tex2DNode4_g5 ) + weightedBlendVar9_g5.y*( appendResult36_g5 * tex2DNode3_g5 ) + weightedBlendVar9_g5.z*( appendResult39_g5 * tex2DNode6_g5 ) + weightedBlendVar9_g5.w*( appendResult42_g5 * tex2DNode7_g5 ) );
			float4 MixDiffuse28_g5 = weightedBlend9_g5;
			float4 temp_output_60_0_g5 = MixDiffuse28_g5;
			float4 localClipHoles100_g5 = ( temp_output_60_0_g5 );
			float2 uv_TerrainHolesTexture = i.uv_texcoord * _TerrainHolesTexture_ST.xy + _TerrainHolesTexture_ST.zw;
			float holeClipValue99_g5 = tex2D( _TerrainHolesTexture, uv_TerrainHolesTexture ).r;
			float Hole100_g5 = holeClipValue99_g5;
			{
			#ifdef _ALPHATEST_ON
				clip(Hole100_g5 == 0.0f ? -1 : 1);
			#endif
			}
			o.Albedo = localClipHoles100_g5.xyz;
			float4 appendResult55_g5 = (float4(_Metallic0 , _Metallic1 , _Metallic2 , _Metallic3));
			float dotResult53_g5 = dot( SplatControl26_g5 , appendResult55_g5 );
			o.Metallic = dotResult53_g5;
			float4 appendResult205_g5 = (float4(_Smoothness0 , _Smoothness1 , _Smoothness2 , _Smoothness3));
			float4 appendResult206_g5 = (float4(tex2DNode4_g5.a , tex2DNode3_g5.a , tex2DNode6_g5.a , tex2DNode7_g5.a));
			float4 defaultSmoothness210_g5 = ( appendResult205_g5 * appendResult206_g5 );
			float dotResult216_g5 = dot( defaultSmoothness210_g5 , SplatControl26_g5 );
			o.Smoothness = dotResult216_g5;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
816.6667;568.6667;1293.333;660.3334;1101.187;56.77037;1.103127;True;False
Node;AmplifyShaderEditor.RangedFloatNode;10;-587.4973,513.1779;Inherit;False;Property;_Tesselation;Tesselation;0;0;Create;True;0;0;0;False;0;False;30;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;7;-535.7131,369.9892;Inherit;False;Deformer_Node;25;;4;aea2c7177e8debd498a364527bbb05f7;0;0;3;FLOAT;10;FLOAT;9;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;4;-214.5456,251.6473;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;1;-822.1517,-10.35818;Inherit;False;Four Splats First Pass Terrain;1;;5;37452fdfb732e1443b7e39720d05b708;2,85,0,102,1;7;59;FLOAT4;0,0,0,0;False;60;FLOAT4;0,0,0,0;False;61;FLOAT3;0,0,0;False;57;FLOAT;0;False;58;FLOAT;0;False;201;FLOAT;0;False;62;FLOAT;0;False;7;FLOAT4;0;FLOAT3;14;FLOAT;56;FLOAT;45;FLOAT;200;FLOAT;19;FLOAT3;17
Node;AmplifyShaderEditor.DistanceBasedTessNode;9;-414.9642,513.9625;Inherit;False;3;0;FLOAT;10;False;1;FLOAT;10;False;2;FLOAT;25;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-2.206253,-5.870713;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;DynamicGroundTerrain;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;50;50;100;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;1;17
WireConnection;4;1;7;0
WireConnection;9;0;10;0
WireConnection;0;0;1;0
WireConnection;0;1;1;14
WireConnection;0;3;1;56
WireConnection;0;4;1;45
WireConnection;0;11;7;0
WireConnection;0;14;9;0
ASEEND*/
//CHKSM=B1B336656439F57F24A1B79AF09C55CF4AEFBD87