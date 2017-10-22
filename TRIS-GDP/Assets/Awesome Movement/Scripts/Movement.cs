using System.Collections;
using UnityEngine;

/// <summary>
/// this component is a movement library that allows the gameObject to see if it can move to a position or not and move
/// to the position based on the result. It also has methods for moving as much as possible toward the posion.
/// There are coroutines for grid/tile like movement so you can call a method and tell the object to move to a specific point with a specific
/// speed. You can check if it is moving toward the position or not and easily code grid based movement logic.
/// </summary>
public class Movement : MonoBehaviour
{
	/// <summary>
	/// Different shapes which are supported for casts against the world for movement checks and queries.
	/// Each Movement component chooses one of these modes for its casts to check if it can go somewhere
	/// or not.
	/// </summary>
	public enum ShapeMode { Capsule, Sphere }

	/// <summary>
	/// Layers which movement functions check collisions against.
	/// </summary>
	public LayerMask layerMask = -1;

	/// <summary>
	/// The desired shape of the moving object's casts for collision checks.
	/// </summary>
	public ShapeMode shapeMode;

	/// <summary>
	/// Radius of the object that we are moving.
	/// </summary>
	public float radius = 0.5f;

	/// <summary>
	/// Height of the capsule if we use the capsule shape for casting.
	/// </summary>
	public float height = 2;

	/// <summary>
	/// The offset from object pivot that we cast from to see if we collide with things or not.
	/// </summary>
	public Vector3 offsetFromPivot;

	/// <summary>
	/// Is the object being moved using one of the Smooth movement methods which move
	/// objects over time in a coroutine.
	/// </summary>
	/// <remarks>
	/// You can check this value to see if the object is still moving or you can initiate the next
	/// move based on user input/AI/...
	/// </remarks>
	public bool isMovingUsingSmoothMove
	{
		get
		{
			return isMoving;
		}
	}

	private RaycastHit[] hits; //the array to hold all raycast info.
	private Transform myTransform; //we will cache the transform component in it.
	private Rigidbody myRigidbody; //we will cache the rigidbody component in it.
	private CharacterController myCharacter; //the character controller reference
	private Vector3 motion; //the vector of the motion
	private Vector3 targetCell; //this is the target position in global space used in grid movement
	private bool isMoving = false; //are we moving toward a grid (i,e running a coroutine)
	private Vector3 safeMotion; //we use this vector in MOveSafe to get the distance that we can move

	private void Awake()
	{
		//cache all components needed in this script
		myTransform = transform;
		myRigidbody = GetComponent<Rigidbody>();
		myCharacter = GetComponent<CharacterController>();
		motion = Vector3.zero; //we are not moving at first
		safeMotion = Vector3.zero;
	}

	/// <summary>
	/// Set movement properties based on the attached collider to the GameObject
	/// </summary>
	/// <remarks>
	/// You usually might want to use this if you add the component to a GameObject at runtime and don't
	/// want to set properties like shapeMode and radius yourself.
	/// </remarks>
	public void SetMovementPropertiesFromCollider()
	{
		//check if there is any collider attached to set the radius based on the collider.
		if (myCharacter != null)
		{
			radius = myCharacter.radius * Mathf.Max(myTransform.localScale.x, myTransform.localScale.z);
			offsetFromPivot = myCharacter.center;
			height = myCharacter.height;
			shapeMode = ShapeMode.Capsule;
		}
		SphereCollider sph = GetComponent<SphereCollider>();
		if (sph != null)
		{
			radius = sph.radius * Mathf.Max(myTransform.localScale.x, Mathf.Max(myTransform.localScale.y, myTransform.localScale.z));
			offsetFromPivot = sph.center;
			shapeMode = ShapeMode.Sphere;
			return;
		}
		CapsuleCollider cap = GetComponent<CapsuleCollider>();
		if (cap != null)
		{
			radius = cap.radius * Mathf.Max(myTransform.localScale.x, myTransform.localScale.z);
			offsetFromPivot = cap.center;
			height = cap.height;
			shapeMode = ShapeMode.Capsule;
			return;
		}
		BoxCollider box = GetComponent<BoxCollider>();
		if (box != null)
		{
			radius = Mathf.Max(box.size.x, box.size.z);
			offsetFromPivot = box.center;
			shapeMode = ShapeMode.Sphere;
			return;
		}
	}

	//The 3 methods below are just helper methods for moving along cardinal axes.
	//They use our main Move() function and are all in local space.

