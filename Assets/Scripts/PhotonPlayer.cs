using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PhotonPlayer : MonoBehaviour
{
    private PhotonView PV;
    public GameObject myAvatar;
    private GameManager gm; 
    public int ID;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        gm = GameObject.FindWithTag("gm").GetComponent<GameManager>();
        Spawn();
    }

    private void Spawn()
    {
        int spawnPicker = Random.Range(0, GameSetup.gs.spawnPoints.Length);
        ID = PV.GetInstanceID();
        if (PV.IsMine)
        {            
            myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), GameSetup.gs.spawnPoints[spawnPicker].position, GameSetup.gs.spawnPoints[spawnPicker].rotation, 0);
            Material pMat = myAvatar.GetComponent<Material>();
            pMat = GameSetup.gs.p1Mat;
            gm.addPlayer(myAvatar);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //temporary respawn for testing
        if(myAvatar.transform.position.y <= -100)
        {
            gm.removePlayer(myAvatar);
            PhotonNetwork.Destroy(myAvatar);
            Spawn();
        }
    }

}
