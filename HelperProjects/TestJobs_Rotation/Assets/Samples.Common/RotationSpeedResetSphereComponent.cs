using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Entities;


namespace Samples.Common
{
    public class RotationSpeedResetSphereComponent : ComponentDataWrapper<RotationSpeedResetSphere> { }

    [Serializable]
    public struct RotationSpeedResetSphere : IComponentData
    {
        public float speed;
    }
}