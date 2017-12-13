using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceShipController : MonoBehaviour {
	public float shakeAmount = 0.2f;
	internal bool flying = true;

	Vector2 originalPos;
	Animator anim;

	// Use this for initialization
	void Start () {
		originalPos = transform.localPosition;
		anim = GetComponent<Animator>();
		ChangeFly(false);
	}
	
	// Update is called once per frame
	void Update () {		
		MoveVibration();		
	}

	private void MoveVibration(){
		if(flying && Time.frameCount%3 == 0){
			transform.localPosition = originalPos + (Vector2)Random.insideUnitSphere * shakeAmount;
		}
	}

	internal void ChangeFly(bool value){
		flying = false;
		anim.SetBool("flying", value);
	}
}
