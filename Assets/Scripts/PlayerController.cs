using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterController
{
    float jumpTimer = 0;


    public PlayerController(Player parent) : base(parent) {  }

    override public void UpdateMovementFlags()
    {
        this.movementFlags.forward = Input.GetKey(KeyCode.W);
        this.movementFlags.backward = Input.GetKey(KeyCode.S);
        this.movementFlags.left = Input.GetKey(KeyCode.A);
        this.movementFlags.right = Input.GetKey(KeyCode.D);
        this.movementFlags.jumping = Input.GetKey(KeyCode.Space);


        Vector3 ground = GameObject.FindGameObjectWithTag("Ground").GetComponent<Collider>().ClosestPointOnBounds(parent.GetComponent<CapsuleCollider>().transform.position);
        Vector3 cap = parent.GetComponent<CapsuleCollider>().ClosestPoint(ground);
        this.movementFlags.isOnGround = cap.Equals(ground);
    }

    Vector3 GetMovementVector()
    {
        Vector3 moveDirection = new Vector3();

        float sine = Mathf.Sin(parent.gameObject.transform.eulerAngles.y * Mathf.PI / 180);
        float cosine = Mathf.Cos(parent.gameObject.transform.eulerAngles.y * Mathf.PI / 180);


        if (this.movementFlags.forward)
            moveDirection += parent.gameObject.transform.forward;

        if (this.movementFlags.backward)
            moveDirection += parent.gameObject.transform.forward*-1;

        if (this.movementFlags.left)
            moveDirection += new Vector3(-cosine, 0, sine);

        if (this.movementFlags.right)
            moveDirection += new Vector3(cosine, 0, -sine);


        moveDirection.Normalize();

        return moveDirection * parent.moveSpeed;
    }
    override public void ApplyMovement()
    {
        Vector3 translationVector = GetMovementVector();

        Vector3 rotationVector = new Vector3(0, Input.GetAxis("Horizontal")* ((Player)parent).lookSpeed, 0);
        parent.gameObject.transform.Rotate(rotationVector);


        parent.getBody().velocity +=translationVector;
        parent.getBody().velocity = Vector3.ClampMagnitude(parent.getBody().velocity, parent.maxSpeed);


        if (movementFlags.canCurrentlyJump)
        {
            if (movementFlags.jumping)
            {
                movementFlags.canCurrentlyJump = false;
                movementFlags.jumping = true;
                movementFlags.isOnGround = false;
                parent.getBody().AddForce(new Vector3(0, ((Player)parent).jumpForce, 0));
            }
        }
        else if (movementFlags.isOnGround)
        {
            movementFlags.canCurrentlyJump = true;
        }
    }


}
