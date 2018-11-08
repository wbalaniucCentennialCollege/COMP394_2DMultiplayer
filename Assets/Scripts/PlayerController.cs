using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour {

    // Public variables
    [Header("Movement Settings")]
    public float speed = 5.0f;
    public float jumpForce = 20.0f;
    public LayerMask floorMask;

    [Header("Weapon Settings")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float weaponDamage = 30.0f;

    // Private variables
    private Rigidbody2D rBody;
    private bool isRight = true;
    private float distToGround;
    private bool isGrounded = false;

    public override void OnStartLocalPlayer()
    {
        GetComponent<SpriteRenderer>().color = Color.cyan;
    }

    // Use this for initialization
    void Start () {
        rBody = GetComponent<Rigidbody2D>();
        floorMask = LayerMask.NameToLayer("Floor");
        distToGround = GetComponent<Collider2D>().bounds.extents.y;
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
        movement.x *= speed;

        if(Input.GetButton("Jump") && isGrounded)
        {
            movement.y = jumpForce;
            isGrounded = false;
        }

        rBody.velocity = movement;

        if (Input.GetButton("Fire1"))
        {
            CmdFire();
        }
        /*
        if(Input.GetButton("Jump") && isGrounded)
        {
            rBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
            isGrounded = false;
        }*/
    }

    [Command]
    void CmdFire()
    {
        var bullet = (GameObject)Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);

        bullet.GetComponent<Rigidbody2D>().velocity = transform.right * weaponDamage * transform.localScale.x;

        NetworkServer.Spawn(bullet);

        Destroy(bullet, 2.0f);
    }

    /*
    private bool IsGrounded()
    {
        bool isGrounded = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + distToGround), -Vector2.up, -0.5f, floorMask);
        Debug.Log("Player is grounded? " + isGrounded);
        return isGrounded;
    }
    */

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - distToGround - 0.5f));
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            Debug.Log("Jumping");
            isGrounded = true;   
        }
        else
        {
            Debug.Log("Not Jumping!");
            isGrounded = false;
        }
    }
}
