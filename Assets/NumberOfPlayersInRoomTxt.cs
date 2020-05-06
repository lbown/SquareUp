using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class NumberOfPlayersInRoomTxt : MonoBehaviour
{
    private TMP_Text thisText;
    private GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        thisText = GetComponent<TMP_Text>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        thisText.text = gm.readyPlayers + "/" + PhotonNetwork.PlayerList.Length + " players ready to start";
    }
}
