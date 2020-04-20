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
    public int numKills;
    public int numDeaths;

    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        gm = GameObject.FindWithTag("gm").GetComponent<GameManager>();
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
                PV.RPC("RPC_SetID", RpcTarget.AllBuffered, PV.ViewID);
                gm.addPlayer(myAvatar);
            }
        }
    }

    private void DieAndRespawn()
    {
        PV.RPC("RPC_GiveDeath", RpcTarget.AllBuffered);
        rumbleTimer = 0.5f;
        notWaitingForDelay = false;
        gm.removePlayer(myAvatar);
        PhotonNetwork.Destroy(myAvatar);
        StartCoroutine(SpawnDelay());
    }

    IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(2f);
        Spawn();
        notWaitingForDelay = true;
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
        }

        //temporary DieAndRespawn for testing
        if (myAvatar == null)
        {
            if(notWaitingForDelay) Spawn();
        }
        else if (myAvatar.transform.position.y <= -100)
        {
            DieAndRespawn();
        } else if (myAvatar.GetComponent<CharacterMovement>().health <= 0)
        {
            int killerID = myAvatar.GetComponent<CharacterMovement>().lastShotMe;
            if (ID != killerID)
            {
                gm.giveKill(ID);
            }
            DieAndRespawn();
        }
    }
    [PunRPC]
    public void RPC_GiveKill()
    {
        numKills += 1;
    }
    [PunRPC]
    public void RPC_GiveDeath()
    {
        numDeaths += 1;
    }
    [PunRPC]
    public void RPC_SetID(int id)
    {
        if (myAvatar == null) Spawn();
        else
        {
            myAvatar.GetComponent<CharacterMovement>().ID = id;
            ID = id;
        }
    }
}
