﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class Pause : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject init;
    public GameObject dcButton;
    public GameObject GameOverPanel;
    // Start is called before the first frame update
    void Start()
    {
        pausePanel.SetActive(false);
        GameOverPanel.SetActive(false);
        EventSystem.current.SetSelectedGameObject(init);
        //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    }

    void OnStart() {
        pausePanel.SetActive(!pausePanel.activeSelf);
        EventSystem.current.SetSelectedGameObject(dcButton);
    }
}
