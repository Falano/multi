using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

// juste la classe score avec ses fonctions
// fucks with ColorManager's RpcUpdatePlayersList (line 32) if I don't make it a SthBehaviour // unity says there is no constructor :/ // they say UNetWeaver error

public class Score : NetworkBehaviour
{
    [Tooltip("its index in the list")]
    public int idNumber;
    public GameObject playerObj;
    public PlayerBehaviour behaviour;
    public PlayerHealth health;
    public PlayerChangeCol changeCol;
    [SyncVar]
    public string playerName;
    string timeOfDeath = "0";
    public int colorChangesToOthers;
    public int colorChangesFromOthers;
    public int colorChangesFromMice;
    public int colorChangesFromGround;
    public int colorChangesFromSelf = -1;
    public int colorChangesGiftedToTeam;
    public int colorChangesGiftedByTeam;
    public Text ScoreTx;
    float startTime; // l'assigner on start game
    public int team = -1;

    private void Start()
    {
        transform.SetParent(ColorManager.singleton.ScoresHolderParent.transform);
    }


    public string TimeOfDeath
    {
        get
        {
            return timeOfDeath;
        }
    }
    public GameObject PlayerObj
    {
        get
        {
            return playerObj;
        }
    }

    public void SetTimeOfDeath()
    {
        timeOfDeath = (Time.time - startTime).ToString("0.0");
    }

    public void SetI(int newI)
    {
        idNumber = newI;
    }

    public void SetStartTime()
    {
        startTime = Time.time;
    }

    public void SetPlayerObj(GameObject player)
    {
        playerObj = player;
        behaviour = player.GetComponent<PlayerBehaviour>();
        changeCol = player.GetComponent<PlayerChangeCol>();
        health = player.GetComponent<PlayerHealth>();
        playerName = behaviour.localName ;

    }

}
