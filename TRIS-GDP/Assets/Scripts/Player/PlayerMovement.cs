using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour {
	[SerializeField] public float speed = 4f;
	[SerializeField] public bool debug;	
	[SerializeField] public bool canChangeGravity;	
	[SerializeField] public DeviceType actualDevice;
	[SerializeField] public float movementThreshold = 0.3f;
	[SerializeField] public CheckPoint actualCheckpoint;

	float input;
	bool inputAction;
	bool inputPause;
	bool action;
	bool isMoving = false;
	bool grounded = false;
	bool dead = false;
	int gravityCoolDown = 0;
	//Vector2 position;
	Vector2 movement;
	Animator anim;
	Camera mainCam;
	bool inverted;

	bool accel = true;

	// Use this for initialization
	void Awake () {
		anim = GetComponent<Animator>();
		anim.speed = 0.25f * speed;
		actualDevice = SystemInfo.deviceType;
		Input.gyro.enabled = true;
		mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

		if(debug){
			actualDevice = DeviceType.Handheld;
		}
	}
	
	// Update is called once per frame
	void Update () {				
		GetInput();
		Action();		
		CheckDeath();		
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
		//inputAction guarda la pulsacion del boton cambiar gravedad
		//Esta implementado de forma que la accion solo puede realizarse entre movimientos, por lo que si se pulsa en medio de uno 
		//se guarda el valor para ejecutarse al acabar este
		//input para guardar la entrada de activar/desactivar la pausa

		if(actualDevice == DeviceType.Handheld){
			if(accel){
				input = Mathf.Abs(Input.acceleration.x) >= movementThreshold? Mathf.Sign(Input.acceleration.x): 0f;	
			}else{
				input = Mathf.Abs(Input.gyro.rotationRateUnbiased.x) >= movementThreshold? Mathf.Sign(Input.gyro.rotationRateUnbiased.x): 0f;	
			}

			inputAction = false;
			if(Input.touchCount == 1){
				TouchPhase touch = Input.touches[0].phase;
				if(touch == TouchPhase.Ended && touch != TouchPhase.Canceled){
					inputAction = true;
				}
			}

		}else{
			input = Input.GetAxisRaw("Horizontal");		
			inputAction = Input.GetButton("Action");		
			inputPause = Input.GetButton("Pause");
		}		
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
				if(action && gravityCoolDown == 0){
					Physics2D.gravity *= -1;
					inverted = Mathf.Sign(Physics2D.gravity.y) > 0? true: false;	
					gravityCoolDown = 16;				
				}
			}			
		}
		gravityCoolDown = gravityCoolDown-1 < 0? 0: gravityCoolDown-1;
	}

	void Move(){	
		ManageSprite();	
		if(!dead && !isMoving){
			CheckGround();
			movement = Vector2.zero;			
			if(grounded){
				RepositionCamera();				
				if(input != 0 && !action){
					movement = new Vector2(input, 0f);				
					StartCoroutine(SmoothGridMoveCoroutine(movement, speed, true));		
				}
			}else{
				movement = new Vector2(0f, Mathf.Sign(Physics2D.gravity.y));
				StartCoroutine(SmoothGridMoveCoroutine(movement, Physics2D.gravity.y, true));		
			}		
			action = false;	
		}		
	}

	private void CheckDeath(){
		float yOffset = mainCam.GetComponentInParent<PixelPerfectCamera>().getYOffset();
		Debug.DrawLine(mainCam.transform.position - new Vector3(100, yOffset, -10), mainCam.transform.position - new Vector3(-100, yOffset, -10), Color.yellow);
		Debug.DrawLine(mainCam.transform.position + new Vector3(-100, yOffset, 10), mainCam.transform.position + new Vector3(100, yOffset, 10), Color.yellow);
		float pivotOffset = 0.5f;
		if(transform.position.y - pivotOffset < mainCam.transform.position.y - yOffset || transform.position.y + pivotOffset > mainCam.transform.position.y + yOffset){
			Die();
		}
	}

	public void Die(){
		if(dead)
			return;
		dead = true;
		StopAllCoroutines();
		isMoving = false;
		Time.timeScale = 0.4f;
		ManageSprite();
		for(int i = 0; i < 1; i++){
			mainCam.GetComponentInParent<CameraMovement>().Shake();
		}
	}

	private void Restart(){
		dead = false;		
		Time.timeScale = 1f;	
		ResetPositionToActiveCheckpoint();
		ManageSprite();
	}
	
	private void ManageSprite(){
		SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
		if(inverted){
			if(!mySprite.flipY){
				mySprite.flipY = true;
			}
		}else{
			if(mySprite.flipY){
				mySprite.flipY = false;
			}
		}
		anim.SetBool("dead", dead);
		//anim.SetBool("inverted", inverted);
		anim.SetBool("moving", movement.x != 0 && isMoving);
		anim.SetInteger("direction", (int)Mathf.Sign(movement.x));
	}

	void RepositionCamera(){
		GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().ChangeY(transform.position.y + 0.5f * Mathf.Sign(Physics2D.gravity.y));
	}

	void DrawDebug(){
		if(!debug)
			return;
		Debug.DrawRay(transform.position, Vector2.up * Mathf.Sign(Physics2D.gravity.y) * (1f), Color.red);
	}

	public void ResetToPosition(Vector2 pos, bool inverted)
    {		
        this.inverted = inverted;
        this.transform.position = pos;
    }

    internal void ResetPositionToActiveCheckpoint()
    {
        /*LevelController lv = GameObject.Find("LevelController").GetComponent<LevelController>();
        Physics2D.gravity = lv.activeCP.isInverted()?-lv.startingGravity:lv.startingGravity;
		ResetToPosition(lv.activeCP.gameObject.transform.position, lv.activeCP.isInverted());*/
		Physics2D.gravity = new Vector3 (0, actualCheckpoint.isInverted()? Mathf.Abs(Physics2D.gravity.y): -Mathf.Abs(Physics2D.gravity.y), 0);
        ResetToPosition(actualCheckpoint.transform.position, actualCheckpoint.isInverted());
    }

	internal void refreshCheckpoint(CheckPoint cp){
		actualCheckpoint = cp;
	}


	//metodos debug
	public void setAccel(bool state){
		accel = state;
	}

	public void setThreshold(float val){
		movementThreshold = val;
	}


}
