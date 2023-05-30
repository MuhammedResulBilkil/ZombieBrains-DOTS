using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawnTombstoneSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GraveyardComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        Entity graveyardEntity = SystemAPI.GetSingletonEntity<GraveyardComponent>();
        GraveyardAspect graveyardAspect = SystemAPI.GetAspect<GraveyardAspect>(graveyardEntity);
        
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);

        BlobBuilder builder = new BlobBuilder(Allocator.Temp);
        ref ZombieSpawnPointsBlob spawnPoints = ref builder.ConstructRoot<ZombieSpawnPointsBlob>();
        BlobBuilderArray<float3> arrayBuilder = builder.Allocate(ref spawnPoints.value, graveyardAspect.GetNumberTombstonesToSpawn());
        
        float3 tombStoneOffset = new float3(0f, -2f, 1f);

        for (int i = 0; i < graveyardAspect.GetNumberTombstonesToSpawn(); i++)
        {
            Entity newTombstone = entityCommandBuffer.Instantiate(graveyardAspect.GetTombstonePrefab());
            LocalTransform newTombstoneTransform = graveyardAspect.GetRandomTombstoneTransform();
            entityCommandBuffer.SetComponent(newTombstone, newTombstoneTransform);
            
            float3 newZombieSpawnPoint = newTombstoneTransform.Position + tombStoneOffset;
            arrayBuilder[i] = newZombieSpawnPoint;
        }
        
        BlobAssetReference<ZombieSpawnPointsBlob> blobAsset = builder.CreateBlobAssetReference<ZombieSpawnPointsBlob>(Allocator.Persistent);
        entityCommandBuffer.SetComponent(graveyardEntity, new ZombieSpawnPointsComponent{ value = blobAsset });
        builder.Dispose();
        
        entityCommandBuffer.Playback(state.EntityManager);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}