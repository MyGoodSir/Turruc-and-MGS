using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Burst;

namespace TMGS.Rotate
{

    
    public class RotateSystem : JobComponentSystem
    {
        [BurstCompile]
        private struct RotateJob : IJobForEach<RotationEulerXYZ, Rotate>
        {
            public float deltaTime;
            public void Execute(ref RotationEulerXYZ euler, ref Rotate rotate)
            {
                euler.Value.y += rotate.radiansPerSecond * deltaTime;
            }
        }
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new RotateJob { deltaTime = Time.DeltaTime };
            return job.Schedule(this, inputDeps);
        }
    }


    //Mono-threadded implementation below

    /*
    public class RotateSystem : ComponentSystem
    {
        
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Rotate rotate, ref RotationEulerXYZ euler) =>
            {
                euler.Value.y += rotate.radiansPerSecond * Time.DeltaTime;
            });
        }
    }
    */
}