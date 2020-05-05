﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CollideListener : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject bloodObj;
    public LayerMask canvasMask;
    public LayerMask groundMask;
    public GameObject cubeTransform;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnParticleCollision(GameObject other)
    {
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();
        ParticlePhysicsExtensions.GetCollisionEvents(other.GetComponent<ParticleSystem>(), gameObject, collisionEvents);
        
        if (other != null) {
            foreach (ParticleCollisionEvent p in collisionEvents) {
                GameObject blood = Instantiate(bloodObj, cubeTransform.transform);
                //int otherPlayerID = other.GetComponent<ParticleSystem>().gameObject.GetComponentInParent<CharacterMovement>().lastShotMe;
                //Debug.Log("This is other player id= " + otherPlayerID);
                //int otherPlayerColorID = PhotonView.Find(otherPlayerID).gameObject.GetComponent<PhotonPlayer>().myAvatar.GetComponent<CharacterMovement>().colorID;
                //bloodObj.GetComponent<MeshRenderer>().sharedMaterial = PlayerInfo.PI.totalMaterials[otherPlayerColorID];
                Material newMat = new Material(PlayerInfo.PI.totalMaterials[0]);
                newMat.color = (other.GetComponent<ParticleSystem>().startColor);
                Debug.Log(other.GetComponent<ParticleSystem>().startColor);
                Debug.Log(other.GetComponent<ParticleSystem>().startColor == newMat.color);
                blood.GetComponent<MeshRenderer>().sharedMaterial = newMat;
                blood.transform.position = p.intersection;
                blood.transform.rotation = Quaternion.FromToRotation(Vector3.up, p.normal);
                bool offEdge = false;
                foreach (Transform t in blood.transform)
                {
                    if (!Physics.CheckSphere(t.position, 0.01f, canvasMask) && !Physics.CheckSphere(t.position, 0.01f, groundMask))
                    {
                        offEdge = true;
                    }
                }

                if (offEdge)
                {
                    Destroy(blood);
                } else
                {
                    blood.SetActive(true);
                }

            }
        }
    }
}
