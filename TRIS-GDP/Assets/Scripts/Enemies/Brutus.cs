using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brutus : MonoBehaviour 
{
    private GameObject player; 
    private Animator anim;
    public float fallSpeed = -12;
    public float ascendSpeed = 4;

    private float speed = 0;
    private bool grounded = false;

	private enum State
    {
        idle = 0,
        fall= 1,
        wait = 2,
        ascend = 3,

    }

    private State state = State.idle;
    private bool isMoving;
    private Vector2 movement;

    void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
        switch (state)
        {
            case State.idle:
                Vector3 tris_pos = player.transform.position;
                Vector3 brutus_pos = transform.position;
                if (Mathf.Abs(tris_pos.x - brutus_pos.x) <= 1.1 && tris_pos.y <= brutus_pos.y && tris_pos.y < brutus_pos.y + 7)
                {
                    // TODO: Sonido

                    state = State.fall;
                    speed = fallSpeed;
                    anim.SetTrigger("Fall");
                }
            break;

            case State.fall:
                if(!isMoving && grounded)
                {
                    state = State.wait;
                    speed = 0;
                    anim.SetTrigger("Wait");
                }
                
            break;

            case State.ascend:
                if(!isMoving && grounded)
                {
                    state = State.idle;
                    speed = 0;
                    anim.SetTrigger("Idle");
                }
                
            break;
        }

        Move();
            
	}

    public void Ascend()
    {
        speed = ascendSpeed;
        state = State.ascend;
    }

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

    private bool CanMoveTo(Vector2 motion)
    {
		RaycastHit2D hit;
		hit = Physics2D.Raycast((Vector2)transform.position, motion, motion.magnitude, LayerMask.GetMask("Ground"));
		if(hit.collider != null){
			return false;
		}else{
			return true;
		}
	}

    public void CheckGround(){
		grounded = !CanMoveTo(Vector2.up * Mathf.Sign(speed));
	}

    void Move()
    {
		float spd = speed;
		
		if(!isMoving)
        {
			CheckGround();

            movement = new Vector2(0, Mathf.Sign(spd));				
			StartCoroutine(SmoothGridMoveCoroutine(movement, spd, true));
		}
	}
}
