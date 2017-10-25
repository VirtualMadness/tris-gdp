using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
	public float smoothing = 2;
	public float maxWidth = 160/8f;
	GameObject target;
	float actualY;

	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 camPos = new Vector3(0f, actualY, transform.position.z);
		transform.position = Vector3.Lerp(transform.position, camPos, smoothing * Time.deltaTime);
	}

	public void ChangeY(float newY){
		actualY = newY;
	}
}
