using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Entities;


namespace Samples.Common
{
    public class RadiusComponent : ComponentDataWrapper<Radius> { }

    [Serializable]
    public struct Radius : IComponentData
    {
        public float radius;
    }
}