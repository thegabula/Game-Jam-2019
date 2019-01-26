using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;

namespace Samples.Common
{
    public class SpawnRandomCircleComponent : SharedComponentDataWrapper<SpawnRandomCircle> { }

    [Serializable]
    public struct SpawnRandomCircle : ISharedComponentData
    {
        public GameObject prefab;
        public bool spawnLocal;
        public float radius;
        public int count;
    }
}