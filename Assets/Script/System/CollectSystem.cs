using Unity.Entities;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class CollectSystem : SystemBase
{   
    //用bufferSystem来处理这些碰撞事件
    private EndFixedStepSimulationEntityCommandBufferSystem bufferSystem;
    //初始化entity的物理世界
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
        //每一帧都添加一个triggerjob来进行碰撞判断，因为需要判断的是有MoveComponent的玩家
        //以及有DeleteTag的收集物，所以就要在job中进行选择
        Dependency = new TriggerJob
        {
            speedEntities = GetComponentDataFromEntity<MoveComponent>(),
            entitiesToDelete = GetComponentDataFromEntity<DeleteTag>(),
            commandBuffer = bufferSystem.CreateCommandBuffer(),
        }.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, Dependency);

        //把job传递到buffer中
        bufferSystem.AddJobHandleForProducer(Dependency);

    }


    //创建一个triggerjob来进行碰撞处理
    private struct TriggerJob : ITriggerEventsJob
    {   
        //初始化处理的entity
        public ComponentDataFromEntity<MoveComponent> speedEntities;
        [ReadOnly] public ComponentDataFromEntity<DeleteTag> entitiesToDelete;
        public EntityCommandBuffer commandBuffer;

        public void Execute(TriggerEvent triggerEvent)
        {
            TestEntityTrigger(triggerEvent.EntityA, triggerEvent.EntityB);
            TestEntityTrigger(triggerEvent.EntityB, triggerEvent.EntityA);
        }

        //处理碰撞，如果被碰撞的物品没有DeleteTag，就把DeleteTag挂上去，移除它的物理组件
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
