using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pause : MonoBehaviour
{
    private Canvas c;
    public GameObject init;
    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<Canvas>();
        EventSystem.current.SetSelectedGameObject(init);
        Debug.Log(EventSystem.current.name);
        //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }

    void OnStart() {
        c.enabled = !c.enabled;
    }
}
