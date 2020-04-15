using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.IO;

public class CharacterMovement : MonoBehaviour
{
    private PhotonView PV;
    private GameObject cube;
    private CubeController cubeControl;
    
    public CharacterController cc;
    public GameObject bullet;
    public float speed = 50f;
    public float gravity = -9.8f;
    public float jumpHeight = 2;

    public Transform groundCheck;
    public float groundDistance = 0.6f;
    public LayerMask groundMask;
    public bool timePaused = false;

    Vector3 velocity;
    Vector2 lMovement;
    bool isGround;
    private int jumpNum;
    public GameObject gameManager;
    public GameManager gm;
    private Vector2 aimDirection;

    private Vector3 impact;

    public int WhichPlayerAmI;

    public int health;

    private int cooldown;
    private int levitate;
    private int invulnerable;
    private int invert;

    public PhotonPlayer lastShotMe;

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
        cube = GameObject.Find("Cube");
        cubeControl = cube.GetComponent<CubeController>();
        WhichPlayerAmI = GetPlayerSkin();
        levitate = 0;
        health = 100;
        cooldown = 0;
        invulnerable = 0;
        invert = 1;
        impact = Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine && !gm.timePaused)
        {
            isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGround && velocity.y < 0)
            {
                velocity.y = 0f;
                jumpNum = 2;
            }
            else if (levitate == 0)
            {
                velocity.y += gravity*invert * Time.deltaTime;
            }

            

            //cc.Move(velocity * Time.deltaTime);

            Move();
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y,0);
        }
    }

    private void FixedUpdate()
    {
        if (cooldown > 0)
        {
            cooldown -= 1;
        }
        if (levitate > 0)
        {
            levitate -= 1;
        }
        if (invulnerable > 0)
        {
            invulnerable -= 1;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "bullet" )
        {
            if (collision.gameObject.GetComponent<BulletController>().whoShotMe != gameObject.GetComponent<PhotonPlayer>()) ;
            {
                //TODO: Bullet needs to know which 
                lastShotMe = collision.gameObject.GetComponent<BulletController>().whoShotMe;
                if (invulnerable == 0)
                {
                    health -= 20;
                }
                collision.gameObject.GetComponent<BulletController>().Remove(collision.gameObject);
                Vector3 vel = collision.gameObject.GetComponent<Rigidbody>().velocity;
                Vector3 imp = new Vector3(vel.x, vel.y, 0f);
                impact += Vector3.Normalize(imp);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (PV.IsMine)
        {
            if (other.gameObject.tag == "RotatePowerUp" && !cubeControl.inRotation)
            {
                cubeControl.TransferOwnershipOfCube();
                GameObject.Find("RotateCubePowerUp(Clone)").GetComponent<PhotonView>().RequestOwnership();
                cubeControl.StartRotation();
                gm.ResetRotatePowerUpTimer();
            }
        }
    }

    private void Move()
    {
        Vector3 move = new Vector3(lMovement.x,velocity.y,0f);
        if (levitate > 0)
        {
            move.y = 0f;
        }
        cc.Move((move * speed + impact*-10f) * Time.deltaTime);
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
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
                 velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            jumpNum -= 1;
        }
    }

    private void OnShoot(InputValue value)
    {

        if (PV.IsMine && (Mathf.Abs(aimDirection.x) > 0.5 || Mathf.Abs(aimDirection.y) > 0.5))
        {
            GameObject clone;
            clone = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Bullet"), transform.position + new Vector3 (aimDirection.x*1.5f, aimDirection.y*1.5f, transform.position.z), Quaternion.identity);
            clone.GetComponent<Rigidbody>().velocity = Vector3.Normalize(new Vector3(aimDirection.x,aimDirection.y,0))*30;
            clone.GetComponent<BulletController>().whoShotMe = gameObject.GetComponentInParent<PhotonPlayer>();
        }
    }
    private void OnAim(InputValue value)
    {
        if (PV.IsMine)
        {
            aimDirection = value.Get<Vector2>();
        }
    }
    private void OnAbility()
    {
        if (PV.IsMine)
        {
            if (cooldown == 0)
            {
                if (WhichPlayerAmI == 0)
                {
                    Vector3 move = new Vector3(lMovement.x, lMovement.y, 0f);
                    impact += Vector3.Normalize(move) * -4;
                    cooldown = 60;
                }
                if (WhichPlayerAmI == 1)
                {
                    //invulnerable = 60;
                    if (invert == 1)
                    {
                        invert = -1;
                        velocity.y = 0;
                        cooldown = 10;
                    }
                    else
                    {
                        invert = 1;
                        velocity.y = 0;
                        cooldown = 180;
                    }
                }
                if (WhichPlayerAmI == 2)
                {
                    levitate = 60;
                    jumpNum = 2;
                    cooldown = 180;
                }
                if (WhichPlayerAmI == 3)
                {
                    cc.enabled = false;
                    Vector3 pos = cc.transform.position;
                    Vector3 move = Vector3.Normalize(new Vector3(lMovement.x, lMovement.y, 0f))*5;
                    cc.transform.position = new Vector3(pos.x + move.x, pos.y + move.y, pos.z);
                    cooldown = 30;
                    cc.enabled = true;
                }
            }
        }
    }

    private int GetPlayerSkin()
    {
        return PlayerInfo.PI.mySelectedCharacter;
    }
}
