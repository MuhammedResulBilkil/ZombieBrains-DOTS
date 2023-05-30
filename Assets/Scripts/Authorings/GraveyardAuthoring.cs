using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class GraveyardAuthoring : MonoBehaviour
{
    public GameObject tombstonePrefab;
    public GameObject zombiePrefab;
    public float2 fieldDimensions;
    
    public float zombieSpawnRate;
    public int numberTombstonesToSpawn;
    public uint randomSeed;
}

public class GraveyardBaker : Baker<GraveyardAuthoring>
{
    public override void Bake(GraveyardAuthoring authoring)
    {
        Entity graveyardEntity = GetEntity(authoring, TransformUsageFlags.Dynamic);
        Entity tombstonePrefab = GetEntity(authoring.tombstonePrefab, TransformUsageFlags.Dynamic);
        Entity zombiePrefab = GetEntity(authoring.zombiePrefab, TransformUsageFlags.Dynamic);
        
        AddComponent(graveyardEntity, new GraveyardComponent
        {
            tombstonePrefab = tombstonePrefab,
            zombiePrefab = zombiePrefab,
            zombieSpawnRate = authoring.zombieSpawnRate,
            fieldDimensions = authoring.fieldDimensions,
            numberTombstonesToSpawn = authoring.numberTombstonesToSpawn
        });
        
        AddComponent(graveyardEntity, new GraveyardRandomComponent
        {
            value = Random.CreateFromIndex(authoring.randomSeed)
        });
        
        AddComponent<ZombieSpawnPointsComponent>(graveyardEntity);
        AddComponent<ZombieSpawnTimerComponent>(graveyardEntity);
    }
}
