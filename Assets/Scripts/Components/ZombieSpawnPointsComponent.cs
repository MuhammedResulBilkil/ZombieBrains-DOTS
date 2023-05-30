using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public struct ZombieSpawnPointsComponent : IComponentData
{
    public BlobAssetReference<ZombieSpawnPointsBlob> value;
}

public struct ZombieSpawnPointsBlob
{
    public BlobArray<float3> value;
}