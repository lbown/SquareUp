using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharacterMovement : MonoBehaviour
{
    private PhotonView PV;
    
    public CharacterController cc;
    public float speed = 12f;
    public float gravity = -9.8f*3;
    public float jumpHeight = 2;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;


    Vector3 velocity;
    bool isGround;

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
            }

            velocity.y += gravity * Time.deltaTime;

            cc.Move(velocity * Time.deltaTime);
        }
    }

    private void OnMove()
    {
        float x = Input.GetAxis("Horizontal");

        Vector3 move = transform.right * x;

        cc.Move(move * speed * Time.deltaTime);
    }

    private void OnJump()
    {
        Debug.Log("Jump");
        if (isGround)
        {
            Debug.Log("Jump");
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
