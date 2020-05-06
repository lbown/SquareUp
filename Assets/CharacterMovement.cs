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
    public int colorID;
    
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

    private bool autoFire;
    private bool fireing;
    private int fireRate;
    private int fireCooldown;

    public float gunLength;
    public int bulDmg;
    public float bulletSpeed;
    public int ammo;
    public int bulletsPerShot;

    public float recoilAmt;
    public float bulletSize;

    public GameObject Fist;
    private int meleCooldown;

    public ParticleSystem ps;


    public void pauseTime() {
        timePaused = true;
        fireing = false;
    }

    public void unpauseTime()
    {
        timePaused = false;
    }

    void Start()
    {
        recoilAmt = 0f;
        bulletSize = .5f;
        autoFire = true;
        bulletsPerShot = 1;
        bulDmg = 20;
        bulletSpeed = 1f;
        gunLength = 1;
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
        //gun = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/TestGun"), gameObject.transform.position + new Vector3(gunLength/2 + .5f,0,0), Quaternion.identity);
        //gun.transform.parent = GunPivot;
        fireCooldown = 0;

        crown.GetComponent<MeshRenderer>().enabled = false;
        Fist.GetComponent<Fist>().ID = ID;
        Fist.GetComponent<Fist>().damage = 50;
        meleCooldown = 20;

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
                if (fireing)
                {
                    shoot();
                }
            }
            if (meleCooldown > 0)
            {
                meleCooldown -= 1;
            }
        }
    }

    private void shoot()
    {
        fireCooldown = fireRate;
        PV.RPC("RPC_Fire", RpcTarget.All, (transform.position + new Vector3(aimDirection.x * (gunLength + .5f), aimDirection.y * (gunLength + .5f), transform.position.z)), Quaternion.identity, aimDirection, PlayerInfo.PI.mySelectedCharacter, ID, bulDmg, bulletSpeed, colorID);
        impact += Vector3.Normalize(new Vector3(-aimDirection.x, -aimDirection.y, 0)) * recoilAmt;
        if (WhichPlayerAmI == 2)
        {
                PV.RPC("RPC_Fire", RpcTarget.All, (transform.position + new Vector3(aimDirection.x * -(gunLength + .5f), aimDirection.y * -(gunLength + .5f), transform.position.z)), Quaternion.identity, -1 * aimDirection, PlayerInfo.PI.mySelectedCharacter, ID, bulDmg, bulletSpeed, colorID);
        }
        ammo -= 1;
        if (ammo <= 0)
        {
            PV.RPC("RPC_DropGun", RpcTarget.AllBuffered);
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
                    health -= collision.gameObject.GetComponent<NewBulletController>().damage;
                }

                Vector3 vel = collision.gameObject.GetComponent<NewBulletController>().impulse;
                Vector3 imp = new Vector3(vel.x, vel.y, 0f);
                impact += Vector3.Normalize(imp);
                //ps.GetComponent<ParticleSystemRenderer>().material.color = collision.gameObject.GetComponent<MeshRenderer>().sharedMaterial.color;
                ps.GetComponent<ParticleSystem>().startColor = collision.gameObject.GetComponent<MeshRenderer>().sharedMaterial.GetColor("_BaseColor");
                ps.Play();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
            if (other.gameObject.tag == "RotatePowerUp" && !cubeControl.inRotation)
            {  
                if (GetComponent<PhotonView>().IsMine)
                {
                    cubeControl.TransferOwnershipOfCube();
                    cubeControl.StartRotation();
                }
                Destroy(other.gameObject);
                gm.DecrementPowerUps(true);
            }
            if (other.gameObject.tag == "Gun")
            {
                GunScript newGun = other.gameObject.GetComponent<GunScript>();
                PV.RPC("RPC_SetGunInfo", RpcTarget.AllBuffered, newGun.length, newGun.damage, newGun.shotSpeed, newGun.magazineSize, newGun.bulletsPerShot, newGun.reloadTime, newGun.automatic, newGun.name, newGun.recoil, newGun.bulletSize);
                Destroy(other.gameObject);
                gm.DecrementPowerUps(false);
            }
            if (other.gameObject.tag == "Fist" && other.gameObject != Fist)
            {
                lastShotMe = other.gameObject.GetComponent<Fist>().ID;
                other.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                health -= other.gameObject.GetComponent<Fist>().damage;
                impact += other.gameObject.GetComponent<Rigidbody>().velocity*5;
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
        if (PV.IsMine && !timePaused && gun != null)
        {
            if (!autoFire && fireCooldown == 0)
            {
                shoot();
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
            if (value.Get<Vector2>().magnitude >= 0.9 && gun == null && meleCooldown <= 0)
            {
                PV.RPC("RPC_MeleAttack", RpcTarget.AllBuffered, aimDirection,ID);
            }

            if(value.Get<Vector2>().magnitude >= 0.9 && gun != null && autoFire)
            {
                fireing = true;
            }
            else
            {
                fireing = false;
            }
        }
    }

    IEnumerator FistDrag(GameObject f)
    {
        yield return new WaitForSeconds(0.5f);
        f.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        f.transform.localPosition = new Vector3(0, 0, 0);
        f.GetComponent<SphereCollider>().enabled = false;

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
    private void ShootBullet(Vector3 pos, Quaternion dir, Vector2 aimDir, int mat, int playerID, int dmg, float bulSped, int cID)
    {
        GameObject clone = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/NewBullet"), pos, dir);
        clone.GetComponent<MeshRenderer>().sharedMaterial = PlayerInfo.PI.totalMaterials[cID];
        clone.GetComponent<Rigidbody>().velocity = Vector3.Normalize(new Vector3(aimDir.x, aimDir.y, 0)) * 30 * bulSped;
        clone.GetComponent<NewBulletController>().whoShotMe = playerID;
        clone.GetComponent<NewBulletController>().impulse = Vector3.Normalize(new Vector3(aimDir.x, aimDir.y, 0)) * 30 * bulSped;
        clone.GetComponent<NewBulletController>().damage = dmg;
        clone.transform.localScale = transform.localScale * bulletSize;
    }

    private void RotateGun(Vector2 angle)
    {
        Vector2 ang = aimDirection.normalized;
        if (ang.x < 0) GunPivot.localEulerAngles = new Vector3(0, 0, 180 + Mathf.Rad2Deg * Mathf.Atan(ang.y / ang.x));
        else GunPivot.localEulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * Mathf.Atan(ang.y / ang.x));

    }

    [PunRPC]
    private void RPC_SetGunInfo(float l, int d, float s, int m, int bps, int rt, bool a, string gunName, float recoil, float bulSize)
    {
        gunLength = l;
        bulDmg = d;
        bulletSpeed = s;
        ammo = m;
        bulletsPerShot = bps;
        fireRate = rt;
        fireing = false;
        autoFire = a;
        Destroy(gun);
        gun = Instantiate(Resources.Load<GameObject>("PhotonPrefabs/"+gunName), gameObject.transform.position + new Vector3(gunLength / 2 + .5f, 0, 0), Quaternion.identity);
        Vector3 aimdirTemp = GunPivot.localEulerAngles;
        GunPivot.localEulerAngles = new Vector3(0, 0, 0);
        gun.transform.parent = GunPivot;
        GunPivot.localEulerAngles = aimdirTemp;
        recoilAmt = recoil;
        bulletSize = bulSize;
    }
    [PunRPC] 
    private void RPC_Fire(Vector3 pos, Quaternion dir, Vector2 aimDir, int mat, int playerID, int dmg, float speed, int cID)
    {
        ShootBullet(pos, dir, aimDir, mat, playerID, dmg, speed, cID);

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
        if (id == ID)
        {
            crown.SetActive(true);
            crown.GetComponent<MeshRenderer>().enabled = true;
        }
        else crown.SetActive(false);
    }
    [PunRPC]
    public void RPC_MeleAttack(Vector2 aim,int id)
    {
        meleCooldown = 30;
        GameObject f = PhotonView.Find(id+1).gameObject.GetComponentInChildren<Fist>().gameObject;
        f.GetComponent<SphereCollider>().enabled = true;
        f.GetComponent<Rigidbody>().AddForce(aim * 1000);
        StartCoroutine(FistDrag(f));
    }
    [PunRPC]
    public void RPC_DropGun()
    {
        Destroy(gun);
    }
}
