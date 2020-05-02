using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour
{
    public static GameSetup gs;
    public Material[] playerColors;
    public Transform[] spawnPoints;
    public Transform[] powerUpLocations;
    public GameObject myAvatar;

    private void OnEnable()
    {
        if(GameSetup.gs == null)
        {
            GameSetup.gs = this;
        } 
    }

    public void DisconnectPlayer() 
    {
        StartCoroutine(DisconnectAndLoad());
    }

    IEnumerator DisconnectAndLoad()
    {
        PhotonNetwork.Disconnect();
        while(PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        SceneManager.LoadScene(MultiplayerSettings.multiplayerSettings.menuScene);
    }
}
