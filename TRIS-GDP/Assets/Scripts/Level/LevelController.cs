using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {

	// Use this for initialization
    public Vector2 startingGravity = new Vector2(0, -16);
    public CheckPoint activeCP; 
    public Drop[] drops;

	void Start () 
    {
		Physics2D.gravity = startingGravity;
	}
}

class Layer
{
    public static int TRIS = 9;
    public static int IGNORE_RAYCAST = 2;
    public static int GROUND = 8;
    public static int DAMAGE = 10;
}
