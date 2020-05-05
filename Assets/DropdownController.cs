using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropdownController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TMP_Dropdown ddown;
    private List<int> playerOptions;
    public TMP_Text label;
    private Color original;
    // Start is called before the first frame update
    void Start()
    {
        ddown = GetComponent<TMP_Dropdown>();
        original = label.color;
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

    public void OnSelect(BaseEventData eventData)
    {
        label.color = new Color(0, 0, 0);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        label.color = original; 
    }
}
