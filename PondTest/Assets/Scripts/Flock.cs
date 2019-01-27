using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public float speed = 1f;
    public float maxSpeed = 2f;
    float rotationSpeed = 4.0f;
    Vector3 averageHeading;
    Vector3 averagePosition;
    float neighbourDistance = 2.0f;

    public GlobalFlock swarm;

    bool turning = false;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(1f, maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 goalPos = swarm.goalPos;
        if (Vector3.Distance(transform.position, goalPos) >= GlobalFlock.sendboxSize)
        {
            turning = true;
        }
        else
            turning = false;


        if (turning)
        {
            Vector3 direction = goalPos - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction),
                rotationSpeed * Time.deltaTime);
            speed = Random.Range(1f, maxSpeed);
        }
        else { 
            if (Random.Range(0, 5) < 1)
            {
                ApplyRules();
            }
        }
        transform.Translate(0, 0, Time.deltaTime * speed);

    }

    public void AssignSwarm(GlobalFlock newSwarm)
    {
        swarm = newSwarm;
    }

    void ApplyRules()
    {
        GameObject[] gos;
        gos = GlobalFlock.allBugs;

        // center of the group
        Vector3 vcenter = Vector3.zero;
        // avoidance vector
        Vector3 vavoid = Vector3.zero;
        // group speed
        float gSpeed = 0.3f;

        Vector3 goalPos = swarm.goalPos;

        float dist;

        int groupSize = 0;
        foreach(GameObject go in gos)
        {
            if (go!=null && go != this.gameObject)
            {
                dist = Vector3.Distance(go.transform.position, this.transform.position);
                if (dist <= neighbourDistance)
                {
                    vcenter += go.transform.position;
                    groupSize++;

                    if (dist < 1.0f)
                    {
                        vavoid = vavoid + (this.transform.position - go.transform.position);
                    }

                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }
        //Debug.Log(groupSize);
        if (groupSize > 0)
        {
            vcenter = vcenter / groupSize + (goalPos - this.transform.position);
            speed = gSpeed / groupSize;

            Vector3 direction = (vcenter + vavoid) - transform.position;
            //Debug.Log(direction != Vector3.zero);
            if (direction != Vector3.zero)
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(direction),
                                                      rotationSpeed * Time.deltaTime);
            
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
        }
    }
}
