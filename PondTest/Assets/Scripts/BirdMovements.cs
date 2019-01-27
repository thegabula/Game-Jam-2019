using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BirdMovements : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    float rotationSpeed = 10.0f;
    float smooth = 5.0f;
    float tiltAngle = 20.0f;
    float m_Speed;
    //float minSpeed = 10f;
    //float maxSpeed = 20f;
    //float acceleration = 2f;

    //bool restoreRotation = true;
    //bool restoreSpeed = true;

    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Rigidbody component you attach from your GameObject
        m_Rigidbody = GetComponent<Rigidbody>();
        //Set the speed of the GameObject
        m_Speed = 10f;
        // set max speed
        //maxSpeed = 30f;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //Rotate the sprite about the Y axis in the positive direction
            transform.Rotate(new Vector3(0, rotationSpeed, 0) * Time.deltaTime * m_Speed, Space.World);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            //Rotate the sprite about the Y axis in the negative direction
            transform.Rotate(new Vector3(0, -rotationSpeed, 0) * Time.deltaTime * m_Speed, Space.World);
        }
        else
        {

        }

        /*
        if (Input.GetKey(KeyCode.Space))
        {
            if (m_Speed <= maxSpeed)
            {
                m_Speed += acceleration;
            }
        }
        else
        {
            if (m_Speed > minSpeed)
            {
                m_Speed -= acceleration;
            }
            else
            {
                m_Speed = minSpeed;
            }
        }
        */
       
     /*   if (Input.GetKey("space"))
        {
            m_Rigidbody.AddForce(transform.up * 55);
        }
        else if (Input.GetKeyUp("space"))
        {
            m_Rigidbody.AddForce(transform.up * 0);
        }
        */
        
        float tiltAroundX = 0 - Input.GetAxis("Vertical") * tiltAngle;
        Quaternion target = Quaternion.Euler(tiltAroundX, transform.localEulerAngles.y, 0);

        // Dampen towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, target, Time.deltaTime * smooth);
        

        transform.Translate(0, 0, Time.deltaTime * m_Speed);

        //restoreRotation = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.name);
        if (other.gameObject.name == "DRAGNFLY(Clone)")
         {
             this.gameObject.GetComponentInChildren<Slider>().value += 5;
        }
    }
}
