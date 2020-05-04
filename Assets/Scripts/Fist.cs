using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : MonoBehaviour
{
    public int ID;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        ID = player.GetComponent<CharacterMovement>().ID;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
