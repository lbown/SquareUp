using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using XInputDotNetPure;
using UnityEngine.InputSystem.LowLevel;
using System.Runtime.CompilerServices;

public class PhotonPlayer : MonoBehaviour
{
    private ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();
    
    //Information for controller rumble
    private PlayerIndex playerIndex;
    private GamePadState state;
    private GamepadState prevState;
    [SerializeField] private float rumbleTimer;
    //End of controller rumble information

    private bool notWaitingForDelay;

    private PhotonView PV;
    public GameObject myAvatar;
    private GameManager gm; 
    public int ID;
    public CharSelectionController charSelect;
    private bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        gm = GameObject.FindWithTag("gm").GetComponent<GameManager>();
        PV.RPC("RPC_SetPhotonPlayerID", RpcTarget.AllBuffered, PV.ViewID);
        myAvatar = null;
        charSelect = GameObject.Find("MenuController").GetComponent<CharSelectionController>();
        rumbleTimer = -1;
        notWaitingForDelay = true;
    }

    private void ControllerRumble()
    {
        if (rumbleTimer >= 0)
        {
            GamePad.SetVibration(playerIndex, 1f, 1f);
        }
        else GamePad.SetVibration(playerIndex, 0f, 0f);
    }

    public void Spawn()
    {
        if (charSelect.readyToSpawn)
        {
            int spawnPicker = Random.Range(0, GameSetup.gs.spawnPoints.Length);
            
            if (PV.IsMine)
            {
                myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), GameSetup.gs.spawnPoints[spawnPicker].position, GameSetup.gs.spawnPoints[spawnPicker].rotation, 0);
                GameSetup.gs.myAvatar = myAvatar;
                PV.RPC("RPC_SetAvatarID", RpcTarget.AllBuffered, myAvatar.GetComponent<PhotonView>().ViewID, PV.ViewID);
                gm.addPlayer(myAvatar);
            }
        }
    }

    private void DieAndRespawn()
    {
        PhotonView.Find(ID + 1).RPC("RPC_GiveDeath", RpcTarget.AllBuffered);
        rumbleTimer = 0.5f;
        notWaitingForDelay = false;
        PV.RPC("RPC_OnDeath", RpcTarget.AllBuffered, myAvatar.GetComponent<PhotonView>().ViewID);
        StartCoroutine(SpawnDelay());
    }

    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(2f);
        Respawn();
        notWaitingForDelay = true;
    }

    private void Respawn() {
        int spawnPicker = Random.Range(0, GameSetup.gs.spawnPoints.Length);
        myAvatar.transform.position = GameSetup.gs.spawnPoints[spawnPicker].position;
        PV.RPC("RPC_OnRespawn", RpcTarget.AllBuffered, myAvatar.GetComponent<PhotonView>().ViewID);
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            if (rumbleTimer > 0)
            {
                rumbleTimer -= Time.deltaTime;
            }
            ControllerRumble();

            //temporary DieAndRespawn for testing
            if (myAvatar == null)
            {
                if (notWaitingForDelay) Spawn();
            }
            else if (myAvatar.transform.position.y <= -100)
            {
                if (!dead)
                {
                    DieAndRespawn();
                }
            }
            else if (myAvatar.GetComponent<CharacterMovement>().health <= 0)
            {
                int killerID = myAvatar.GetComponent<CharacterMovement>().lastShotMe;
                if (!dead)
                {
                    if (ID != killerID)
                    {
                        giveKill(killerID);
                    }
                    DieAndRespawn();
                }
            }
        }
    }

    private void giveKill(int killer)
    {
        PhotonView.Find(killer+1).RPC("RPC_GiveKill", RpcTarget.AllBuffered);
    }

    public void DisconnectMe()
    {
        GameSetup.gs.myAvatar = myAvatar;
        GameSetup.gs.DisconnectPlayer();
    }

    [PunRPC]
    public void RPC_SetAvatarID(int avatarViewID, int id)
    {
        GameObject avatar = PhotonView.Find(avatarViewID).gameObject;
        avatar.GetComponent<CharacterMovement>().ID = id;
    }
    [PunRPC]
    private void RPC_SetPhotonPlayerID(int id)
    {
        ID = id;
    }
    [PunRPC]
    private void RPC_OnDeath(int id)
    {
        GameObject avatar = PhotonView.Find(id).gameObject;
        avatar.GetComponentInChildren<MeshRenderer>().enabled = false;
        avatar.GetComponent<CharacterMovement>().pauseTime();
        avatar.GetComponent<CapsuleCollider>().enabled = false;
        dead = true;
    }
    [PunRPC]
    private void RPC_OnRespawn(int id)
    {
        GameObject avatar = PhotonView.Find(id).gameObject;
        avatar.GetComponentInChildren<MeshRenderer>().enabled = true;
        avatar.GetComponent<CharacterMovement>().unpauseTime();
        avatar.GetComponent<CapsuleCollider>().enabled = true;
        avatar.GetComponent<CharacterMovement>().health = avatar.GetComponent<CharacterMovement>().startingHP;
        avatar.GetComponent<CharacterMovement>().velocity.y = 0f;
        dead = false;
    }
}

