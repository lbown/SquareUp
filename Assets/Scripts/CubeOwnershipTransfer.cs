using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CubeOwnershipTransfer : MonoBehaviourPun, IPunOwnershipCallbacks
{
    private CubeController cubeControl;
    void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
        cubeControl = gameObject.GetComponent<CubeController>();
    }
    
    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != base.photonView) return;
        if (cubeControl.inRotation) return;
        base.photonView.TransferOwnership(requestingPlayer);
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        if (targetView != base.photonView) return;
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump")) base.photonView.RequestOwnership();
    }
}
