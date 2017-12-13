using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drop : MonoBehaviour {

	bool saved = false;
	bool gathered = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(gathered)
			return;
		if(other.gameObject.tag.Equals("Player")){
			gathered = true;
			GetComponent<SpriteRenderer>().enabled = false;
			GameObject.FindObjectOfType<LevelController>().DropGathered();
		}
	}
	//Si al resetear la escena (cuando tris muere) la gota no se ha guardado en un checkpoint se reinicia
	//devolvemos false para que el level controller reste 1 al contador de gotas, y true si la gota sigue guardada
	internal bool SceneReset(){
		if(!saved && gathered){
			gathered = false;
			GetComponent<SpriteRenderer>().enabled = true;
			return false;
		}
		return true;
	}
}
