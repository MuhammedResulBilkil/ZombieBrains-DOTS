using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
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

        for (int i = 0; i < graveyardAspect.GetNumberTombstonesToSpawn(); i++)
        {
            Entity newTombstone = entityCommandBuffer.Instantiate(graveyardAspect.GetTombstonePrefab());
            LocalTransform newTombstoneTransform = graveyardAspect.GetRandomTombstoneTransform();
            entityCommandBuffer.SetComponent(newTombstone, newTombstoneTransform);
        }
            

        entityCommandBuffer.Playback(state.EntityManager);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}