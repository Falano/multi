using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutoManager : ColorManager {

    public static TutoManager singleton;
    public bool isInTuto;
    public static string tutoName = "7";
    public Text tutoNarr;
    public float speechDuration = 3;
    public static Text tutoText;


    new void Awake()
    {
        base.Awake();

        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }

        if (SceneManager.GetActiveScene().name == (tutoName))// cuz I'm using it in EnemySpawner's Awake
        {
            isInTuto = true;
        }
    }

    // Use this for initialization
    new void Start () {
        base.Start();

        MenuManager.soloGame = true;
        foreach (GameObject gui in GameObject.FindGameObjectsWithTag("GUI"))
        {
            switch (gui.name)
            {
                case "tutoNarr":
                    tutoNarr = gui.GetComponent<Text>();
                    break;
            }
        }
        tutoSpeech(15, "press MenuManager.InteractKey.ToString() when you're ready. The game starts //n when every logged in player is ready (even if everyone //nyou planned to play with hasn't logged in yet). //n//n This tutorial is offline though, so don't worry about it for now, //njust remember: MenuManager.InteractKey.ToString() when you're ready.", tutoNarr);
    }
	
    public void ChangeCol(GameObject obj, Color col, GameObject attacker)
    {
        base.RpcChangeCol(obj, col, attacker);


        if (obj.GetComponent<PlayerBehaviour>().isLocalPlayer)
        {
            tutoSpeech(speechDuration, "I can run, and I can hide!", obj.GetComponentInChildren<Text>());
        }

        if (attacker == obj)
        {
                tutoSpeech(speechDuration, "Ow, I better not do that too often.", attacker.GetComponentInChildren<Text>());
                tutoSpeech(speechDuration * 3, "See the ball in the top-right corner? That's how many//ncolour changes you have left before you turn back to paint.", tutoNarr);
        }
        else if (attacker.CompareTag("Player"))
        {
                tutoSpeech(speechDuration, "so that's what I look like to others...", attacker.
                    
                    
                    
                    
                    
                    
                    
                    
                    form.parent.GetComponentInChildren<Text>());
        }
    }


    private void tutoSpeechLaunch()
    {
        tutoSpeech(speechDuration, "I should try pressing MenuManager.MenuKey.ToString() several times...", tutoText);
    }

    [ClientRpc]
    new public void RpcLaunchGameTx()
    {
        base.RpcLaunchGameTx();
        Invoke("tutoSpeechLaunch", 2);
    }
}
