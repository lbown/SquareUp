using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    private PhotonView PV;
    
    public CharacterController cc;
    public GameObject bullet;
    public float speed = 50f;
    public float gravity = -9.8f;
    public float jumpHeight = 2;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public bool timePaused = false;

    Vector3 velocity;
    Vector2 lMovement;
    bool isGround;
    private int jumpNum;
    public GameObject gameManager;
    public GameManager gm;
    private Vector2 aimDirection;

    public void pauseTime() {
        timePaused = true;
    }

    public void unpauseTime()
    {
        timePaused = false;
    }

    void Start()
    {
        gameManager = GameObject.FindWithTag("gm");
        PV = GetComponent<PhotonView>();
        gm = gameManager.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine && !gm.timePaused)
        {
            isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGround && velocity.y < 0)
            {
                velocity.y = -2f;
                jumpNum = 2;
            }

            velocity.y += gravity * Time.deltaTime;

            cc.Move(velocity * Time.deltaTime);

            Move();
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y,0);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "bullet")
        {
            cc.Move(collision.gameObject.GetComponent<Rigidbody>().velocity / 2f * -1f);
            Destroy(collision.gameObject);
        }
    }

    private void Move()
    {
        Vector3 move = transform.right * lMovement.x;

        cc.Move(move * speed * Time.deltaTime);
    }

    private void OnMove(InputValue value)
    {
        if (PV.IsMine)
        {
            lMovement = value.Get<Vector2>();
            float x = Input.GetAxis("Horizontal");
        }
    }

    private void OnJump(InputValue val)
    {
        if (PV.IsMine)
        {
            if (jumpNum == 2)
            {
                if (!isGround)
                {
                    jumpNum = 1;
                }
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

            }
            if (jumpNum == 1)
            {
                 velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity / 2);
            }
            jumpNum -= 1;
        }
    }

    private void OnShoot(InputValue value)
    {
        if (PV.IsMine && aimDirection != new Vector2(0f,0f))
        {
            GameObject clone;
            clone = Instantiate(bullet, transform.position + new Vector3 (aimDirection.x*1.5f, aimDirection.y*1.5f, transform.position.z), Quaternion.identity);
            clone.GetComponent<Rigidbody>().velocity = new Vector3(aimDirection.x,aimDirection.y,0)*30;
        }
    }
    private void OnAim(InputValue value)
    {
        if (PV.IsMine)
        {
            aimDirection = value.Get<Vector2>();
        }
    }
}
