using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

// cacher le network HUD si on est dans le menu de départ
public class checkIfNetworkHUD : MonoBehaviour {
    public static checkIfNetworkHUD singleton; // it might mess things up (when game>backToMenu>newGame). Be careful.
    private void OnEnable()
    {
        SceneManager.sceneLoaded += CheckIfMenuScene;
        SceneManager.sceneLoaded += checkSingleton;
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
        SceneManager.sceneLoaded -= CheckIfMenuScene;
        SceneManager.sceneLoaded -= checkSingleton;
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

}
