using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// juste la classe score avec ses fonctions
// fucks with ColorManager's RpcUpdatePlayersList (line 32) if I don't make it a SthBehaviour // unity says there is no constructor :/ // they say UNetWeaver error

public class Score : NetworkBehaviour
{
    [Tooltip("its index in the list")]
    public int i;
    public GameObject playerObj;
    public string playerName;
    float timeOfDeath = 0;
    public int colorChangesToOthers;
    public int colorChangesFromOthers;
    public int colorChangesFromMice;
    public int colorChangesFromSelf;
    float startTime; // l'assigner on start game

    private void Start()
    {
        transform.SetParent(ColorManager.singleton.ScoresHolderParent.transform);
    }


    public float TimeOfDeath
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
        timeOfDeath = Time.time - startTime;
    }

    public void SetI(int newI)
    {
        i = newI;
    }




    public void SetPlayersName(string name)
    {
        playerName = name;
    }

}
