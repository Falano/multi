using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// juste la classe score avec ses fonctions

public class Score {
    int i = 0;
    GameObject playerObj;
    string playerName;
    float timeOfDeath;
    int colorChangesToOthers;
    int colorChangesFromOthers;
    int colorChangesFromMice;
    int colorChangesFromSelf;
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
    public void changedCol(ref int param, int nb = 1)
    {
        param+=nb;
    }
}
