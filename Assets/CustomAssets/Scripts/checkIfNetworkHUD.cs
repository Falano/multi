using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

// cacher le network HUD si on est dans le menu de départ
public class checkIfNetworkHUD : MonoBehaviour {
    public static checkIfNetworkHUD singleton; // it might mess things up (when game>backToMenu>newGame). Be careful.
    private NetworkManagerHUD netwHUD;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += CheckIfMenuScene;
        SceneManager.sceneLoaded += checkSingleton;
    }

    private void Awake()
    {
        netwHUD = GetComponent<NetworkManagerHUD>();
    }

    public void CheckIfMenuScene(Scene scene,LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "menu")
        {
            netwHUD.enabled = false;
            ColorManager.isGamePlaying = false;
        }
        else {
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
            netwHUD.enabled = !netwHUD.enabled;
        }
    }


    private void OnDisable()
    {
        SceneManager.sceneLoaded -= CheckIfMenuScene;
        SceneManager.sceneLoaded -= checkSingleton;
    }
}
