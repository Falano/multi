using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// juste la classe score avec ses fonctions
// fucks with ColorManager's RpcUpdatePlayersList (line 32) if I don't make it a SthBehaviour // unity says there is no constructor :/ // they say UNetWeaver error

public class Score : NetworkBehaviour{
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
    bool isReady = false;

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
    public bool IsReady
    {
        get
        {
            return isReady;
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



    public void ToggleReady(bool state)
    {
        isReady = state;
        print("is player ready? " + isReady);
        CmdTogglePlayerReady(gameObject, state);
    }
    [Command]
    public void CmdTogglePlayerReady(GameObject player, bool state)
    {
        isReady = state;
        ColorManager.singleton.RpcTogglePlayerReady(gameObject, state);
    } 
    public void ToggleReadySolo(bool state)
    {
        isReady = state;
    }


    public void SetPlayersName(string name)
    {
        playerName = name;
    }

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && !ColorManager.isGamePlaying) {
            ToggleReady(!isReady);
        }
    }
}
