﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

// cacher le network HUD si on est dans le menu de départ
public class checkIfNetworkHUD : MonoBehaviour {

    private void OnEnable()
    {
        SceneManager.sceneLoaded += CheckIfMenuScene;
    }


    public void CheckIfMenuScene(Scene scene,LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "menu")
        {
            gameObject.GetComponent<NetworkManagerHUD>().enabled = false;
        }
        else {
            gameObject.GetComponent<NetworkManagerHUD>().enabled = true;
        }

    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded += CheckIfMenuScene;
    }

}