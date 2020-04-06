using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{

    public float lookSpeed, jumpForce;
    public float jumpCD;
    // Start is called before the first frame update
    void Start()
    {
    
        body = GetComponent<Rigidbody>();
        controller = new PlayerController(this);
    }

    // Update is called once per frame
    void Update()
    {
        controller.UpdateMovementFlags();
    }
    private void FixedUpdate()
    {
        controller.ApplyMovement();
        
    }



    

}
