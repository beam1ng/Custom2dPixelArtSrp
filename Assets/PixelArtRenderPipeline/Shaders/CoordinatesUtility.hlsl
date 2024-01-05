#include <UnityShaderVariables.cginc>

float3 ReconstructWsFromOrthoDepth(float depth, float2 uv)
{
    float depthWs = depth;
    #if UNITY_REVERSED_Z
    depthWs = 1 - depthWs;
    #endif

    float positionZ = _WorldSpaceCameraPos.z + depthWs * (_ProjectionParams.z - _ProjectionParams.y) +_ProjectionParams.y;
    float3 positionWs = float3(unity_OrthoParams.xy * (uv * 2 - 1) + _WorldSpaceCameraPos.xy, positionZ);

    return positionWs;
}
