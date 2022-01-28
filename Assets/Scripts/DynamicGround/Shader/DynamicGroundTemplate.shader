// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "KrKr/DynamicGroundTemplate"
{
	Properties
	{
		_TopTexture0("Top Texture 0", 2D) = "white" {}
		_OffsetMax("OffsetMax", Float) = 0
		_Color0("Color 0", Color) = (1,1,1,0)
		_TextureSample3("Texture Sample 3", 2D) = "white" {}
		_TextureSample4("Texture Sample 4", 2D) = "white" {}
		_TextureSample5("Texture Sample 5", 2D) = "white" {}
		_AntiTiling1("AntiTiling1", Float) = 0
		_AntiTiling2("AntiTiling2", Float) = 0
		_AntiTiling3("AntiTiling3", Float) = 0
		_BlendPower("BlendPower", Float) = 0
		_Tiling("Tiling", Float) = 1
		[PerRendererData]_DeformMap("DeformMap", 2D) = "white" {}
		[PerRendererData]_DeformHeight("DeformHeight", Float) = 1
		_MaxEdgeHeight("MaxEdgeHeight", Float) = 0
		_Tesselation("Tesselation", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows exclude_path:deferred vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _DeformMap;
		uniform float4 _DeformMap_ST;
		uniform float _MaxEdgeHeight;
		uniform float _DeformHeight;
		uniform float _OffsetMax;
		uniform sampler2D _TextureSample3;
		uniform float _AntiTiling1;
		uniform sampler2D _TextureSample4;
		uniform float _AntiTiling2;
		uniform sampler2D _TextureSample5;
		uniform float _AntiTiling3;
		uniform float _BlendPower;
		uniform float4 _Color0;
		sampler2D _TopTexture0;
		uniform float _Tiling;
		uniform float _Tesselation;


		inline float4 TriplanarSampling119( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
		}


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, 10.0,25.0,_Tesselation);
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float2 _DeformXZoffset = float2(0,0);
			float2 uv_DeformMap = v.texcoord * _DeformMap_ST.xy + _DeformMap_ST.zw;
			float4 tex2DNode1_g22 = tex2Dlod( _DeformMap, float4( uv_DeformMap, 0, 0.0) );
			float4 appendResult3_g22 = (float4(_DeformXZoffset.x , ( tex2DNode1_g22.r + ( ( tex2DNode1_g22.r * tex2DNode1_g22.g ) * _MaxEdgeHeight ) ) , _DeformXZoffset.y , 0.0));
			float4 break166 = ( appendResult3_g22 * _DeformHeight );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float4 transform158 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float temp_output_1_0_g21 = 1.0;
			float clampResult183 = clamp( ( ( distance( transform158 , float4( _WorldSpaceCameraPos , 0.0 ) ) - temp_output_1_0_g21 ) / ( 0.0 - temp_output_1_0_g21 ) ) , 0.0 , 1.0 );
			float clampResult169 = clamp( ( transform158.y - ( -1.0 + _WorldSpaceCameraPos.y ) ) , 0.0 , 1.0 );
			float3 appendResult167 = (float3(break166.x , ( break166.y - ( _OffsetMax * ( clampResult183 * clampResult169 ) ) ) , break166.z));
			v.vertex.xyz += appendResult167;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float clampResult242 = clamp( ( ( tex2D( _TextureSample3, ( i.uv_texcoord * _AntiTiling1 ) ).r * tex2D( _TextureSample4, ( i.uv_texcoord * _AntiTiling2 ) ).r * tex2D( _TextureSample5, ( i.uv_texcoord * _AntiTiling3 ) ).r ) + _BlendPower ) , 0.0 , 1.0 );
			float2 temp_cast_0 = (_Tiling).xx;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float4 triplanar119 = TriplanarSampling119( _TopTexture0, ase_worldPos, ase_worldNormal, 1.0, temp_cast_0, 1.0, 0 );
			float4 temp_output_124_0 = ( _Color0 * triplanar119 );
			float4 temp_output_231_0 = ( clampResult242 * temp_output_124_0 );
			o.Albedo = temp_output_231_0.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18900
1242.667;698.6667;1293.333;660.3334;788.8406;165.0487;1;True;False
Node;AmplifyShaderEditor.PosVertexDataNode;154;-2425.544,1028.684;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldSpaceCameraPos;153;-2282.397,1218.586;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;158;-2232.476,1029.1;Inherit;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;227;-2305.946,-1666.618;Inherit;False;Property;_AntiTiling2;AntiTiling2;15;0;Create;True;0;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;213;-2368,-2048;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;219;-2371.197,-1534.251;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;217;-2368,-1792;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;226;-2295.11,-1410.229;Inherit;False;Property;_AntiTiling3;AntiTiling3;16;0;Create;True;0;0;0;False;0;False;0;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;228;-2311.358,-1921.206;Inherit;False;Property;_AntiTiling1;AntiTiling1;14;0;Create;True;0;0;0;False;0;False;0;0.57;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;-2002.002,1243.103;Inherit;False;2;2;0;FLOAT;-1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;170;-1928.206,938.4674;Inherit;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;171;-1735.542,890.222;Inherit;False;Inverse Lerp;-1;;21;09cbe79402f023141a4dc1fddd4c9511;0;3;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;224;-2044.639,-1792;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;182;-1857.93,1157.746;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;225;-2045.938,-1538.434;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;223;-2044.187,-2050.056;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;221;-1792,-1792;Inherit;True;Property;_TextureSample4;Texture Sample 4;12;0;Create;True;0;0;0;False;0;False;-1;None;27e669d9237cab44f8fa8954df3ca4a6;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;222;-1792,-1600;Inherit;True;Property;_TextureSample5;Texture Sample 5;13;0;Create;True;0;0;0;False;0;False;-1;None;2e7106498645dd24387cf309bcfe795d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;220;-1792,-1984;Inherit;True;Property;_TextureSample3;Texture Sample 3;11;0;Create;True;0;0;0;False;0;False;-1;None;a2ec7ba0a0a99ad4cb83b9a0a5517e3b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;169;-1670.827,1154.823;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;183;-1540.72,852.7632;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-2225.75,-896.8969;Inherit;False;Property;_Tiling;Tiling;18;0;Create;True;0;0;0;False;0;False;1;0.37;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;172;-1381.52,851.9203;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;179;-1564.476,745.3073;Inherit;False;Property;_OffsetMax;OffsetMax;8;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;195;-1528.809,349.2281;Inherit;True;Deformer_Node;19;;22;aea2c7177e8debd498a364527bbb05f7;0;0;3;FLOAT;10;FLOAT;9;FLOAT4;0
Node;AmplifyShaderEditor.WorldPosInputsNode;40;-2614.037,-978.5355;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;229;-1392.698,-1786.394;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;236;-1389.824,-1446.513;Inherit;False;Property;_BlendPower;BlendPower;17;0;Create;True;0;0;0;False;0;False;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;123;-1942.985,-1241.913;Inherit;False;Property;_Color0;Color 0;9;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TriplanarNode;119;-1988.509,-1003.896;Inherit;True;Spherical;World;False;Top Texture 0;_TopTexture0;white;0;None;Mid Texture 0;_MidTexture0;white;1;None;Bot Texture 0;_BotTexture0;white;0;None;UpLayer;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;241;-1127.71,-1525.949;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;184;-1212.759,745.1348;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;166;-1114.251,398.1208;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;124;-1442.211,-1031.6;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;242;-947.9282,-1425.895;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;92;-425.9318,592.8239;Inherit;False;Property;_Tesselation;Tesselation;23;0;Create;True;0;0;0;False;0;False;1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;161;-876.4487,421.1508;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;211;-1501.902,-112.4279;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0.1;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceBasedTessNode;118;-264.1267,597.5473;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;10;False;2;FLOAT;25;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;44;-2599.976,-642.8505;Inherit;False;Constant;_UVoffset;UVoffset;6;0;Create;True;0;0;0;False;0;False;10,10;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-1994.447,-688.4757;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;41;-2348.026,-843.2529;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;45;-2168.113,-664.0152;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;174;-1075.411,1018.731;Inherit;False;Constant;_Color1;Color 1;10;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;22;-871.7517,-310.9308;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;-834.6406,796.8156;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;167;-432.6666,289.9151;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;122;-423.3203,-26.30854;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;23;-1802.948,-714.2007;Inherit;True;Property;_DownLayer;DownLayer;6;0;Create;True;0;0;0;False;0;False;-1;None;23d8a1d6b915eb747b41db46131577ab;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;137;-1085.521,129.7154;Inherit;False;Inverse Lerp;-1;;26;09cbe79402f023141a4dc1fddd4c9511;0;3;1;FLOAT;57.63;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;209;-1332.681,-303.2172;Inherit;True;Property;_TextureSample1;Texture Sample 1;10;0;Create;True;0;0;0;False;0;False;-1;None;c65f7611603ac0349b3d70dd2f4935a4;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;164;-533.4438,789.0709;Inherit;False;Constant;_Vector1;Vector 1;10;0;Create;True;0;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LerpOp;208;-661.0206,-172.1449;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;212;-1767.027,-1.9807;Inherit;False;Constant;_Float0;Float 0;13;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-1320.186,80.6617;Inherit;False;Property;_LayerThreshold;LayerThreshold;7;0;Create;True;0;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;140;-916.7974,129.661;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;210;-929.7657,-80.59239;Inherit;False;Inverse Lerp;-1;;25;09cbe79402f023141a4dc1fddd4c9511;0;3;1;FLOAT;1;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;231;-812.5237,-1180.379;Inherit;False;2;2;0;FLOAT;1;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;125;-624.0906,101.6931;Inherit;False;Sparkles;1;;24;bc5e3522acbfc1f43bb0bfa924a3f558;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;178;-1075.534,918.847;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,2.735751;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;KrKr/DynamicGroundTemplate;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;5;True;True;0;False;Opaque;;Geometry;ForwardOnly;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;0;10;10;25;False;1;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;0;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;158;0;154;0
WireConnection;185;1;153;2
WireConnection;170;0;158;0
WireConnection;170;1;153;0
WireConnection;171;3;170;0
WireConnection;224;0;217;0
WireConnection;224;1;227;0
WireConnection;182;0;158;2
WireConnection;182;1;185;0
WireConnection;225;0;219;0
WireConnection;225;1;226;0
WireConnection;223;0;213;0
WireConnection;223;1;228;0
WireConnection;221;1;224;0
WireConnection;222;1;225;0
WireConnection;220;1;223;0
WireConnection;169;0;182;0
WireConnection;183;0;171;0
WireConnection;172;0;183;0
WireConnection;172;1;169;0
WireConnection;229;0;220;1
WireConnection;229;1;221;1
WireConnection;229;2;222;1
WireConnection;119;9;40;0
WireConnection;119;3;43;0
WireConnection;241;0;229;0
WireConnection;241;1;236;0
WireConnection;184;0;179;0
WireConnection;184;1;172;0
WireConnection;166;0;195;0
WireConnection;124;0;123;0
WireConnection;124;1;119;0
WireConnection;242;0;241;0
WireConnection;161;0;166;1
WireConnection;161;1;184;0
WireConnection;211;0;195;9
WireConnection;211;1;83;0
WireConnection;211;2;195;10
WireConnection;211;3;195;10
WireConnection;211;4;212;0
WireConnection;118;0;92;0
WireConnection;42;0;43;0
WireConnection;42;1;45;0
WireConnection;41;0;40;1
WireConnection;41;1;40;3
WireConnection;45;0;41;0
WireConnection;45;1;44;0
WireConnection;22;0;124;0
WireConnection;22;1;23;0
WireConnection;22;2;140;0
WireConnection;175;0;178;0
WireConnection;175;1;174;1
WireConnection;167;0;166;0
WireConnection;167;1;161;0
WireConnection;167;2;166;2
WireConnection;122;0;208;0
WireConnection;122;1;125;0
WireConnection;23;1;42;0
WireConnection;137;1;83;0
WireConnection;137;3;195;9
WireConnection;208;0;231;0
WireConnection;208;1;22;0
WireConnection;208;2;210;0
WireConnection;140;0;137;0
WireConnection;210;3;211;0
WireConnection;231;0;242;0
WireConnection;231;1;124;0
WireConnection;178;0;169;0
WireConnection;0;0;231;0
WireConnection;0;11;167;0
WireConnection;0;14;118;0
ASEEND*/
//CHKSM=7F97BF8ACD73327E1A72BFEE57AF395A77B3C6AA