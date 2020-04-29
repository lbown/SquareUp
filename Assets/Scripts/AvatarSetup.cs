using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public class AvatarSetup : MonoBehaviour
{
    private PhotonView PV;
    public int characterValue;
    public GameObject myCharacter;
    public Material myBulletColor;
    private GameManager gm;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        if(PV.IsMine)
        {
            gm = GameObject.FindWithTag("gm").GetComponent<GameManager>();
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedCharacter);
        }
    }

    [PunRPC]
    void RPC_AddCharacter(int whichCharacter)
    {
        characterValue = whichCharacter;
        int materialID = Random.Range(0, gm.totalMaterials.Count);
        myCharacter = Instantiate(PlayerInfo.PI.allCharacters[whichCharacter], transform.position, transform.rotation, transform);
        myCharacter.GetComponent<CharacterMovement>().WhichPlayerAmI = whichCharacter;
        myCharacter.GetComponent<MeshRenderer>().sharedMaterial = gm.totalMaterials[materialID];
        gm.playerMaterials.Add(PhotonNetwork.LocalPlayer.ActorNumber, gm.totalMaterials[materialID]);
        gm.totalMaterials.Remove(gm.totalMaterials[materialID]);
        //gm.playerMaterials[PhotonNetwork.LocalPlayer.ActorNumber] = myCharacter.GetComponent<MeshRenderer>().sharedMaterial;
        //PlayerInfo.PI.allMaterials.Remove(PlayerInfo.PI.allMaterials[0]);

    }
}
