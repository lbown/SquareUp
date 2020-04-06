using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoSettings : MonoBehaviour
{
    public static VideoSettings vs;
    // Start is called before the first frame update
    private void Start()
    {
        
        if (VideoSettings.vs == null)
        {
            VideoSettings.vs = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void fullScreenToggle() {
        Screen.fullScreen = !Screen.fullScreen;
    }

    public void exitGame()
    {
        Application.Quit();
    }

    public void resChange(int i) {
        if (i == 0) {
            Screen.SetResolution(720, 480, Screen.fullScreen);
        }
        if (i == 1)
        {
            Screen.SetResolution(1080, 720, Screen.fullScreen);
        }
        if (i == 2) {
            Screen.SetResolution(1920, 1080, Screen.fullScreen);
        }
    }
}
