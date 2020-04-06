using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController
{
    protected Character parent;
    protected MovementFlags movementFlags;
    protected struct MovementFlags
    {
        public bool forward, backward, left, right, up, down, jumping;
        public bool canCurrentlyJump, isOnGround;
    }

    public CharacterController(Character parent)
    {
        this.parent = parent;
    }
    public virtual void UpdateMovementFlags(){ }

    public virtual void ApplyMovement()
    {

    }
}
