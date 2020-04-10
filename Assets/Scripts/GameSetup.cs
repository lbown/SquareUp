using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public static GameSetup gs;
    public Material[] playerColors;
    public Transform[] spawnPoints;
    public Transform[] powerUpLocations;

    private void OnEnable()
    {
        if(GameSetup.gs == null)
        {
            GameSetup.gs = this;
        } 
    }
}
