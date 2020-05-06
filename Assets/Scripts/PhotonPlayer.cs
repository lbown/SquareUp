using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using XInputDotNetPure;
using UnityEngine.InputSystem.LowLevel;
using System.Runtime.CompilerServices;
using UnityEngine.InputSystem;

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
    public bool dead = false;

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
            
                myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerAvatar"), GameSetup.gs.spawnPoints[spawnPicker].position, GameSetup.gs.spawnPoints[spawnPicker].rotation, 0);
                GameSetup.gs.myAvatar = myAvatar;
                PV.RPC("RPC_SetAvatarID", RpcTarget.AllBuffered, myAvatar.GetComponent<PhotonView>().ViewID, PV.ViewID);
                gm.addPlayer(myAvatar);

        }
    }

    public void SetAvatarInfo(int avatarID)
    {
        PV.RPC("RPC_SetAvatarID", RpcTarget.AllBuffered, avatarID, PV.ViewID);
    }

    private void DieAndRespawn()
    {
        if (gm.activeGame)
        {
            PhotonView.Find(ID + 1).RPC("RPC_GiveDeath", RpcTarget.AllBuffered);
        }
        rumbleTimer = 0.5f;
        notWaitingForDelay = false;
        PV.RPC("RPC_OnDeath", RpcTarget.AllBuffered, myAvatar.GetComponent<PhotonView>().ViewID);
        gm.findWinner();
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
        PV.RPC("RPC_OnRespawn", RpcTarget.AllBuffered, myAvatar.GetComponent<PhotonView>().ViewID, GameSetup.gs.spawnPoints[spawnPicker].position);
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
        if (gm.activeGame)
        {
            PhotonView.Find(killer + 1).RPC("RPC_GiveKill", RpcTarget.AllBuffered);
        }
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
        if (avatar != null) myAvatar = avatar;
        else {
            Debug.Log("Can't set a null avatar!");
        }
    }
    [PunRPC]
    private void RPC_SetPhotonPlayerID(int id)
    {
        ID = id;
    }
    [PunRPC]
    private void RPC_OnDeath(int id)
    {
        if (!dead)
        {
            dead = true;
            GameObject avatar = PhotonView.Find(id).gameObject;
            foreach (MeshRenderer item in avatar.GetComponentsInChildren<MeshRenderer>()) {
                if(item.gameObject.activeSelf)  item.enabled = false;
            }
            avatar.GetComponent<PlayerInput>().enabled = false;
            avatar.GetComponent<CharacterController>().enabled = false;
            avatar.GetComponent<CapsuleCollider>().enabled = false;
            avatar.GetComponent<CharacterMovement>().enabled = false;
            avatar.GetComponent<CharacterMovement>().Fist.GetComponent<SphereCollider>().enabled = false;
        }
    }
    [PunRPC]
    private void RPC_OnRespawn(int id, Vector3 pos)
    {
        GameObject avatar = PhotonView.Find(id).gameObject;
        avatar.transform.position = pos;
        foreach (MeshRenderer item in avatar.GetComponentsInChildren<MeshRenderer>())
        {
            if(item.gameObject.activeSelf) item.enabled = true;
        }
        avatar.GetComponent<PlayerInput>().enabled = true;
        avatar.GetComponent<CharacterController>().enabled = true;
        avatar.GetComponent<CapsuleCollider>().enabled = true;
        avatar.GetComponent<CharacterMovement>().enabled = true;
        avatar.GetComponent<CharacterMovement>().health = avatar.GetComponent<CharacterMovement>().startingHP;
        avatar.GetComponent<CharacterMovement>().velocity.y = 0f;
        avatar.GetComponent<CharacterMovement>().Fist.GetComponent<SphereCollider>().enabled = true;
        if(avatar.GetComponent<CharacterMovement>().gun != null)
        {
            Destroy(avatar.GetComponent<CharacterMovement>().gun);
        }
        dead = false;
    }
}

