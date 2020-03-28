using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterController
{
    float jumpTimer = 0;
    
    Vector3 Walk(float moveSpeed, Player p)
    {
        Vector3 moveDirection = new Vector3();
        if (Input.GetKey(KeyCode.W))
            moveDirection += p.gameObject.transform.TransformDirection(new Vector3(0, 0, 1));

        if (Input.GetKey(KeyCode.S))
            moveDirection += p.gameObject.transform.TransformDirection(new Vector3(0, 0, -1));

        if (Input.GetKey(KeyCode.A))
            moveDirection += p.gameObject.transform.TransformDirection(new Vector3(-1, 0, 0));

        if (Input.GetKey(KeyCode.D))
            moveDirection += p.gameObject.transform.TransformDirection(new Vector3(1, 0, 0));


        if (moveDirection.x != 0 && moveDirection.y != 0)
            moveDirection /= Mathf.Sqrt(2);

        return moveDirection * moveSpeed;
    }
    override public void ApplyMovement(Character c)
    {
        Player p = (Player)c;
        Vector3 translationVector = Walk(p.moveSpeed, p);
        Vector3 rotationVector = new Vector3(0, Input.GetAxis("Horizontal")*p.lookSpeed, 0);

        p.gameObject.transform.Rotate(rotationVector);
        p.gameObject.transform.position += translationVector;

        if (jumpTimer <= 0)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                jumpTimer = p.jumpCD;
                p.getBody().AddForce(new Vector3(0, p.jumpForce, 0));
            }
        }
        else if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }
    }


}