	/// <summary>
	/// Moves the object alongside local x axis.
	/// </summary>
	/// <param name="distance">The distance that you want to move the object.</param>
	/// <returns>If the movement is done or a collision prevented it.</returns>
	public bool MoveAlongX(float distance)
	{
		motion = Vector3.zero;
		motion.x = distance;
		return Move(myTransform.TransformPoint(motion) - myTransform.position);
	}

	/// <summary>
	/// Moves the object alongside local y axis.
	/// </summary>
	/// <param name="distance">The distance that you want to move the object.</param>
	/// <returns>If the movement is done or a collision prevented it.</returns>
	public bool MoveAlongY(float distance)
	{
		motion = Vector3.zero;
		motion.y = distance;
		return Move(myTransform.TransformPoint(motion) - myTransform.position);
	}

	/// <summary>
	/// Moves the object alongside local z axis.
	/// </summary>
	/// <param name="distance">The distance that you want to move the object.</param>
	/// <returns>If the movement is done or a collision prevented it.</returns>
	public bool MoveAlongZ(float distance)
	{
		motion = Vector3.zero;
		motion.z = distance;
		return Move(myTransform.TransformPoint(motion) - myTransform.position);
	}

	/// <summary>
	/// Moves the GameObject as much as possible in direction of the specified motion.
	/// </summary>
	/// <param name="motion">The motion that you want to take place</param>
	/// <returns>the distance traveled</returns>
	/// <remarks>The object will move the whole motion if it doesn't collide with anything, otherwise it will
	/// move as much as it can before a collision accurs.</remarks>
	public Vector3 TryMove(Vector3 motion)
	{
		if (CanMoveTo(motion, ref safeMotion))
		{
			MoveGameObjectToPosition(motion);
		}
		else
		{
			MoveGameObjectToPosition(safeMotion);
		}
		return safeMotion;
	}

	private void MoveGameObjectToPosition(Vector3 motion)
	{
		if (myCharacter)
		{
			myCharacter.SimpleMove(motion);
		}
		else if (myRigidbody)
		{
			myRigidbody.MovePosition(myRigidbody.position + motion);
		}
		else
		{
			myTransform.position += motion;
		}
	}

	private void SetGameObjectPosition(Vector3 position)
	{
		if (myRigidbody)
		{
			myRigidbody.MovePosition(position);
		}
		else
		{
			myTransform.position = position;
		}
	}

	/// <summary>
	/// Moves the GameObject as much as possible in direction of the specified motion.
	/// </summary>
	/// <param name="x">x component of the motion vector</param>
	/// <param name="y">y component of the motion vector</param>
	/// <param name="z">z motion of the motion vector</param>
	/// <returns>the distance traveled</returns>
	/// <remarks>The object will move the whole motion if it doesn't collide with anything, otherwise it will
	/// move as much as it can before a collision accurs.</remarks>
	public Vector3 TryMove(float x, float y, float z)
	{
		motion.x = x;
		motion.y = y;
		motion.z = z;
		return TryMove(motion);
	}

	/// <summary>
	/// Moves the GameObject using motion vector if the movmeent doesn't cause any collisions.
	/// </summary>
	/// <param name="x">x component of the motion vector</param>
	/// <param name="y">y component of the motion vector</param>
	/// <param name="z">z component of the motion vector</param>
	/// <returns>If the movement taken place or not</returns>
	/// <remarks>
	/// Colliion check is done using CanMove family of methods based on shapeMode and other set movement properties.
	/// </remarks>
	public bool Move(float x, float y, float z)
	{
		motion.x = x;
		motion.y = y;
		motion.z = z;
		return Move(motion);
	}

