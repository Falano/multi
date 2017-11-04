using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// juste la classe score avec ses fonctions
// fucks with ColorManager's RpcUpdatePlayersList (line 32) // unity says there is no constructor :/ // they say UNetWeaver error
public class Score {
    public int i;
    GameObject playerObj;
    string playerName;
    float timeOfDeath;
    public int colorChangesToOthers = 0;
    public int colorChangesFromOthers;
    public int colorChangesFromMice;
    public int colorChangesFromSelf;
    float startTime;

    public string PlayerName
    {
        get
        {
            return playerName;
        }
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

    public Score(GameObject playerObject, string playerNamed) 
    {
        playerObj = playerObject;
        startTime = Time.time;
        playerName = playerNamed;
    }

    public void SetTimeOfDeath()
    {
        timeOfDeath = Time.time - startTime;
    }
    public void SetI(int newI)
    {
        i = newI;
    }
}
