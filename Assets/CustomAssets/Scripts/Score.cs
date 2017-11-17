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
    public string playerName;
    string timeOfDeath = "0";
    public int colorChangesToOthers;
    public int colorChangesFromOthers;
    public int colorChangesFromMice;
    public int colorChangesFromSelf;
    public Text ScoreTx;
    float startTime; // l'assigner on start game

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

    public void SetPlayersName(string name)
    {
        playerName = name;
    }

}
