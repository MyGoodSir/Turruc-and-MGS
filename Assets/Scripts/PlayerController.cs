using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody body;
    public float moveSpeed, jumpForce;
    public float jumpCD;
    float jumpTimer;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        jumpTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 moveDirection = new Vector3();

        gameObject.transform.Rotate(0, Input.GetAxis("Horizontal"), 0);

        if (Input.GetKey(KeyCode.W))
            moveDirection += gameObject.transform.TransformDirection(new Vector3(0, 0, 1));
        
        if(Input.GetKey(KeyCode.S))
            moveDirection += gameObject.transform.TransformDirection(new Vector3(0, 0, -1 ));
        
        if(Input.GetKey(KeyCode.A))
            moveDirection += gameObject.transform.TransformDirection(new Vector3(-1, 0, 0));
        
        if(Input.GetKey(KeyCode.D))
            moveDirection += gameObject.transform.TransformDirection(new Vector3(1, 0, 0));
        

        if(moveDirection.x != 0 && moveDirection.y != 0)
            moveDirection /= Mathf.Sqrt(2);

        moveDirection *= moveSpeed;
        gameObject.transform.position += moveDirection;
        if (jumpTimer <= 0)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                jumpTimer = jumpCD;
                body.AddForce(new Vector3(0, jumpForce, 0));
            }
        }
        else if(jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }

        }


}
