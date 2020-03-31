using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public static GameSetup gs;
    public Material p1Mat, p2Mat, p3Mat, p4Mat;
    public Transform[] spawnPoints;

    private void OnEnable()
    {
        if(GameSetup.gs == null)
        {
            GameSetup.gs = this;
        } 
    }
}
