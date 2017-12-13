using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoDamage : MonoBehaviour {

    private GameObject player;

    public bool destroyAfterDoingDamage = false;

	// Use this for initialization
	void Start () 
    {
		player = GameObject.FindGameObjectWithTag("Player");
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("Contact");
        if(other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Damage");
            resetScene();
        }
    }
    void resetScene()
    {
        PlayerMovement pm = player.GetComponent<PlayerMovement>();       

        pm.Die();
    }
}
