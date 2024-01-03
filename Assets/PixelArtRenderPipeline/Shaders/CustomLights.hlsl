 struct DirectionalLightData
 {
     float4 directionToLight;
     float4 color;
     float intensity;
 };
            
struct PointLightData
{
    float4 position;
    float4 color;
    float range;
    float intensity;
};

StructuredBuffer<DirectionalLightData> _DirectionalLightDataBuffer;
StructuredBuffer<PointLightData> _PointLightDataBuffer;
int _PointLightCount;
int _DirectionalLightCount;