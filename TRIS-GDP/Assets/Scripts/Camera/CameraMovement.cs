using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
	public float smoothing = 2;
	public float maxWidth = 160/8f;
	GameObject target;
	float actualY;

	public static Color BLACK = new Color(33f/255f, 24f/255f, 3f/255f);
	public static Color PINK = new Color(179f/255f, 112f/255f, 116f/255f);
	public static Color GREEN = new Color(31f/255f, 94f/255f, 96f/255f);
	public static Color WHITE = new Color(215f/255f, 239f/255f, 222f/255f);


    // Use this for initialization
    void Start () {
		target = GameObject.FindGameObjectWithTag("Player");
		GetComponent<Camera>().backgroundColor = PINK;
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
