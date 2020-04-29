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
            PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedCharacter, GenerateNewColor());
        }
    }

    [PunRPC]
    void RPC_AddCharacter(int whichCharacter, int randomMaterialID)
    {
            characterValue = whichCharacter;
            myCharacter = Instantiate(PlayerInfo.PI.allCharacters[whichCharacter], transform.position, transform.rotation, transform);
            myCharacter.GetComponent<CharacterMovement>().WhichPlayerAmI = whichCharacter;
            myCharacter.GetComponent<MeshRenderer>().sharedMaterial = PlayerInfo.PI.totalMaterials[randomMaterialID];
            PlayerInfo.PI.alreadySelectedMaterials.Add(randomMaterialID);
            if (PlayerInfo.PI.playerMaterials.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber))
            {
                PlayerInfo.PI.playerMaterials[PhotonNetwork.LocalPlayer.ActorNumber] = randomMaterialID;
            }
            else
                PlayerInfo.PI.playerMaterials.Add(PhotonNetwork.LocalPlayer.ActorNumber, randomMaterialID);
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
