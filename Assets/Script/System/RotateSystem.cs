using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class RotateSystem : SystemBase
{
    protected override void OnUpdate()
    {

        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Rotation rotation, in RotationComponent rotationSpeed) =>
        {
            rotation.Value = math.mul(rotation.Value, quaternion.RotateX(math.radians(rotationSpeed.rotateSpeed * deltaTime)));
            rotation.Value = math.mul(rotation.Value, quaternion.RotateY(math.radians(rotationSpeed.rotateSpeed * deltaTime)));
            rotation.Value = math.mul(rotation.Value, quaternion.RotateZ(math.radians(rotationSpeed.rotateSpeed * deltaTime)));
        }).Run();
    }
}
