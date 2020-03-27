using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody body;
    public float testStrength;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 moveDirection = new Vector3();

        if(Input.GetKey(KeyCode.W))
        {
            moveDirection += gameObject.transform.TransformDirection(new Vector3(0, 0, 1 * testStrength));
        }

        if(Input.GetKey(KeyCode.S))
        {
            moveDirection += gameObject.transform.TransformDirection(new Vector3(0, 0, -1 * testStrength));
        }
        if(Input.GetKey(KeyCode.A))
        {
            moveDirection += gameObject.transform.TransformDirection(new Vector3(-1 * testStrength, 0, 0));
        }
        if(Input.GetKey(KeyCode.D))
        {
            moveDirection += gameObject.transform.TransformDirection(new Vector3(1 * testStrength, 0, 0));
        }
            body.AddForce(moveDirection);
    }


}
