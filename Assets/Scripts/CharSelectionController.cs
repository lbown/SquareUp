using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class CharSelectionController : MonoBehaviour
{
    public Canvas canvas;
    public GameObject panel;
    public Button[] buttons;
    public List<string> charNames;
    public TMP_Text timer, waitingTxt, readyTxt;
    public bool readyToSpawn;
    private GameManager gm;

    private void Awake()
    {
        readyToSpawn = false;
        timer.enabled = false;
        waitingTxt.enabled = false;
        readyTxt.color = Color.green;
        readyTxt.enabled = false;
        buttons = panel.GetComponentsInChildren<Button>();
        charNames = new List<string> { "Dash", "Sticky", "Double", "Warp" };
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void OnClickCharacterSelection(int whichCharacter)
    {
        if (PlayerInfo.PI != null)
        {
            waitingTxt.enabled = true;
            readyTxt.enabled = true;
            readyToSpawn = true;
            foreach(Button btn in buttons)
            {
                btn.GetComponent<CharSelectButtonController>().charInfo.text = charNames[whichCharacter] + " Selected";
                btn.gameObject.SetActive(false);
            }
            PlayerInfo.PI.mySelectedCharacter = whichCharacter;
            PlayerPrefs.SetInt("SelectedCharacter", whichCharacter);
            gm.incReadyPlayers();
        }
    }

    private void Update()
    {
        //if (gm.activeGame) panel.SetActive(false);
    }
}
