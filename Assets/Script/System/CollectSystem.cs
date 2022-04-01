using Unity.Entities;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class CollectSystem : SystemBase
{   
    //��bufferSystem��������Щ��ײ�¼�
    private EndFixedStepSimulationEntityCommandBufferSystem bufferSystem;
    //��ʼ��entity����������
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;

    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<EndFixedStepSimulationEntityCommandBufferSystem>();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        //ÿһ֡�����һ��triggerjob��������ײ�жϣ���Ϊ��Ҫ�жϵ�����MoveComponent�����
        //�Լ���DeleteTag���ռ�����Ծ�Ҫ��job�н���ѡ��
        Dependency = new TriggerJob
        {
            speedEntities = GetComponentDataFromEntity<MoveComponent>(),
            entitiesToDelete = GetComponentDataFromEntity<DeleteTag>(),
            commandBuffer = bufferSystem.CreateCommandBuffer(),
        }.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, Dependency);

        //��job���ݵ�buffer��
        bufferSystem.AddJobHandleForProducer(Dependency);

    }


    //����һ��triggerjob��������ײ����
    private struct TriggerJob : ITriggerEventsJob
    {   
        //��ʼ�������entity
        public ComponentDataFromEntity<MoveComponent> speedEntities;
        [ReadOnly] public ComponentDataFromEntity<DeleteTag> entitiesToDelete;
        public EntityCommandBuffer commandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            TestEntityTrigger(triggerEvent.EntityA, triggerEvent.EntityB);
            TestEntityTrigger(triggerEvent.EntityB, triggerEvent.EntityA);
        }

        //������ײ���������ײ����Ʒû��DeleteTag���Ͱ�DeleteTag����ȥ���Ƴ������������
        private void TestEntityTrigger(Entity entity1, Entity entity2)
        {
            if (speedEntities.HasComponent(entity1))
            {
                if (entitiesToDelete.HasComponent(entity2)) { return; }
                commandBuffer.AddComponent<DeleteTag>(entity2);
                commandBuffer.RemoveComponent<PhysicsCollider>(entity2);
            }
        }
    }
}
