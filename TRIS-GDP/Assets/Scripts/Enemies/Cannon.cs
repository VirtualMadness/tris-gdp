using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

	private GameObject tris;

    public int cooldown = 60;

    public GameObject bulletPrefab;

    public enum ShootDir
    {
        left = 0, 
        right = 1,
        stop = 2
    }

    private ShootDir dir = ShootDir.stop;

    private bool broken;
	void Start () {
		tris = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () 
    {
		if(tris == null)
        {
            tris = GameObject.FindGameObjectWithTag("Player");
            return;
        }

        if(tris.transform.position.x > transform.position.x)
        {
            dir = ShootDir.right;
        }
        else if(tris.transform.position.x < transform.position.x)
        {
            dir = ShootDir.left;
        }
        else
        {
            dir = ShootDir.stop;
        }

        if(dir == ShootDir.stop && tris.transform.position.y == transform.position.y + 1)
        {
            
            broken = true;
        }
        
        if(!broken) StartCoroutine(Shoot());
	}

    IEnumerator Shoot()
    {
        var cd = cooldown;

        while(cd > 0)
        {
            cd--;
            cooldown = cd;
            yield break;
        }
        cooldown = 60;

        GameObject bullet;
        // Shoot
        if(dir != ShootDir.stop)
        {
            if(dir == ShootDir.left)
            {
                bullet = Instantiate(bulletPrefab, transform.position + new Vector3(-0.8f, 0, 0), transform.rotation);
                bullet.GetComponent<CannonBullet>().dir = dir;
            }
            else if(dir == ShootDir.right)
            {
                bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0.8f, 0, 0), transform.rotation);
                bullet.GetComponent<CannonBullet>().dir = dir;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(broken) return;
        BreakIfTrisOrBrutus(other);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(broken) return;
        BreakIfTrisOrBrutus(other);
    }

    void BreakIfTrisOrBrutus(Collider2D other)
    {
        if((other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Brutus")))
        {
            GameObject tris = other.gameObject;

            SpriteRenderer sr = tris.GetComponent<SpriteRenderer>();

            if((tris.transform.position.y == this.transform.position.y + 1 && !sr.flipY) 
            || (tris.transform.position.y == this.transform.position.y - 1 && sr.flipY)
            )
            {
                broken = true;
                Debug.Log("broken");
            }
        }
    }
}
