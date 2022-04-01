using Unity.Entities;

[GenerateAuthoringComponent]
public struct MoveComponent : IComponentData
{
    public float moveSpeed;
}
