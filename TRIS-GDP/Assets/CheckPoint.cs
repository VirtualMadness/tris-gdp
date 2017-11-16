using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

	// Use this for initialization
    private bool inverted;
    private bool active = false;
    private bool starter = false;
	void Start () {
		inverted = GetComponent<SpriteRenderer>().flipY;
        if(GetComponent<Animator>() == null){
            starter = true;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool isInverted()
    {
        return inverted;
    }

    internal void disable(){
        active = false;
        if(!starter){
            GetComponent<Animator>().SetBool("active", active);
        }
    }

    internal void enable(){
        active = true;
        if(!starter){
            GetComponent<Animator>().SetBool("active", active);
        }
    }

	void OnTriggerEnter2D(Collider2D other)    
    {
        if(starter)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(!active){
        //Debug.Log("Contact");
        if(other.gameObject.CompareTag(player.tag))
        {   
            CheckPoint[] cps = GameObject.FindGameObjectWithTag("LevelController").GetComponent<LevelController>().checkpoints;
            foreach(CheckPoint cp in cps){
                cp.disable();
            }     
            player.GetComponent<PlayerMovement>().refreshCheckpoint(this);
            enable();
        }
        }		
    }
}
