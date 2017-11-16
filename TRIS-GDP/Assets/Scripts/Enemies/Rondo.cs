using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rondo : MonoBehaviour {

	public Vector2 direction = new Vector2(1, 0);
    public float speed = 4;

	void Update () {
        Vector2 aux = direction * speed * Time.deltaTime;
		this.transform.position += new Vector3(aux.x, aux.y, 0);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("RondoTrigger");
        if(other.CompareTag("RondoTrigger"))
        {
            this.direction = this.direction*-1;
        }
    }
}
