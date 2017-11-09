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
    [SerializeField]
    GameObject playerObj;
    [SerializeField]
    string playerName;
    float timeOfDeath;
    bool alive; // XXX
    public int colorChangesToOthers;
    public int colorChangesFromOthers;
    public int colorChangesFromMice;
    public int colorChangesFromSelf;
    float startTime;
    [SerializeField]
    bool _isReady = false;
    public GameObject ScoreTx;

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
        set
        {
            playerObj = value;
        }
    }
    public bool IsReady
    {
        get
        {
            return _isReady;
        }
        set
        {
            ToggleReady(value);
        }
    }

    public bool Alive // XXX
    {
        get
        {
            return alive;
        }
        private set
        {
            alive = value;
        }
    }

    public Score(GameObject playerObject, string playerNamed)
    {
        playerObj = playerObject;
        startTime = Time.time;
        playerName = playerNamed;
        Alive = true; // XXX
        timeOfDeath = 0; // XXX
    }

    public void SetTimeOfDeath()
    {
        Alive = false;//XXX
        timeOfDeath = Time.time - startTime;
    }
    public void SetI(int newI)
    {
        i = newI;
    }



    public void ToggleReady(bool state)
    {
        _isReady = state;
        //print("is player ready? " + isReady);
        CmdTogglePlayerReady(gameObject, state);
    }
    [Command]
    public void CmdTogglePlayerReady(GameObject player, bool state)
    {
        _isReady = state;
        ColorManager.singleton.RpcTogglePlayerReady(gameObject, state);
    }
    public void ToggleReadySolo(bool state)
    {
        _isReady = state;
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
        if (Input.GetKeyDown(KeyCode.Space) && !ColorManager.isGamePlaying)
        {
            ToggleReady(!_isReady);
        }
    }
}
