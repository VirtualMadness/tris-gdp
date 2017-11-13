using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

	// Use this for initialization
    private bool inverted;
	void Start () {
		inverted = GetComponent<SpriteRenderer>().flipY;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool isInverted()
    {
        return inverted;
    }
}
