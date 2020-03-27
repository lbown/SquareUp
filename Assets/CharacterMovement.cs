using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;

public class CharacterMovement : MonoBehaviour
{
    private PhotonView PV;
    
    public CharacterController cc;
    public float speed = 50f;
    public float gravity = -9.8f;
    public float jumpHeight = 2;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;


    Vector3 velocity;
    Vector2 lMovement;
    bool isGround;
    private int jumpNum;

    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
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
                if (isGround)
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }
            if (jumpNum == 1)
            {
                 velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity / 2);
            }
            jumpNum -= 1;
        }
    }
}
