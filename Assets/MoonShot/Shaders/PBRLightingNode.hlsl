#ifndef PBRLightingNode_INCLUDED
#define PBRLightingNode_INCLUDED

#if !SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

void PBRLightingNode_float(float3 PositionWS, float3 NormalWS, float3 ViewDirWS, float3 Albedo, float Metallic, float Smoothness, float Occlusion, float3 Emission, float AlphaIn, out float3 ColorOut, out float AlphaOut)
{
#if SHADERGRAPH_PREVIEW
	ColorOut = float3(1,0,0);
	AlphaOut = 1.0;
#else
    InputData inputData = (InputData)0;

#if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
    inputData.positionWS = PositionWS;
#endif

    inputData.normalWS = NormalWS;

    inputData.viewDirectionWS = ViewDirWS;

    inputData.shadowCoord = float4(0, 0, 0, 0);

    inputData.fogCoord = 0.0f;
    inputData.vertexLighting = float3(0,0,0);
    inputData.bakedGI = SAMPLE_GI(float2(0,0), float3(1,1,1), inputData.normalWS);

    half4 colorInt = UniversalFragmentPBR(inputData, Albedo, Metallic, float3(0,0,0), Smoothness, Occlusion, Emission, AlphaIn);

    ColorOut = colorInt.rgb;
    AlphaOut = colorInt.a;
#endif
}
#endif