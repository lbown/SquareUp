using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

public class CharSelectionController : MonoBehaviour
{
    public Canvas canvas;
    public GameObject panel;
    public bool readyToSpawn;
    private GameManager gm;

    private void Awake()
    {
        readyToSpawn = false;
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnClickCharacterSelection(int whichCharacter)
    {
        if (PlayerInfo.PI != null)
        {
            readyToSpawn = true;
            PlayerInfo.PI.mySelectedCharacter = whichCharacter;
            PlayerPrefs.SetInt("SelectedCharacter", whichCharacter);
            panel.SetActive(false);
            Debug.Log(panel.activeSelf);
        }
    }
}
