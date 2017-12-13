using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {

	// Use this for initialization
    public Vector2 startingGravity = new Vector2(0, -16);    
    internal Drop[] drops;
    internal CheckPoint[] checkpoints;

    private int dropsGathered = 0;

    //Creamos un event listener que se ejecutara cada vez que cambie la variable dropsGathered
    public int drop_gathered{
        get{return dropsGathered;}
        set {
         if (dropsGathered == value) return;
         dropsGathered = value;
         if (OnVariableChange != null)
             OnVariableChange(dropsGathered);     
        }
    }
    public delegate void OnVariableChangeDelegate(int newVal);
    public event OnVariableChangeDelegate OnVariableChange;


	void Start () 
    {
		Physics2D.gravity = startingGravity;
        drops = GameObject.FindObjectsOfType<Drop>();
        checkpoints = GameObject.FindObjectsOfType<CheckPoint>();
        this.OnVariableChange += VariableChangeHandler;
	}

    private void VariableChangeHandler(int newVal)
 {
     //TODO: refrescar el HUD con el numero de gotas
     print("gotas recogidas: " + dropsGathered);
 }

    internal void DropGathered(){
        drop_gathered++;
    }

    internal void ResetScene(){
        foreach(Drop d in drops){
            if (!d.SceneReset())
            drop_gathered--;
        }
    }

}

/*class Layer
{
    public static int TRIS = 9;
    public static int IGNORE_RAYCAST = 2;
    public static int GROUND = 8;
    public static int DAMAGE = 10;
}*/
