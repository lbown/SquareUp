using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private CubeController cubeControl;

    private void Awake()
    {
        cubeControl = GameObject.Find("Cube").GetComponent<CubeController>();
    }

}
