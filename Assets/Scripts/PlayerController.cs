using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    [Header("Movement Settings")]
    public float speed = 5.0f;

    [Header("Weapon Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float waitInterval = 0.5f;

    private Rigidbody2D rBody;
    private float counter = 0.0f;

    public override void OnStartLocalPlayer()
    {
        GetComponent<SpriteRenderer>().color = Color.cyan;
    }

    // Use this for initialization
    void Start () {
        rBody = GetComponent<Rigidbody2D>();
	}

    void Update()
    {
        if(Input.GetButton("Fire1") && counter > waitInterval)
        {
            CmdFire();
        }

        counter += Time.deltaTime;
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
	}

    [Command]
    void CmdFire()
    {
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Rigidbody2D>().velocity = Vector2.right * 30.0f;
        Destroy(bullet, 2.0f);

        NetworkServer.Spawn(bullet);

        counter = 0.0f;
    }
}
