using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myAvatar;
    public int ID;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        int spawnPicker = Random.Range(0, GameSetup.gs.spawnPoints.Length);
        ID = PV.GetInstanceID();
        if(PV.IsMine)
        {
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), GameSetup.gs.spawnPoints[spawnPicker].position, GameSetup.gs.spawnPoints[spawnPicker].rotation, 0);
            Material pMat = myAvatar.GetComponent<Material>();
            pMat = GameSetup.gs.p1Mat;
            GameObject gm = GameObject.FindWithTag("gm");
            gm.GetComponent<GameManager>().addPlayer(myAvatar);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
