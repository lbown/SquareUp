using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myAvatar;

    // Start is called before the first frame update
    void Awake()
    {
        PV = GetComponent<PhotonView>();
        int spawnPicker = Random.Range(0, GameSetup.gs.spawnPoints.Length);
        if(PV.IsMine)
        {
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), GameSetup.gs.spawnPoints[spawnPicker].position, GameSetup.gs.spawnPoints[spawnPicker].rotation, 0);
        }
    }
    void Start() {
        GameObject gm = GameObject.FindWithTag("gm");
        Debug.Log(gm.name);
        gm.GetComponent<GameManager>().addPlayer(myAvatar);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
