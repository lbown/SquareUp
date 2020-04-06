using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleCorrector : MonoBehaviour
{
    public Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        toggle.isOn = Screen.fullScreen;
    }

    // Update is called once per frame
    void Update()
    {
        toggle.isOn = Screen.fullScreen;
    }

    public void toggled() {
        if (toggle.isOn != Screen.fullScreen)
        {
            VideoSettings.vs.fullScreenToggle();
        }
    }
}
