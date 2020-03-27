using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<GameObject> players;
    // Start is called before the first frame update
    void Start()
    {
        players = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addPlayer(GameObject p) {
        players.Add(p);
    }

    public void pauseTime() {
        foreach (GameObject player in players) {
            player.GetComponent<CharacterMovement>().pauseTime();
        }
    }
    public void unpauseTime()
    {
        foreach (GameObject player in players)
        {
            player.GetComponent<CharacterMovement>().unpauseTime();
        }
    }
}
