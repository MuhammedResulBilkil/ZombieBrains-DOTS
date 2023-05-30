using Unity.Entities;
using Unity.Mathematics;

public struct GraveyardComponent : IComponentData
{
    public Entity tombstonePrefab;
    public Entity zombiePrefab;
    public float2 fieldDimensions;
    
    public int numberTombstonesToSpawn;
    public float zombieSpawnRate;
}
