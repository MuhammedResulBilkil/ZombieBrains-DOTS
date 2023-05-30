using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
public partial struct SpawnZombieSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var ecbSingleton = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();
        
        new SpawnZombieJob()
        {
            entityCommandBuffer = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged),
            deltaTime = deltaTime
        }.Run();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}

[BurstCompile]
public partial struct SpawnZombieJob : IJobEntity
{
    public EntityCommandBuffer entityCommandBuffer;
    
    public float deltaTime;

    private void Execute(GraveyardAspect graveyardAspect)
    {
        float zombieSpawnTimer = graveyardAspect.GetZombieSpawnTimer();
        zombieSpawnTimer -= deltaTime;
        graveyardAspect.SetZombieSpawnTimer(zombieSpawnTimer);
        
        if(!graveyardAspect.IsTimeToSpawnZombie()) return;
        if(!graveyardAspect.IsZombieSpawnPointInitialized()) return;

        graveyardAspect.ResetZombieSpawnTimer();
        Entity newZombie = entityCommandBuffer.Instantiate(graveyardAspect.GetZombiePrefab());

        LocalTransform newZombieTransform = graveyardAspect.GetZombieSpawnPoint();
        entityCommandBuffer.SetComponent(newZombie, newZombieTransform);

        float zombieHeading =
            MathHelpers.GetHeading(newZombieTransform.Position, graveyardAspect.GetLocalTransformPosition());
        //entityCommandBuffer.SetComponent(newZombie, new ZombieHeadingComponent {value = zombieHeading});

    }
}

