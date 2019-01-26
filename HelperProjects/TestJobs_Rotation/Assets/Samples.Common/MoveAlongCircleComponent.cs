using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System;

namespace Samples.Common
{
    public class MoveAlongCircleComponent : ComponentDataWrapper<MoveAlongCircle> { }

    [Serializable]
    public struct MoveAlongCircle : IComponentData
    {
        public float3 center;
        public float radius;
        [NonSerialized]
        public float t;
    }
}