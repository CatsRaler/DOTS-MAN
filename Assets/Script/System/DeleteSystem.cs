using Unity.Entities;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//�������һ�����ԣ�������collectionsystem����֮���ٸ��£���ΪҪ����ײ֮���ٽ��д���
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