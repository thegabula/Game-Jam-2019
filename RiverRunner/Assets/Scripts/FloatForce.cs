using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatForce : MonoBehaviour {

    public float waterLevel = 4;
    public float floatHeight = 2;
    public float bounceDamp = 0.05f;
    public Vector3 buoyancyCenterOffset;
    public WaveMaker water;

    private float forceFactor;
    private Vector3 actionPoint;
    private Vector3 upLift;

    public Rigidbody rb;
    private Vector3 relativePos;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        relativePos = rb.position;
    }

    public void ResetPosition()
    {
        transform.localPosition = new Vector3(0, 0, 0);
    }

    private void Update()
    {        
        waterLevel = water.GetWaterHeight(transform.position);
        actionPoint = transform.position + transform.TransformDirection(buoyancyCenterOffset);
        forceFactor = 1 - ((actionPoint.y - waterLevel) / floatHeight);
        float waterPressure = (transform.position.y + buoyancyCenterOffset.y) - waterLevel;
        upLift = -Physics.gravity * (forceFactor - rb.velocity.y * bounceDamp);
        if (forceFactor > 0f)
        {
            rb.AddForceAtPosition(upLift*2, actionPoint);
        }
        else
        {
            rb.AddForceAtPosition(upLift * 30, actionPoint);
        }        
    }
}
