using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

namespace Samples.Common
{
    public class RotationSpeedComponent : ComponentDataWrapper<RotationSpeed> { }

    [Serializable]
    public struct RotationSpeed : IComponentData
    {
        public float Value;
    }

}