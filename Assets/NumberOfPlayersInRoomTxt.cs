using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class NumberOfPlayersInRoomTxt : MonoBehaviour
{
    private TMP_Text thisText;
    // Start is called before the first frame update
    void Start()
    {
        thisText = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        thisText.text = "Number of players in lobby: " + PhotonNetwork.PlayerList.Length;
    }
}
