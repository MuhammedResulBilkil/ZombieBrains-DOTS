using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GraveyardAuthoring : MonoBehaviour
{
    public GameObject tombstonePrefab;
    public float2 fieldDimensions;
    
    public int numberTombstonesToSpawn;
}

public class GraveyardBaker : Baker<GraveyardAuthoring>
{
    public override void Bake(GraveyardAuthoring authoring)
    {
        Entity graveyardEntity = GetEntity(authoring, TransformUsageFlags.None);
        Entity tombstonePrefab = GetEntity(authoring.tombstonePrefab, TransformUsageFlags.None);
        
        AddComponent(graveyardEntity, new GraveyardComponent
        {
            tombstonePrefab = tombstonePrefab,
            fieldDimensions = authoring.fieldDimensions,
            numberTombstonesToSpawn = authoring.numberTombstonesToSpawn
        });
    }
}
