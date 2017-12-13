using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBullet : MonoBehaviour {

    public GameObject hitFX;
    public float speed;
    internal Cannon.ShootDir dir;
    void Update()
    {
        if(Physics2D.Raycast(transform.position, dir == Cannon.ShootDir.left ? Vector2.left : Vector2.right, 0.01f, LayerMask.GetMask("Ground")))
        {
            Destroy(this);
        }
        else
        {
            var toAdd = dir == Cannon.ShootDir.left ? new Vector3(-speed*Time.deltaTime, 0, 0) : new Vector3(speed*Time.deltaTime, 0, 0);
            transform.position += toAdd;
        }
    }

    void OnDestroy()
    {
        Instantiate(hitFX, transform.position, transform.rotation);
    }
}
