using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerBehaviour : NetworkBehaviour
{

    bool _isReady = false;
    public int idNumber;
    [SyncVar]
    public string localName;
    public GameObject ScoreTx;
    public GameObject ScoreObj;



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


    void Start()
    {
        if (isLocalPlayer)
        {
            ColorManager.singleton.localPlayer = gameObject;
            CameraMover.singleton.activePlayer = transform; // on dit à la camera que c'est lui ici le player à suivre
            if (ColorManager.isGamePlaying) // s'il arrive dans un jeu en cours 
            {
                //print("GAME IS PLAYING");
                ColorManager.singleton.LaunchGameSolo(); //il désactive la GUI du lobby
                GetComponent<PlayerHealth>().TakeDamage(999999); // that was so assholes who come mid-game died but could still follow it; don't think it works though
            }
            if (PlayerPrefs.HasKey("playerName"))
            {
                localName = PlayerPrefs.GetString("playerName");
                CmdSetLocalName(localName, gameObject);
            }
        }
        StartCoroutine("waitToAssignScore");
    }

    IEnumerator waitToAssignScore()
    {
        yield return new WaitForSeconds(.5f);
        ScoreObj = ColorManager.singleton.SpawnScore(localName, gameObject);
        name = "sheep-" + localName; // ça ne marche que chez soi, probs parce que le CmdSetLocalName n'a pas encore fini de run. Mais on s'en fout parce que pour le reste c'est bon.

    }

    [Command]
    public void CmdSetLocalName(string name, GameObject obj)
    {
        ColorManager.singleton.RpcSetLocalName(name, obj);
    }

    public void SetLocalNameSolo(string name)
    {
        localName = name;
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
        ColorManager.singleton.RpcTogglePlayerReady(gameObject, state);
    }
    public void ToggleReadySolo(bool state)
    {
        ColorManager.singleton.RefreshListOfPlayers();
        _isReady = state;
    }



    void Update()
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
