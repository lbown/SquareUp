using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDCallResChange : MonoBehaviour
{
    public TMPro.TMP_Dropdown dd;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void resChange() {
        VideoSettings.vs.resChange(dd.value);
    }
}
