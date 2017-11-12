using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBlock : MonoBehaviour
{

    public Collider2D trigger;
    private enum State
    {
        ready = 0,
        breaking = 1,
        broken = 2,
        restoring = 3
    }
    private State state;
    private Animator anim;

    void Start()
    {

        state = State.ready;
        anim = GetComponent<Animator>();
        
    }


    void Update()
    {

        if(state == State.ready && trigger.IsTouchingLayers(Physics2D.GetLayerCollisionMask(9)))
        {
            this.breaking();
        }
    }

    private void breaking()
    {
        Debug.Log("Breaking block at " + transform.position.ToString());
        anim.SetTrigger("break");
        state = State.breaking;
    }

    private void broken()
    {
        this.gameObject.layer = Layer.ignoreRayCast.GetHashCode();
        state = State.broken;
    }

    private void restored()
    {
        this.gameObject.layer = Layer.ground.GetHashCode();
        state = State.ready;
    }

}
