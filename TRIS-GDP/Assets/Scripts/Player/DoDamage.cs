﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoDamage : MonoBehaviour {

    private GameObject player;

	// Use this for initialization
	void Start () 
    {
		player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Contact");
        if(other.gameObject.CompareTag(player.tag))
        {
            Debug.Log("Damage");
            resetScene();
        }
    }
    void resetScene()
    {
        PlayerMovement pm = player.GetComponent<PlayerMovement>();
        CameraMovement cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraMovement>();

        cam.Shake();
        cam.Shake();
        cam.Shake();

        pm.ResetPositionToActiveCheckpoint();
    }
}
