using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class startButtonController : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TMP_Text btnText;
    private Color original;
    // Start is called before the first frame update
    void Awake()
    {
        original = btnText.color;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnSelect(BaseEventData eventData)
    {
        btnText.color = new Color(0, 0, 0);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        btnText.color = original;
    }
}
