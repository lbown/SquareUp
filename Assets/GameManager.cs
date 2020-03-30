﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    private PhotonView PV;
    private List<GameObject> players;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        players = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void addPlayer(GameObject p) {
        players.Add(p);
    }

    public void pauseTime() {
        foreach (GameObject player in players) {
            player.GetComponent<CharacterMovement>().pauseTime();
        }
    }
    public void unpauseTime()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<CharacterMovement>().unpauseTime();
        }
    }
}
