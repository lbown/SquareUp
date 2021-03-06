﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CollideListener : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private GameObject[] bloodObjs;
    public LayerMask canvasMask;
    public LayerMask groundMask;
    public GameObject cubeTransform;
    public Material mat;
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
                mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                //mat.color = (other.GetComponent<ParticleSystem>().startColor);
                mat.SetColor("_BaseColor", other.GetComponent<ParticleSystem>().startColor);
                //mat.SetColor("_EmissionColor", other.GetComponent<ParticleSystem>().startColor);
                //mat.EnableKeyword("_EMISSION");
                GameObject bloodObj = bloodObjs[Random.Range(0, bloodObjs.Length)];
                GameObject blood = Instantiate(bloodObj, cubeTransform.transform);
                foreach (MeshRenderer b in blood.GetComponentsInChildren<MeshRenderer>()) {
                    b.sharedMaterial = mat;
                }
                //int otherPlayerID = other.GetComponent<ParticleSystem>().gameObject.GetComponentInParent<CharacterMovement>().lastShotMe;
                //Debug.Log("This is other player id= " + otherPlayerID);
                //int otherPlayerColorID = PhotonView.Find(otherPlayerID).gameObject.GetComponent<PhotonPlayer>().myAvatar.GetComponent<CharacterMovement>().colorID;
                //bloodObj.GetComponent<MeshRenderer>().sharedMaterial = PlayerInfo.PI.totalMaterials[otherPlayerColorID];

                //Debug.Log(other.GetComponent<ParticleSystem>().startColor);
                //Debug.Log(other.GetComponent<ParticleSystem>().startColor == newMat.color);

                //Debug.Log(other.GetComponent<ParticleSystem>().startColor == blood.GetComponent<MeshRenderer>().sharedMaterial.color);
                blood.transform.position = p.intersection;
                blood.transform.Rotate(Vector3.up, Random.Range(0, 360));
                blood.transform.rotation = Quaternion.FromToRotation(Vector3.up, p.normal);
                float scaleFactor = Random.RandomRange(0.75f, 1.25f);
                blood.transform.localScale.Scale(new Vector3(scaleFactor, 1, scaleFactor));
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Fist")
        {
            other.gameObject.GetComponent<Rigidbody>().velocity = other.gameObject.GetComponent<Rigidbody>().velocity*-.5f;
        }
    }
}
