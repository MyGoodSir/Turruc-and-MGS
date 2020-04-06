using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    protected Rigidbody body;
    public float moveSpeed, maxSpeed, health, stamina;
    protected CharacterController controller;

    public virtual Rigidbody getBody()
    {
        return body;
    }

}
