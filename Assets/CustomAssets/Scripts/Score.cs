using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// juste la classe score avec ses fonctions
// fucks with ColorManager's RpcUpdatePlayersList (line 32) if I don't make it a SthBehaviour // unity says there is no constructor :/ // they say UNetWeaver error

public class Score : NetworkBehaviour
{
    [Tooltip("its index in the list")]
    public int i = -1;
    [SerializeField]
    GameObject playerObj;
    [SerializeField]
    public string PlayerName = "default";
    float timeOfDeath;
    bool alive;
    public int colorChangesToOthers;
    public int colorChangesFromOthers;
    public int colorChangesFromMice;
    public int colorChangesFromSelf;
    float startTime;
    [SerializeField]
    public GameObject ScoreTx;
    private GameObject ScoreParent;

    private void Start()
    {
        ScoreParent = ColorManager.singleton.Scores;
        transform.SetParent(ScoreParent.transform);
        gameObject.tag = "Score";
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

    public void SetTimeOfDeath()
    {
        playerObj.GetComponent<PlayerHealth>().Alive = false;
        timeOfDeath = Time.time - startTime;
    }
    public void SetI(int newI)
    {
        i = newI;
    }


    /*
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
    }*/


}
