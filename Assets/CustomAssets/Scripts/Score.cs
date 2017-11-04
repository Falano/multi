using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// juste la classe score avec ses fonctions

public class Score {
    int i = 0;
    GameObject playerObj;
    string playerName;
    float timeOfDeath;
    public int colorChangesToOthers;
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
