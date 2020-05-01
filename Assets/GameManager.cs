using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour, IPunObservable
{
    public PhotonView PV;
    public List<GameObject> players;
    public bool timePaused;
    private bool currentRotatePowerUp;
    public TextMeshProUGUI timer;
    private float StartTime;
    public int TimeLimitMinutes;
    public GameObject gameOverPanel;
    public GameObject Winner;
    [SerializeField] private float totalTimeUntilRotatePowerup, powerUpTimer;
    // Start is called before the first frame update
    void Start()
    {
        PV = GetComponent<PhotonView>();
        players = new List<GameObject>();
        totalTimeUntilRotatePowerup = 60;
        powerUpTimer = totalTimeUntilRotatePowerup;
        StartTime = Time.time;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(timePaused);
        }
        else if (stream.IsReading)
        {
            timePaused = (bool)stream.ReceiveNext();
        }
    }

    public void ResetRotatePowerUpTimer()
    {
        powerUpTimer = totalTimeUntilRotatePowerup;
        PV.RPC("RPC_SynchronizePowerUps", RpcTarget.AllBuffered, false);
    }

    void Update()
    {
       // float t = Time.time;
      //  int minutes = ((int)t / 60);
       // int seconds = (int) (t % 60);

       // timer.text = (TimeLimitMinutes - minutes).ToString() + ":" + (60 - seconds).ToString();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            powerUpTimer -= Time.deltaTime;
            if (powerUpTimer <= 0 && currentRotatePowerUp == false)
            {
                PV.RPC("RPC_SynchronizePowerUps", RpcTarget.AllBuffered, true);
                int spawnPicker = Random.Range(0, GameSetup.gs.powerUpLocations.Length);
                PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "RotateCubePowerUp"), GameSetup.gs.powerUpLocations[spawnPicker].position, GameSetup.gs.powerUpLocations[spawnPicker].rotation, 0);
            }

            if (!timePaused)
            {
                float t = StartTime - Time.time;
                int minutes = ((int)t / 60);
                int seconds = (int)(t % 60);

                string time = (TimeLimitMinutes - minutes).ToString() + ":" + (60 + seconds).ToString();
                PV.RPC("RPC_SyncTimer", RpcTarget.AllBuffered, StartTime, time);
                
            }

            
            foreach (GameObject p in players)
            {
                if (Winner == null)
                {
                    Winner = p;
                }
                if(p.GetComponent<CharacterMovement>().numKills - p.GetComponent<CharacterMovement>().numDeaths > Winner.GetComponent<CharacterMovement>().numKills - Winner.GetComponent<CharacterMovement>().numDeaths)
                {
                    Winner = p;
                }
            }
        }

    }

    public void addPlayer(GameObject p) {
        players.Add(p);
    }

    public void removePlayer(GameObject p)
    {
        players.Remove(p);
    }
    public void pauseTime() {
        timePaused = true;
        foreach (GameObject player in players) {
            player.GetComponent<CharacterMovement>().pauseTime();
        }
    }
    public void unpauseTime()
    {
        timePaused = false;
        foreach (GameObject player in players)
        {
            player.GetComponent<CharacterMovement>().unpauseTime();
        }
    }
    public void countTime(int min, int sec)
    {

    }

    [PunRPC]
    private void RPC_SynchronizePowerUps(bool isActive)
    {
        currentRotatePowerUp = isActive;
        powerUpTimer = 60;
    }
    [PunRPC]
    private void RPC_SyncTimer(float stime,string time)
    {
        StartTime = stime;
        timer.text = time;
        if (System.Convert.ToInt32(time.Split(':')[0]) == 0 && System.Convert.ToInt32(time.Split(':')[1]) == 1)
        {
            PV.RPC("RPC_EndGame", RpcTarget.AllBuffered);
        }
    }
    [PunRPC]
    private void RPC_EndGame()
    {
        pauseTime();
        gameOverPanel.SetActive(true);
    }

}
