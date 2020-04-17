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
    public CharSelectionController charSelect;
    public int numKills;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        gm = GameObject.FindWithTag("gm").GetComponent<GameManager>();
        myAvatar = null;
        charSelect = GameObject.Find("MenuController").GetComponent<CharSelectionController>();
    }

    public void Spawn()
    {
        if (charSelect.readyToSpawn)
        {
            int spawnPicker = Random.Range(0, GameSetup.gs.spawnPoints.Length);
            
            if (PV.IsMine)
            {
                ID = PV.ViewID;
                myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), GameSetup.gs.spawnPoints[spawnPicker].position, GameSetup.gs.spawnPoints[spawnPicker].rotation, 0);
                PV.RPC("RPC_SetParent", RpcTarget.AllBuffered);
                gm.addPlayer(myAvatar);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //temporary respawn for testing
        if (myAvatar == null)
        {
            Spawn();
        }
        else if (myAvatar.transform.position.y <= -100)
        {
            gm.removePlayer(myAvatar);
            PhotonNetwork.Destroy(myAvatar);
            Spawn();
        } else if (myAvatar.GetComponent<CharacterMovement>().health <= 0)
        {
            int killerID = myAvatar.GetComponent<CharacterMovement>().lastShotMe;
            if (ID != killerID)
            {
                gm.giveKill(ID);
                //killer.RPC_GiveKill();
            }
            gm.removePlayer(myAvatar);
            PhotonNetwork.Destroy(myAvatar);
            Spawn();
        }
    }
    [PunRPC]
    public void RPC_GiveKill()
    {
        numKills += 1;
    }
    [PunRPC]
    public void RPC_SetParent()
    {
        foreach(GameObject player in GameObject.FindGameObjectsWithTag("PhotonPlayer"))
        {
            if(player.GetComponent<PhotonPlayer>().ID == ID)
            {
                myAvatar.transform.SetParent(player.transform);
            }
        }
    }
}
