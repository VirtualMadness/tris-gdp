using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour {
	[SerializeField] public float speed = 4f;
	[SerializeField] public bool debug;	
	[SerializeField] public bool canChangeGravity;

	float input;
	bool inputAction;
	bool inputPause;
	bool action;
	bool isMoving = false;
	bool grounded = false;
	//Vector2 position;
	Vector2 movement;
	Animator anim;
	bool inverted;


	// Use this for initialization
	void Awake () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {		
		GetInput();
		Action();
		Move();
		DrawDebug();
	}

	///<summary>
	/// Comprueba si hay una colision en la direccion del moviento que pasemos como argumento y devuelve un boolean con true si puede realizarse
	/// la traslacion sin problemas o false si encuentra algun obstaculo en medio
	///</summary>
	private bool CanMoveTo(Vector2 motion){
		RaycastHit2D hit;
		hit = Physics2D.Raycast((Vector2)transform.position, motion, motion.magnitude, LayerMask.GetMask("Ground"));
		if(hit.collider != null){
			return false;
		}else{
			return true;
		}
	}

	/// <summary>
	/// This coroutine moves the gameObject using the motion vector with the specified speed in meters per second.
	/// Don't forget to call it with StartCoroutine() or use the none coroutine method
	/// if you don't want to chain other coroutines to it.
	/// </summary>
	/// <param name="motion">The motion that you want to happen</param>
	/// <param name="speed">Speed of the movement in meters per second. So if the motion vector's length is 2 and you set 1 as speed, it will take 2 seconds for the object to arrive at destination</param>
	/// <param name="checkCollision">If true then the method will check for collisions before moving but not while moving toward there</param>
	/// <returns></returns>
	/// <remarks>This method is good for moving objects in grid/tile based games smoothly.
	/// You can give the motions to the object in size of the tile and also give it an appropreate speed to arrive in time but still the object
	/// only stops at grid boundaries while having a smooth motion visually.
	/// <para>The <c>isMovingUsingSmoothMove property is true while the move is being executed.</c></para>
	/// </remarks>
	public IEnumerator SmoothGridMoveCoroutine(Vector2 motion, float speed, bool checkCollision)
	{			
		if (checkCollision)
		{
			if (!CanMoveTo(motion)) yield break;
		}		
		if(motion == Vector2.zero || speed == 0.0f){
			yield break;
		}

		speed = Mathf.Abs(speed);
		Vector2 targetCell = (Vector2)transform.position + motion;
		float distance = Vector2.Distance((Vector2)transform.position, targetCell);
		Vector2 initialPosition = (Vector2)transform.position;
		isMoving = true;
		float weight = 0;
		//print("iniciando movimiento de " + position + " hacia "+targetCell+"; distancia total "+ distance+" a velocidad "+speed);
		while (weight < 1)
		{
			weight += Time.deltaTime * speed / distance;
			transform.position = (Vector3)(Vector2.Lerp(initialPosition, targetCell, weight));
			yield return null;
		}
		transform.position = (Vector3)targetCell;
		isMoving = false;
	}

	void GetInput(){
		//input guarda la entrada para el movimiento horizontal
		input = Input.GetAxisRaw("Horizontal");
		//inputAction guarda la pulsacion del boton cambiar gravedad
		//Esta implementado de forma que la accion solo puede realizarse entre movimientos, por lo que si se pulsa en medio de uno 
		//se guarda el valor para ejecutarse al acabar este
		inputAction = Input.GetButton("Action");
		//input para guardar la entrada de activar/desactivar la pausa
		inputPause = Input.GetButton("Pause");
	}

	///<sumary>
	/// Usando la funcion CanMoveTo() se comprueba si hay una colision en el tile justo bajo el personaje (en este caso se considera el sentido
	/// de la gravedad para determinar cual es el tile "bajo" el personaje)
	/// El estado se guarda en el boolean "grounded"
	///</sumary>
	void CheckGround(){
		grounded = !CanMoveTo(Vector2.up * Mathf.Sign(Physics2D.gravity.y));
	}

	///<sumary>
	/// Si pulsamos el boton de accion y podemos cambiar la gravedad la cambiamos en este método.
	///</summary>
	void Action(){		
		if(!canChangeGravity)
			return;
		//Si pulsamos el boton de accion en mitad de un movimiento, la accion se guardara para ejecutarse al final de este.
		if(inputAction && !action)
			action = true;

		if(!isMoving){
			CheckGround();
			if(grounded){
				if(action){
					Physics2D.gravity *= -1;
					inverted = Mathf.Sign(Physics2D.gravity.y) > 0? true: false;					
				}
			}			
		}
	}

	void Move(){
		float spd = speed;
		
		if(!isMoving){
			CheckGround();
			movement = Vector2.zero;			
			if(grounded){
				if(input != 0 && !action){
					movement = new Vector2(input, 0f);				
					StartCoroutine(SmoothGridMoveCoroutine(movement, spd, true));		
				}
			}else{
				movement = new Vector2(0f, Mathf.Sign(Physics2D.gravity.y));
				spd = Physics2D.gravity.y;
				StartCoroutine(SmoothGridMoveCoroutine(movement, spd, true));		
			}		
			action = false;	
		}
		ManageSprite();
	}
	
	private void ManageSprite(){
		anim.SetBool("inverted", inverted);
		anim.SetBool("moving", movement.x != 0);
		anim.SetInteger("direction", (int)Mathf.Sign(movement.x));
	}

	void DrawDebug(){
		if(!debug)
			return;
		Debug.DrawRay(transform.position, Vector2.up * Mathf.Sign(Physics2D.gravity.y) * (1f), Color.red);
	}
}
