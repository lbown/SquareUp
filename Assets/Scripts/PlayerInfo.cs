using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo PI;
    public int mySelectedCharacter, myRandomColor;
    public GameObject[] allCharacters;
    public List<Material> totalMaterials;
    public Dictionary<int, Material> materialList;

    // Start is called before the first frame update
    private void OnEnable()
    {
        if(PlayerInfo.PI == null)
        {
            PlayerInfo.PI = this;
        }
        else
        {
            if(PlayerInfo.PI != this)
            {
                Destroy(PlayerInfo.PI);
                PlayerInfo.PI = this;
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("SelectedCharacter"))
        {
            mySelectedCharacter = PlayerPrefs.GetInt("SelectedCharacter");
        }
        else
        {
            mySelectedCharacter = 0;
            PlayerPrefs.SetInt("SelectedCharacter", mySelectedCharacter);
        }
        myRandomColor = Random.Range(0, gm.totalMaterials.Count);
    }
}
