using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.PackageManager;
using Unity.Entities;
using UnityEngine.Jobs;


public class RotationSpeedHybridECS : MonoBehaviour
{

    public float speed;

    private void Start()
    {
        speed = Random.Range(2, 100);
    }
}




public class RotatingSystemHybridECS : ComponentSystem
{
    public struct Group
    {
        public Transform transform;
        public RotationSpeedHybridECS rotation;

    }

    protected override void OnUpdate()
    {
        float dt = Time.deltaTime;
        foreach (var e in GetEntities<Group>())
        {
            e.transform.rotation = e.transform.rotation * Quaternion.AngleAxis(dt * e.rotation.speed, Vector3.up);
        }
    }
}

