using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Crown : MonoBehaviour
{
    private PhotonView PV;
    public GameObject gm;
    public GameObject Winner;
    public GameObject topScore;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
            foreach (GameObject p in gm.GetComponent<GameManager>().players)
            {
                if (Winner == null)
                {
                    Winner = p;
                    setPlayer();
                }
                if (p.GetComponent<CharacterMovement>().numKills - p.GetComponent<CharacterMovement>().numDeaths > Winner.GetComponent<CharacterMovement>().numKills - Winner.GetComponent<CharacterMovement>().numDeaths)
                {
                    Winner = p;
                    setPlayer();
                }
            }
        
    }
    private void setPlayer()
    {
        gameObject.transform.position = Winner.transform.position + new Vector3(0, 2, 0);
        gameObject.transform.parent = Winner.transform;
    }
}
