using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct GraveyardAspect : IAspect
{
    private const float BRAIN_SAFETY_RADIUS_SQ = 100;
    
    public readonly Entity entity;
    
    private readonly RefRO<LocalTransform> _localTransform;
    
    private readonly RefRO<GraveyardComponent> _graveyardComponent;
    private readonly RefRW<GraveyardRandomComponent> _graveyardRandomComponent;
    
    private readonly RefRW<ZombieSpawnPointsComponent> _zombieSpawnPointsComponent;
    private readonly RefRW<ZombieSpawnTimerComponent> _zombieSpawnTimerComponent;

    public float GetZombieSpawnTimer() => _zombieSpawnTimerComponent.ValueRO.value;
    public void SetZombieSpawnTimer(float value) => _zombieSpawnTimerComponent.ValueRW.value = value;
    public bool IsTimeToSpawnZombie() => GetZombieSpawnTimer() <= 0f;
    public float3 GetLocalTransformPosition() => _localTransform.ValueRO.Position;

    public LocalTransform GetRandomTombstoneTransform() =>
        new()
        {
            Position = GetRandomPosition(),
            Rotation = GetRandomRotation(),
            Scale = GetRandomScale(0.5f)
        };
    
    public LocalTransform GetZombieSpawnPoint()
    {
        var position = GetRandomZombieSpawnPoint();
        return new LocalTransform
        {
            Position = position,
            Rotation = quaternion.RotateY(MathHelpers.GetHeading(position, GetLocalTransformPosition())),
            Scale = 1f
        };
    }
    
    public Entity GetTombstonePrefab() => _graveyardComponent.ValueRO.tombstonePrefab;
    public Entity GetZombiePrefab() => _graveyardComponent.ValueRO.zombiePrefab;
    public int GetNumberTombstonesToSpawn() => _graveyardComponent.ValueRO.numberTombstonesToSpawn;
    public float GetZombieSpawnRate() => _graveyardComponent.ValueRO.zombieSpawnRate;
    public void ResetZombieSpawnTimer() => SetZombieSpawnTimer(GetZombieSpawnRate());
    public bool IsZombieSpawnPointInitialized() => _zombieSpawnPointsComponent.ValueRO.value.IsCreated && GetZombieSpawnPointCount() > 0;

    
    private float3 GetMinCorner() => _localTransform.ValueRO.Position - GetHalfDimensions();
    private float3 GetMaxCorner() => _localTransform.ValueRO.Position + GetHalfDimensions();
    private float3 GetZombieSpawnPoint(int i) => _zombieSpawnPointsComponent.ValueRO.value.Value.value[i];
    private int GetZombieSpawnPointCount() => _zombieSpawnPointsComponent.ValueRO.value.Value.value.Length;
    private float3 GetRandomZombieSpawnPoint() =>
        GetZombieSpawnPoint(_graveyardRandomComponent.ValueRW.value.NextInt(GetZombieSpawnPointCount()));

    private float3 GetRandomPosition()
    {
        float3 randomPosition;
        do
        {
            randomPosition = _graveyardRandomComponent.ValueRW.value.NextFloat3(GetMinCorner(), GetMaxCorner());
        } while (math.distancesq(_localTransform.ValueRO.Position, randomPosition) <= BRAIN_SAFETY_RADIUS_SQ);

        return randomPosition;
    }
    
    private float3 GetHalfDimensions() =>
        new()
        {
            x = _graveyardComponent.ValueRO.fieldDimensions.x * 0.5f,
            y = 0f,
            z = _graveyardComponent.ValueRO.fieldDimensions.y * 0.5f
        };

    private quaternion GetRandomRotation() =>
        quaternion.RotateY(_graveyardRandomComponent.ValueRW.value.NextFloat(-0.25f, 0.25f));
    private float GetRandomScale(float min) => _graveyardRandomComponent.ValueRW.value.NextFloat(min, 1f);
}