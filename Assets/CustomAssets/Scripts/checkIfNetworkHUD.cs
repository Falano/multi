using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

// cacher le network HUD si on est dans le menu de départ
public class checkIfNetworkHUD : MonoBehaviour {
    public static checkIfNetworkHUD singleton; // it might mess things up (when game>backToMenu>newGame). Be careful.
    public static NetworkManagerHUD netwHUD;
    public static NetworkDiscovery netwDisc;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += CheckIfMenuScene;
        SceneManager.sceneLoaded += checkSingleton;
    }

    private void Awake()
    {
        netwHUD = GetComponent<NetworkManagerHUD>();
        netwDisc = GetComponent<NetworkDiscovery>();
    }

    public void CheckIfMenuScene(Scene scene,LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "menu")
        {
            netwHUD.enabled = false;
            netwDisc.enabled = false;
        }
        else {
            netwDisc.enabled = false;
            netwHUD.enabled = true;
        }
    }

    
    private void checkSingleton(Scene scene, LoadSceneMode mode)
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleNetworkGUI();
            }
    }

    public static void ToggleNetworkGUI()
    {
        netwDisc.enabled = !netwDisc.enabled;
        netwHUD.enabled = !netwHUD.enabled;
    }

    public static void ToggleNetworkGUI(bool state)
    {
        netwDisc.enabled = state;
        netwHUD.enabled = state;
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= CheckIfMenuScene;
        SceneManager.sceneLoaded -= checkSingleton;
    }
}
