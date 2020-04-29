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
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedCharacter, PlayerInfo.PI.myRandomColor);
        }
    }

    [PunRPC]
    void RPC_AddCharacter(int whichCharacter, int randomMaterialID)
    {
            characterValue = whichCharacter;
            myCharacter = Instantiate(PlayerInfo.PI.allCharacters[whichCharacter], transform.position, transform.rotation, transform);
            myCharacter.GetComponent<CharacterMovement>().WhichPlayerAmI = whichCharacter;
            myCharacter.GetComponent<MeshRenderer>().sharedMaterial = PlayerInfo.PI.totalMaterials[randomMaterialID];
            gm.playerMaterials.Add(PhotonNetwork.LocalPlayer.ActorNumber, PlayerInfo.PI.totalMaterials[randomMaterialID]);
            //gm.totalMaterials.Remove(gm.totalMaterials[randomMaterialID]);
            //gm.playerMaterials[PhotonNetwork.LocalPlayer.ActorNumber] = myCharacter.GetComponent<MeshRenderer>().sharedMaterial;
            //PlayerInfo.PI.allMaterials.Remove(PlayerInfo.PI.allMaterials[0]);
    }
}