	/// <summary>
	/// Moves the GameObject using motion vector if the movmeent doesn't cause any collisions.
	/// </summary>
	/// <param name="motion">The motion vector that specifies the amount of movement</param>
	/// <returns>If the movement taken place or not</returns>
	/// <remarks>
	/// Colliion check is done using CanMove family of methods based on shapeMode and other set movement properties.
	/// </remarks>
	public bool Move(Vector3 motion)
	{
		if (CanMoveTo(motion))
		{
			MoveGameObjectToPosition(motion);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Checks if the GameObject can do the specified motion without collisions or not.
	/// </summary>
	/// <param name="motion">The movement which the GameObject wants to do</param>
	/// <param name="hits">The method will populate this array with the info returned from the raycast, you can see what collisions will accure exactly by examining the array</param>
	/// <returns>If any collision accurs or not</returns>
	/// <remarks>The cast will be done based on shapeMode and other properties of the object.</remarks>
	public bool CanMoveTo(Vector3 motion, ref RaycastHit[] hits)
	{
		if (shapeMode == ShapeMode.Sphere)
			hits = Physics.SphereCastAll(myTransform.position + offsetFromPivot, radius, motion, motion.magnitude, layerMask);
		else if (shapeMode == ShapeMode.Capsule)
			hits = Physics.CapsuleCastAll(transform.position + offsetFromPivot - Vector3.up * height / 2f, transform.position + Vector3.up * height / 2f, radius, motion, motion.magnitude, layerMask);
		foreach (RaycastHit hit in hits)
		{
			if (hit.transform.GetInstanceID() != myTransform.GetInstanceID())
			{
				//if there is something in our way other than the ourself then we can not move to the position.
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Checks if the GameObject can do the specified motion without collisions or not.
	/// </summary>
	/// <param name="motion">The movement which the GameObject wants to do</param>
	/// <returns>If any collision accurs or not</returns>
	/// <remarks>The cast will be done based on shapeMode and other properties of the object.</remarks>
	public bool CanMoveTo(Vector3 motion)
	{
		return CanMoveTo(motion, ref hits);
	}

	/// <summary>
	/// Checks if the GameObject can do the specified motion without collisions or not.
	/// </summary>
	/// <param name="x">x component of the motion vector</param>
	/// <param name="y">y component of the motion vector</param>
	/// <param name="z">z component of the motion vector</param>
	/// <returns>If any collision accurs or not</returns>
	/// <remarks>The cast will be done based on shapeMode and other properties of the object.</remarks>
	public bool CanMoveTo(float x, float y, float z)
	{
		motion.x = x;
		motion.y = y;
		motion.z = z;
		return CanMoveTo(motion);
	}


	/// <summary>
	/// Checks if the GameObject can do the specified motion without collisions or not. If collisions happen, then it
	/// fills the safeMotion variable with a motioin vector toward the original motion which the GameObject can
	/// move using it without any collisions happening.
	/// </summary>
	/// <param name="x">The x component of the motion that you want to do with the GameObject and are checking it for collisions</param>
	/// <param name="y">The y component of the motion that you want to do with the GameObject and are checking it for collisions</param>
	/// <param name="z">The z component of the motion that you want to do with the GameObject and are checking it for collisions</param>
	/// <param name="safeMotion">The method will fill this with the biggest safe motion with no collisions if the full motion can not be performed</param>
	/// <returns>If you can move the full motion without any collisions or not</returns>
	public bool CanMoveTo(float x, float y, float z, ref Vector3 safeMotion)
	{
		motion.x = x;
		motion.y = y;
		motion.z = 0;
		return CanMoveTo(motion, ref safeMotion);
	}

	/// <summary>
	/// Checks if the GameObject can do the specified motion without collisions or not. If collisions happen, then it
	/// fills the safeMotion variable with a motioin vector toward the original motion which the GameObject can
	/// move using it without any collisions happening.
	/// </summary>
	/// <param name="motion">The motion that you want to do with the GameObject and are checking it for collisions</param>
	/// <param name="safeMotion">The method will fill this with the biggest safe motion with no collisions if the full motion can not be performed</param>
	/// <param name="hits">The method will fill this with the hits returned from the cast</param>
	/// <returns>If you can move the full motion without any collisions or not</returns>
	public bool CanMoveTo(Vector3 motion, ref Vector3 safeMotion, ref RaycastHit[] hits)
	{
		if (shapeMode == ShapeMode.Sphere)
			hits = Physics.SphereCastAll(myTransform.position + offsetFromPivot, radius, motion, motion.magnitude, layerMask);
		else if (shapeMode == ShapeMode.Capsule)
			hits = Physics.CapsuleCastAll(transform.position + offsetFromPivot - Vector3.up * height / 2f, transform.position + Vector3.up * height / 2f, radius, motion, motion.magnitude, layerMask);
		foreach (RaycastHit hit in hits)
		{
			if (hit.transform.GetInstanceID() != myTransform.GetInstanceID())
			{
				safeMotion = ((myTransform.position - hit.point).magnitude - ((shapeMode == ShapeMode.Sphere) ? radius : Mathf.Max(radius, height))) * motion.normalized;
				//if there is something in our way other than the ground then please don't go there
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Checks if the GameObject can do the specified motion without collisions or not. If collisions happen, then it
	/// fills the safeMotion variable with a motioin vector toward the original motion which the GameObject can
	/// move using it without any collisions happening.
	/// </summary>
	/// <param name="motion">The motion that you want to do with the GameObject and are checking it for collisions</param>
	/// <param name="safeMotion">The method will fill this with the biggest safe motion with no collisions if the full motion can not be performed</param>
	/// <returns>If you can move the full motion without any collisions or not</returns>
	public bool CanMoveTo(Vector3 motion, ref Vector3 safeMotion)
	{
		return CanMoveTo(motion, ref safeMotion, ref hits);
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
	public IEnumerator SmoothGridMoveCoroutine(Vector3 motion, float speed, bool checkCollision)
	{
		if (checkCollision)
		{
			if (!CanMoveTo(motion)) yield break;
		}
		targetCell = myTransform.position + motion;
		var distance = Vector3.Distance(myTransform.position, targetCell);
		var initialPosition = myTransform.position;
		isMoving = true;
		float weight = 0;
		while (weight < 1)
		{
			weight += Time.deltaTime * speed / distance;
			SetGameObjectPosition(Vector3.Lerp(initialPosition, targetCell, weight));
			yield return null;
		}
		myTransform.position = targetCell;
		isMoving = false;
	}

	/// <summary>
	/// Moves the gameObject using the motion vector with the specified speed in meters per second.
	/// </summary>
	/// <param name="motion">The motion that you want to happen</param>
	/// <param name="speed">Speed of the movement in meters per second. So if the motion vector's length is 2 and you set 1 as speed, it will take 2 seconds for the object to arrive at destination</param>
	/// <param name="checkCollision">If true then the method will check for collisions before moving but not while moving toward there</param>
	/// <remarks>This method is good for moving objects in grid/tile based games smoothly.
	/// You can give the motions to the object in size of the tile and also give it an appropreate speed to arrive in time but still the object
	/// only stops at grid boundaries while having a smooth motion visually.
	/// <para>The <c>isMovingUsingSmoothMove property is true while the move is being executed.</c></para>
	/// </remarks>
	public void SmoothGridMove(Vector3 motion, float speed, bool checkColision)
	{
		StartCoroutine(SmoothGridMoveCoroutine(motion, speed, checkColision));
	}

	/// <summary>
	/// Moves the gameObject using the motion vector with the specified speed in meters per second.
	/// </summary>
	/// <param name="x">The x component of the motion that you want to happen</param>
	/// <param name="y">The y component of the motion that you want to happen</param>
	/// <param name="z">The z component of the motion that you want to happen</param>
	/// <param name="speed">Speed of the movement in meters per second. So if the motion vector's length is 2 and you set 1 as speed, it will take 2 seconds for the object to arrive at destination</param>
	/// <param name="checkCollision">If true then the method will check for collisions before moving but not while moving toward there</param>
	/// <remarks>This method is good for moving objects in grid/tile based games smoothly.
	/// You can give the motions to the object in size of the tile and also give it an appropreate speed to arrive in time but still the object
	/// only stops at grid boundaries while having a smooth motion visually.
	/// <para>The <c>isMovingUsingSmoothMove property is true while the move is being executed.</c></para>
	/// </remarks>
	public void SmoothGridMove(float x, float y, float z, float speed, bool checkColision)
	{
		Vector3 motion = new Vector3(x, y, z);
		StartCoroutine(SmoothGridMoveCoroutine(motion, speed, checkColision));
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		if (shapeMode == ShapeMode.Sphere)
			Gizmos.DrawWireSphere(transform.position + offsetFromPivot, radius);
		else if (shapeMode == ShapeMode.Capsule)
		{
			Gizmos.DrawWireSphere(transform.position + offsetFromPivot - Vector3.up * height / 2f + Vector3.up * radius, radius);
			//Gizmos.color = Color.yellow;
			Vector3 from = transform.position + offsetFromPivot - Vector3.up * height / 2f;
			Vector3 to = transform.position + offsetFromPivot + Vector3.up * height / 2f;
			from.y += radius;
			to.y -= radius;

			Vector3 ft = from;
			Vector3 tt = to;
			ft.x -= radius;
			tt.x -= radius;
			Gizmos.DrawLine(ft, tt);

			ft = from;
			tt = to;
			ft.x += radius;
			tt.x += radius;
			Gizmos.DrawLine(ft, tt);

			ft = from;
			tt = to;
			ft.z -= radius;
			tt.z -= radius;
			Gizmos.DrawLine(ft, tt);

			ft = from;
			tt = to;
			ft.z += radius;
			tt.z += radius;
			Gizmos.DrawLine(ft, tt);

			//Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position + offsetFromPivot + Vector3.up * height / 2f - Vector3.up * radius, radius);
		}
	}
}