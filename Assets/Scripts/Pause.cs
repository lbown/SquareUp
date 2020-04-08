using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private Canvas c;
    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnStart() {
        c.enabled = !c.enabled;
    }
}
