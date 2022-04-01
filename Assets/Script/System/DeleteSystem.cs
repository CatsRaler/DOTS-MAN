using Unity.Entities;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//这里添加一个属性，就是在collectionsystem发生之后再更新，因为要先碰撞之后再进行处理
[UpdateAfter(typeof(CollectSystem))]
public class DeleteSystem : SystemBase
{
    private EndFixedStepSimulationEntityCommandBufferSystem _endSimulationECBSystem;

    protected override void OnStartRunning()
    {
        _endSimulationECBSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _endSimulationECBSystem.CreateCommandBuffer();

        Entities
            .WithAll<DeleteTag>()
            .WithoutBurst()
            .ForEach((Entity entity) =>
            {
                GameManager.instance.IncreaseScore();
                ecb.DestroyEntity(entity);
            }).Run();
        _endSimulationECBSystem.AddJobHandleForProducer(Dependency);
    }
}