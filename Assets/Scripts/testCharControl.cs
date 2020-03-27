using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class testCharControl : MonoBehaviour
{
    private PhotonView PV;
    private CharacterController cc;
    [SerializeField] private float moveSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(PV.IsMine)
        {
            BasicMovement();
        }
    }

    void BasicMovement()
    {
        if(Input.GetKey(KeyCode.D))
        {
            cc.Move(transform.right * Time.deltaTime * moveSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            cc.Move(-transform.right * Time.deltaTime * moveSpeed);
        }
    }
}
