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
    private int currentSpawnedPowerUps;
    public bool currentSpawnedRotatePowerUp;
    public TextMeshProUGUI timer;
    private float StartTime;
    public int TimeLimitMinutes;
    public GameObject gameOverPanel;
    public int Winner;
    public int WinnerScore;
    private List<GameObject> DisconectedPlayers;
    public List<string> pwrUps;
    private int maxPowerUps;
    [SerializeField] private float totalTimeUntilPowerUp, totalTimeUntilRotatePowerUp, powerUpTimer, rotatePowerUpTimer;

    private bool gameActive;
    // Start is called before the first frame update
    void Start()
    {
        currentSpawnedPowerUps = 0;
        maxPowerUps = 2;
        PV = GetComponent<PhotonView>();
        players = new List<GameObject>();
        totalTimeUntilRotatePowerUp = 30;
        totalTimeUntilPowerUp = 12;
        powerUpTimer = totalTimeUntilPowerUp;
        rotatePowerUpTimer = totalTimeUntilRotatePowerUp;
        StartTime = Time.time;
        Winner = 0;
        WinnerScore = 0;
        DisconectedPlayers = new List<GameObject>();
        gameActive = false;
        currentSpawnedRotatePowerUp = false;
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
        rotatePowerUpTimer = totalTimeUntilRotatePowerUp;
        PV.RPC("RPC_SynchronizeRotatePowerUps", RpcTarget.AllBuffered, true);
    }
    public void ResetPowerUpTimer()
    {
        powerUpTimer = totalTimeUntilPowerUp;
        PV.RPC("RPC_SynchronizePowerUps", RpcTarget.AllBuffered, currentSpawnedPowerUps);
    }
    public void DecrementPowerUps(bool isCube) {
        PV.RPC("RPC_SynchronizePowerUps", RpcTarget.AllBuffered, (currentSpawnedPowerUps - 1));
        if (isCube)
        {
            PV.RPC("RPC_SynchronizeRotatePowerUps", RpcTarget.AllBuffered, false);
        }
    }

    void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            powerUpTimer -= Time.deltaTime;
            rotatePowerUpTimer -= Time.deltaTime;
            if (powerUpTimer <= 0)
            {
                if (currentSpawnedPowerUps < maxPowerUps)
                {
                    PV.RPC("RPC_SynchronizePowerUps", RpcTarget.AllBuffered, (currentSpawnedPowerUps + 1));
                    ResetPowerUpTimer();
                    int spawnPicker = Random.Range(0, GameSetup.gs.powerUpLocations.Length);
                    System.Random rnd = new System.Random();
                    PV.RPC("RPC_InstantiatePowerUp", RpcTarget.AllBuffered, spawnPicker, rnd.Next(pwrUps.Count));
                }
                else {
                    ResetPowerUpTimer();
                }
            }
            if (rotatePowerUpTimer <= 0)
            {
                if (currentSpawnedPowerUps < maxPowerUps && !currentSpawnedRotatePowerUp)
                {
                    PV.RPC("RPC_SynchronizeRotatePowerUps", RpcTarget.AllBuffered, true);
                    PV.RPC("RPC_SynchronizePowerUps", RpcTarget.AllBuffered, (currentSpawnedPowerUps + 1));
                    ResetPowerUpTimer();
                    ResetRotatePowerUpTimer();
                    int spawnPicker = Random.Range(0, GameSetup.gs.powerUpLocations.Length);
                    PV.RPC("RPC_InstantiateRotatePowerUp", RpcTarget.AllBuffered, spawnPicker);
                }
                else {
                    ResetRotatePowerUpTimer();
                }
            }

            if (!timePaused)
            {
                float t = StartTime - Time.time;
                int minutes = ((int)t / 60);
                int seconds = (int)(t % 60);

                string time = (TimeLimitMinutes + minutes).ToString() + ":" + (60 + seconds).ToString();
                PV.RPC("RPC_SyncTimer", RpcTarget.AllBuffered, StartTime, time);
                
            }
        }
    }

    public void findWinner()
    {
        PV.RPC("RPC_PurgePlayerList", RpcTarget.AllBuffered);
        WinnerScore = -1000;
        foreach (GameObject p in players)
        {
            if (p.GetComponent<CharacterMovement>().numKills - p.GetComponent<CharacterMovement>().numDeaths > WinnerScore)
            {
                WinnerScore = p.GetComponent<CharacterMovement>().numKills - p.GetComponent<CharacterMovement>().numDeaths;
                Winner = p.GetComponent<CharacterMovement>().ID;
            }
        }
        PV.RPC("RPC_tellWinner", RpcTarget.AllBuffered, Winner);
        PV.RPC("RPC_GMWinner", RpcTarget.AllBuffered, Winner, WinnerScore);
    }


    public void addPlayer(GameObject p)
    {
        PV.RPC("RPC_AddPlayer", RpcTarget.AllBuffered, p.GetComponent<PhotonView>().ViewID);
        if(p == null)
        {
            Debug.Log("Attempted to add null player");
        }
    }

    public void purgePlayerList()
    {
        PV.RPC("RPC_PurgePlayerList", RpcTarget.AllBuffered);
    }

    public void pauseTime() {
        PV.RPC("RPC_PauseTime", RpcTarget.AllBuffered);
    }
    public void unpauseTime()
    {
        PV.RPC("RPC_UnPauseTime", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_InstantiateRotatePowerUp(int spawnPos)
    {
        Instantiate(Resources.Load<GameObject>("PhotonPrefabs/RotateCubePowerUp"), GameSetup.gs.powerUpLocations[spawnPos].position, GameSetup.gs.powerUpLocations[spawnPos].rotation);
    }
    [PunRPC]
    private void RPC_InstantiatePowerUp(int spawnPos, int rndPowerUp)
    {
        Instantiate(Resources.Load<GameObject>("PhotonPrefabs/"+ pwrUps[rndPowerUp]), GameSetup.gs.powerUpLocations[spawnPos].position, GameSetup.gs.powerUpLocations[spawnPos].rotation);
    }

    [PunRPC] 
    private void RPC_PauseTime()
    {
        PV.RPC("RPC_PurgePlayerList", RpcTarget.AllBuffered);
        timePaused = true;
        foreach (GameObject player in players)
        {
            player.GetComponent<CharacterMovement>().pauseTime();
        }
    }
    [PunRPC]
    private void RPC_UnPauseTime()
    {
        timePaused = false;
        foreach (GameObject player in players)
        {
            player.GetComponent<CharacterMovement>().unpauseTime();
        }
    }

    [PunRPC]
    private void RPC_SynchronizeRotatePowerUps(bool isActive)
    {
        currentSpawnedRotatePowerUp = isActive;
    }
    [PunRPC]
    private void RPC_SynchronizePowerUps(int numUps)
    {
        currentSpawnedPowerUps = numUps;
    }
    [PunRPC]
    private void RPC_SyncTimer(float stime,string time)
    {
        StartTime = stime;
        timer.text = time;
        if (System.Convert.ToInt32(time.Split(':')[0]) == 0 && System.Convert.ToInt32(time.Split(':')[1]) == 69)
        {
            PV.RPC("RPC_EndGame", RpcTarget.AllBuffered,Winner);
        }
    }
    [PunRPC]
    private void RPC_EndGame(int id)
    {
        pauseTime();
        PhotonView.Find(id + 1).gameObject.transform.position = new Vector3(0, 0, -50);
        gameOverPanel.SetActive(true);
    }
    [PunRPC]
    private void RPC_tellWinner(int id)
    {
        foreach (GameObject p in players)
        {
            if (p != null) p.GetComponent<PhotonView>().RPC("RPC_IsWinner", RpcTarget.AllBuffered, id);
        }
        
    }

    [PunRPC]
    private void RPC_GMWinner(int id, int score)
    {
        Winner = id;
        WinnerScore = score;
    }
    [PunRPC]
    private void RPC_AddPlayer(int id)
    {
        players.Add(PhotonView.Find(id).gameObject);
    }
    [PunRPC]
    private void RPC_PurgePlayerList()
    {
        foreach (GameObject player in players)
        {
            if(player == null)
            {
                Debug.Log("null in players");
                DisconectedPlayers.Add(player);
            }
        }
        if(DisconectedPlayers.Count != 0)
        {
            foreach(GameObject p in DisconectedPlayers)
            {
                players.Remove(p);
            }
        }
    }
}
