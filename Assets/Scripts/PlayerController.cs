using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    [Header("Movement Settings")]
    public float speed = 5.0f;
    public float jumpForce = 20.0f;

    [Header("Weapon Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;

    private Rigidbody2D rBody;
    private LayerMask floorMask;
    private bool isRight = true;

    public override void OnStartLocalPlayer()
    {
        GetComponent<SpriteRenderer>().color = Color.cyan;
    }

    // Use this for initialization
    void Start () {
        rBody = GetComponent<Rigidbody2D>();
        floorMask = LayerMask.NameToLayer("Floor");
	}

    void Update()
    {
        if ((rBody.velocity.x > 0.0 && !isRight) || (rBody.velocity.x < 0.0 && isRight))
        {
            Vector2 temp = transform.localScale;
            temp.x *= -1;
            transform.localScale = temp;

            isRight = !isRight;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (!isLocalPlayer)
        {
            return;
        }

        float horiz = Input.GetAxis("Horizontal");

        Vector2 movement = new Vector2(horiz, 0);

        rBody.velocity = movement * speed;

        if (Input.GetButton("Fire1"))
        {
            CmdFire();
        }
        if (Input.GetButton("Jump"))
        {
            rBody.AddForce(new Vector2(0.0f, jumpForce));
        }
    }

    [Command]
    void CmdFire()
    {
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = transform.right * 30.0f;

        NetworkServer.Spawn(bullet);

        Destroy(bullet, 2.0f);
    }
    /*
    [Command]
    private void CmdFlip()
    {
        Vector2 temp = transform.localScale;
        temp.x *= -1;
        transform.localScale = temp;

        isRight = !isRight;
    }
    */
}
