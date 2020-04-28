﻿using System.IO;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    public static PhotonRoom room;
    private PhotonView PV;
    public int multiplayerScene;
    private int currentScene;

    private void Awake()
    {
        if (PhotonRoom.room == null)
        {
            PhotonRoom.room = this;
        }
        else
        {
            if (PhotonRoom.room != this)
            {
                Destroy(PhotonRoom.room.gameObject);
                PhotonRoom.room = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
        PV = GetComponent<PhotonView>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        PhotonNetwork.AddCallbackTarget(this);
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.RemoveCallbackTarget(this);
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (!PhotonNetwork.IsMasterClient) return;
        StartGame();
    }

    void StartGame()
    {
        PhotonNetwork.LoadLevel(multiplayerScene);
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        currentScene = scene.buildIndex;
        if(currentScene == multiplayerScene)
        {
            CreatePlayer();
        }
    }

    private void CreatePlayer()
    {
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonPlayer"), Vector3.zero, Quaternion.identity);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log(otherPlayer.ActorNumber);
        Material returnMat = GameObject.FindWithTag("gm").GetComponent<GameManager>().playerMaterials[otherPlayer.ActorNumber];
        PlayerInfo.PI.allMaterials.Add(returnMat);
        GameObject.FindWithTag("gm").GetComponent<GameManager>().playerMaterials.Remove(otherPlayer.ActorNumber);
        base.OnPlayerLeftRoom(otherPlayer);
    }

}
