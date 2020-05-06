using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;
using System.Numerics;

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
        gameObject.GetComponent<CharacterMovement>().Fist.GetComponent<MeshRenderer>().sharedMaterial = PlayerInfo.PI.totalMaterials[randomMaterialID];
        gameObject.GetComponent<CharacterMovement>().Fist.GetComponent<SphereCollider>().enabled = false;
        PlayerInfo.PI.alreadySelectedMaterials.Add(randomMaterialID);
            if (PlayerInfo.PI.playerMaterials.ContainsKey(actorID))
            {
                PlayerInfo.PI.playerMaterials[actorID] = randomMaterialID;
            }
            else
                PlayerInfo.PI.playerMaterials.Add(actorID, randomMaterialID);
    }
    private int GenerateNewColor()
    {
        int maxColors = Mathf.Min(MultiplayerSettings.multiplayerSettings.maxPlayers, PlayerInfo.PI.totalMaterials.Count);
        int color = Random.Range(0, maxColors);
        if (PlayerInfo.PI.alreadySelectedMaterials.Count >= PlayerInfo.PI.totalMaterials.Count)
        {
            return color;
        }
        else if (PlayerInfo.PI.alreadySelectedMaterials.Contains(color))
        {
            return GenerateNewColor();
        }
        else return color;
    }
}
