#ifndef PBRLightingNode_INCLUDED
#define PBRLightingNode_INCLUDED

#if !SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

void PBRDirectionalLightNode_float(float3 NormalWS, float3 ViewDirWS, float3 Albedo, float Metallic, float Smoothness, float3 LightDirection, float3 LightColor, float3 ColorIn, out float3 ColorOut)
{
#if SHADERGRAPH_PREVIEW
	ColorOut = float3(1,0,0);
#else
    BRDFData brdfData;
    InitializeBRDFData(Albedo, Metallic, float3(0,0,0), Smoothness, 1.0f, brdfData);

	float3 color = ColorIn;

    Light light;
    light.direction = LightDirection;
    light.distanceAttenuation = 1.0f;
    light.shadowAttenuation = 1.0f;
    light.color = LightColor;
    ColorOut = ColorIn + LightingPhysicallyBased(brdfData, light, NormalWS, ViewDirWS);
#endif
}
#endif