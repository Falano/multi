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
            if (PlayerPrefs.HasKey("playerName"))
            {
                return PlayerPrefs.GetString("playerName");
            }
            return playerName;
        }
        set
        {
            PlayerPrefs.SetString("playerName", value);
            playerName = value;
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
        timeOfDeath = 0;
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
