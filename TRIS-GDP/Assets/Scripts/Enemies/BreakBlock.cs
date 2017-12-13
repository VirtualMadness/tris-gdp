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

    void OnTriggerEnter2D(Collider2D other)
    {
        if(state == State.ready && other.gameObject.CompareTag(GameObject.Find("TRIS").tag))
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
        //this.gameObject.layer = Layer.IGNORE_RAYCAST;
        GetComponent<BoxCollider2D>().enabled = false;
        state = State.broken;
    }

    private void restored()
    {
        //this.gameObject.layer = Layer.GROUND;
        GetComponent<BoxCollider2D>().enabled = true;
        state = State.ready;
    }

}
