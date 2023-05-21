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
    
    private float3 HalfDimensions => new()
    {
        x = _graveyardComponent.ValueRO.fieldDimensions.x * 0.5f,
        y = 0f,
        z = _graveyardComponent.ValueRO.fieldDimensions.y * 0.5f
    };
    private float3 MinCorner => _localTransform.ValueRO.Position - HalfDimensions;
    private float3 MaxCorner => _localTransform.ValueRO.Position + HalfDimensions;
    
    
    public LocalTransform GetRandomTombstoneTransform()
    {
        return new LocalTransform
        {
            Position = GetRandomPosition(),
            Rotation = GetRandomRotation(),
            Scale = GetRandomScale(0.5f)
        };
    }
    public Entity GetTombstonePrefab() => _graveyardComponent.ValueRO.tombstonePrefab;
    public int GetNumberTombstonesToSpawn() => _graveyardComponent.ValueRO.numberTombstonesToSpawn;
    
    private float3 GetRandomPosition()
    {
        float3 randomPosition;
        do
        {
            randomPosition = _graveyardRandomComponent.ValueRW.value.NextFloat3(MinCorner, MaxCorner);
        } while (math.distancesq(_localTransform.ValueRO.Position, randomPosition) <= BRAIN_SAFETY_RADIUS_SQ);

        return randomPosition;
    }
    private quaternion GetRandomRotation() => quaternion.RotateY(_graveyardRandomComponent.ValueRW.value.NextFloat(-0.25f, 0.25f));
    private float GetRandomScale(float min) => _graveyardRandomComponent.ValueRW.value.NextFloat(min, 1f);
}