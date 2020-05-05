﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class AvatarSetup : MonoBehaviour
{
    private PhotonView PV;
    public int characterValue;
    public GameObject myCharacter;
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        gm = GameObject.FindGameObjectWithTag("gm").GetComponent<GameManager>();
        PV = GetComponent<PhotonView>();
        if(PV.IsMine)
        {
            int actorNumber = PhotonNetwork.LocalPlayer.ActorNumber;
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedCharacter, GenerateNewColor(), actorNumber);
        }    
    }

    [PunRPC]
    void RPC_AddCharacter(int whichCharacter, int randomMaterialID, int actorID)
    {
        characterValue = whichCharacter;
        myCharacter = Instantiate(PlayerInfo.PI.allCharacters[whichCharacter], transform.position, transform.rotation, transform);
        myCharacter.GetComponent<MeshRenderer>().sharedMaterial = PlayerInfo.PI.totalMaterials[randomMaterialID];
        gameObject.GetComponent<CharacterMovement>().colorID = randomMaterialID;
        PlayerInfo.PI.alreadySelectedMaterials.Add(randomMaterialID);
        Debug.Log(actorID + " Is actor number on joining");
            if (PlayerInfo.PI.playerMaterials.ContainsKey(actorID))
            {
                PlayerInfo.PI.playerMaterials[actorID] = randomMaterialID;
            }
            else
                PlayerInfo.PI.playerMaterials.Add(actorID, randomMaterialID);
    }
    private int GenerateNewColor()
    {
        int color = Random.Range(0, PlayerInfo.PI.totalMaterials.Count);
        if (PlayerInfo.PI.alreadySelectedMaterials.Contains(color))
        {
            return GenerateNewColor();
        }
        else return color;
    }
}
