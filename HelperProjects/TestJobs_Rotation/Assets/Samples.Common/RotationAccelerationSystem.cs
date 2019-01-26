using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Entities;

namespace Samples.Common
{
    public class RotationAccelerationSystem : JobComponentSystem
    {
        [ComputeJobOptimization]
        struct RotationSpeedAcceleration : IJobProcessComponentData<RotationSpeed, RotationAcceleration>
        {
            public float dt;

            public void Execute(ref RotationSpeed speed, ref RotationAcceleration acc)
            {
                speed.Value = math.max(0.0f, speed.Value + (acc.speed * dt));
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var RotationSpeedAccelerationJob = new RotationSpeedAcceleration { dt = Time.deltaTime };
            return RotationSpeedAccelerationJob.Schedule(this, 64, inputDeps);
        }
    }
}