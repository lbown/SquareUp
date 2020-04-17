﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class BulletController : MonoBehaviour
{
    // Start is called before the first frame update
    private float life;

    public int whoShotMe;

    public Vector3 impulse;

    private PhotonView PV;
    void Start()
    {
        PV = GetComponent<PhotonView>();
        life = 0f;
        
    }

    // Update is called once per frame
    void Update()
    {
        life += Time.deltaTime;
        if(life > 5 || GetComponent<Rigidbody>().velocity.magnitude <= 5)
        {
            NetRemove();
        }
        Vector3 vel = gameObject.GetComponent<Rigidbody>().velocity;
        GetComponent<Rigidbody>().velocity = new Vector3(vel.x, vel.y, 0f);
    }

    public void NetRemove()
    {
        if (PV.IsMine) PhotonNetwork.Destroy(gameObject);
    }

    //    private void OnCollisionExit(Collision collision)
    //   {
    //        Destroy(gameObject);
    //   }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 8)
        {
            NetRemove();
        }
    }

    [PunRPC]
    public void syncBullet_RPC(int howLong, int firerer, Vector3 speed)
    {
        life = howLong;
        whoShotMe = firerer;
        impulse = speed;
    }

    
}
