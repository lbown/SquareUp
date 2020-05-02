using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.IO;
using Photon.Realtime;

public class CharacterMovement : MonoBehaviour
{
    public Material myBulletColor;
    
    private PhotonView PV;
    private GameObject cube;
    private CubeController cubeControl;
    
    public CharacterController cc;
    public float speed = 50f;
    public float gravity = -9.8f;
    public float jumpHeight = 2;

    public Transform groundCheck;
    public Transform wallCheck;
    public Transform GunPivot;
    public float groundDistance = 0.6f;
    public LayerMask groundMask;
    public bool timePaused = false;

    public int ID;

    public Vector3 velocity;
    Vector2 lMovement;
    bool isGround;
    private int jumpNum;
    public GameObject gameManager;
    public GameManager gm;
    private Vector2 aimDirection;

    private Vector3 impact;

    public int WhichPlayerAmI;

    public int health;
    public int startingHP = 100;

    private int cooldown;
    private int levitate;
    private int invulnerable;
    private Vector3 portalPos;

    public int lastShotMe;

    public GameObject gun;
    public GameObject crown;

    public int numKills;
    public int numDeaths;

    private bool fireing;
    private int fireRate;
    private int fireCooldown;

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
        health = startingHP;
        cooldown = 0;
        invulnerable = 0;
        portalPos = new Vector3(0f, 0f, -100f);
        impact = Vector2.zero;
        numDeaths = 0;
        numKills = 0;

        fireing = false;
        fireRate = 10;

        gun = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/TestGun"), gameObject.transform.position + new Vector3(1,0,0), gameObject.transform.rotation);
        gun.transform.parent = GunPivot;
        fireCooldown = 0;

        crown.GetComponent<MeshRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine && !timePaused && !gm.timePaused)
        {
            isGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
            if(WhichPlayerAmI == 1 && !isGround)
            {
                isGround = Physics.CheckSphere(wallCheck.position, 0.6f, groundMask);
            }

            if (isGround && velocity.y < 0)
            {
                velocity.y = 0f;
                jumpNum = 2;
            }
            else if (levitate == 0)
            {
                velocity.y += gravity * Time.deltaTime;
            }

            

            //cc.Move(velocity * Time.deltaTime);

            Move();
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y,0);

        }
    }

    private void FixedUpdate()
    {
        if (PV.IsMine)
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
            if (fireCooldown > 0)
            {
                fireCooldown -= 1;
            }
            if (fireCooldown == 0)
            {
                fireCooldown = fireRate;
                if (fireing)
                {
                    PV.RPC("RPC_Fire", RpcTarget.All, (transform.position + new Vector3(aimDirection.x * 1.5f, aimDirection.y * 1.5f, transform.position.z)), Quaternion.identity, aimDirection, PlayerInfo.PI.mySelectedCharacter, ID);
                    if (WhichPlayerAmI == 2)
                    {
                        PV.RPC("RPC_Fire", RpcTarget.All, (transform.position + new Vector3(aimDirection.x * -1.5f, aimDirection.y * -1.5f, transform.position.z)), Quaternion.identity, -1 * aimDirection, PlayerInfo.PI.mySelectedCharacter, ID);
                    }
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "bullet" )
        {
            if (collision.gameObject.GetComponent<NewBulletController>().whoShotMe != ID)
            {
                //TODO: Bullet needs to know which 
                lastShotMe = collision.gameObject.GetComponent<NewBulletController>().whoShotMe;
                if (invulnerable == 0)
                {
                    health -= 20;
                }

                Vector3 vel = collision.gameObject.GetComponent<NewBulletController>().impulse;
                Vector3 imp = new Vector3(vel.x, vel.y, 0f);
                impact += Vector3.Normalize(imp);
            }
            else {
                Debug.Log("Stop hitting urself");
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
        if (!timePaused)
        {
            Vector3 move = new Vector3(lMovement.x, velocity.y, 0f);
            if (levitate > 0)
            {
                move.y = 0f;
            }
            cc.Move((move * speed + impact * 10f) * Time.deltaTime);
            impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);
        }
    }

    private void OnMove(InputValue value)
    {
        if (PV.IsMine && !timePaused)
        {
                lMovement = value.Get<Vector2>();
                float x = Input.GetAxis("Horizontal");
        }
    }

    private void OnJump(InputValue val)
    {
        if (PV.IsMine && !timePaused)
        {
            if (jumpNum > 0)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpNum -= 1;
            }
        }
    }

    //NEW SHOOT FUNCTION
    private void OnShoot(InputValue value)
    {
        if (PV.IsMine && !timePaused)
        {
            if (fireing)
            {
                fireing = false;
            }
            else
            {
                fireing = true;
            }
        }
    }
    private void OnAim(InputValue value)
    {
        if (PV.IsMine && !timePaused)
        {
            aimDirection = value.Get<Vector2>().normalized;
            RotateGun(aimDirection);
            //PV.RPC("RPC_Aim", RpcTarget.AllBuffered, aimDirection);
        }
    }
    private void OnAbility()
    {
        if (PV.IsMine && !timePaused)
        {
            if (cooldown == 0)
            {
                if (WhichPlayerAmI == 0)
                {
                    Vector3 move = new Vector3(lMovement.x, lMovement.y, 0f);
                    impact += Vector3.Normalize(move) * 4;
                    cooldown = 60;
                }
                if (WhichPlayerAmI == 1)
                {
                    //invulnerable = 60;
                    if (portalPos.z < 0f)
                    {   
                        portalPos = cc.transform.position;
                        cooldown = 30;
                    }
                    else
                    {
                        cc.enabled = false;
                        cc.transform.position = portalPos;
                        cc.enabled = true;
                        portalPos.z = -100f;
                        cooldown = 30;
                        velocity.y = 0;
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
    private void ShootBullet(Vector3 pos, Quaternion dir, Vector2 aimDir, int mat, int playerID)
    {
        GameObject clone = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/NewBullet"), pos, dir);
        clone.GetComponent<MeshRenderer>().sharedMaterial = GetComponentInChildren<MeshRenderer>().sharedMaterial;
        clone.GetComponent<Rigidbody>().velocity = Vector3.Normalize(new Vector3(aimDir.x, aimDir.y, 0)) * 30;
        clone.GetComponent<NewBulletController>().whoShotMe = playerID;
        clone.GetComponent<NewBulletController>().impulse = Vector3.Normalize(new Vector3(aimDir.x, aimDir.y, 0)) * 30;
    }

    private void RotateGun(Vector2 angle)
    {
        Vector2 ang = aimDirection.normalized;
        if (ang.x < 0) GunPivot.localEulerAngles = new Vector3(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan(ang.y / ang.x));
        else GunPivot.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan(ang.y / ang.x));

    }


    [PunRPC] 
    private void RPC_Fire(Vector3 pos, Quaternion dir, Vector2 aimDir, int mat, int playerID)
    {
        ShootBullet(pos, dir, aimDir, mat, playerID);

    }
    [PunRPC]
    private void RPC_Aim(Vector2 angle)
    {
        RotateGun(angle);
    }
    [PunRPC]
    public void RPC_GiveKill()
    {
        numKills += 1;
    }
    [PunRPC]
    public void RPC_GiveDeath()
    {
        numDeaths += 1;
    }
    [PunRPC]
    public void RPC_IsWinner(int id)
    {
        Debug.Log("in IsWinner");
        if (id == ID)
        {
            crown.SetActive(true);
            crown.GetComponent<MeshRenderer>().enabled = true;
        }
        else crown.SetActive(false);
    }
}
