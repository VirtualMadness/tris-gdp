using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
	public float smoothing = 2;
	public float maxWidth = 160/8f;
	//GameObject target;
	float actualY;

	[Space]
	[Header("ScreenShake")]
    public float maxShake = 0.2f;

    private float shake = 0f;

	// Use this for initialization
	void Start () {
		//target = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 camPos = new Vector3(0f, actualY, transform.position.z);
		transform.position = Vector3.Lerp(transform.position, camPos, smoothing * Time.deltaTime) + new Vector3(Mathf.Sign(Random.Range(-1, 1))*shake, Random.Range(-1, 1) *shake, 0);

        shake = Mathf.Lerp(shake, 0, 0.3f);
	}

	public void ChangeY(float newY){
		actualY = newY;
	}

    public void Shake()
    {
        shake += maxShake;
    }
}
