using Unity.Entities;

[GenerateAuthoringComponent]
public struct RotationComponent : IComponentData
{
    public float rotateSpeed; 
}
