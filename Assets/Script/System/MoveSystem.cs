using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

public class MoveSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;

        float2 curInput = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        Entities.ForEach((ref PhysicsVelocity vel, ref MoveComponent speedData) =>
        {
            float2 newVel = vel.Linear.xz;

            newVel += curInput * speedData.moveSpeed * deltaTime;

            vel.Linear.xz = newVel;
        }).Run();
    }
}
