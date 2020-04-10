using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CubeOwnershipTransfer : MonoBehaviourPun, IPunOwnershipCallbacks
{
    private CubeController cubeControl;
    private GameObject gameManager;
    GameManager gm;
    void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
        cubeControl = gameObject.GetComponent<CubeController>();
        gameManager = GameObject.FindWithTag("gm");
        gm = gameManager.GetComponent<GameManager>();
    }
    
    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != base.photonView) return;
        if (cubeControl.inRotation) return;
        base.photonView.TransferOwnership(requestingPlayer);
        gameManager.GetComponent<PhotonView>().TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView != base.photonView) return;
    }

}
