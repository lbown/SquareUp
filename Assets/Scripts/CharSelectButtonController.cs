using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CharSelectButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TMP_Text btnText, charInfo;
    private Color original;
    // Start is called before the first frame update
    void Awake()
    {
        original = btnText.color;
        charInfo.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnSelect(BaseEventData eventData)
    {
        btnText.color = new Color(0, 0, 0);
        charInfo.enabled = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        btnText.color = original;
        charInfo.enabled = false;
    }

    
}
