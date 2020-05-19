using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonLobby : MonoBehaviourPunCallbacks
{
    public static PhotonLobby lobby;

    [SerializeField] private GameObject startButton, cancelButton, loadingTxt;

    private void Awake()
    {
        lobby = this;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to the " + PhotonNetwork.CloudRegion + " server.");
        PhotonNetwork.AutomaticallySyncScene = true;
        startButton.SetActive(true);
        loadingTxt.SetActive(false);
    }

    public void QuickStart()
    {
        startButton.SetActive(false);
        cancelButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom(null, (byte)MultiplayerSettings.multiplayerSettings.maxPlayers);
        Debug.Log("Quick Start.");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a room.");
        CreateRoom();
    }

    private void CreateRoom()
    {
        Debug.Log("Creating Room.");
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSettings.multiplayerSettings.maxPlayers };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps);
        Debug.Log(randomRoomNumber);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room. Attempting again...");
        CreateRoom();
    }

    public void QuickCancel()
    {
        cancelButton.SetActive(false);
        startButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }
}
