using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownController : MonoBehaviour
{
    public TMP_Dropdown ddown;
    private List<int> playerOptions;
    // Start is called before the first frame update
    void Start()
    {
        ddown = GetComponent<TMP_Dropdown>();
        playerOptions = new List<int> { 4, 6, 8, 100 };
        ddown.onValueChanged.AddListener(delegate { DropdownValueChanged(ddown); }); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void DropdownValueChanged(TMP_Dropdown change)
    {
        Debug.Log("DropdownChanged");
        MultiplayerSettings.multiplayerSettings.SetMaxPlayers(playerOptions[change.value]);
    }
}
