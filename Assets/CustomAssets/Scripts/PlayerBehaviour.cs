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
           
            if (PlayerPrefs.HasKey("playerName"))
            {
                localName = PlayerPrefs.GetString("playerName");
                CmdSetLocalName(localName, gameObject);
            }
            //gameObject.AddComponent<AudioListener>(); // avoir un AudioListener et l'activer/desactiver ici ne marche pas :/ 
        }
        StartCoroutine("waitToAssignScore");
    }

    [Command]
    private void CmdRefreshListOfPlayers()
    {
        ColorManager.singleton.RpcRefreshListOfPlayers();
    }

    IEnumerator waitToAssignScore()
    {
        yield return new WaitForSeconds(.1f);
        ScoreObj = ColorManager.singleton.SpawnScore(localName, gameObject);
        name = "sheep-" + localName;
        CmdRefreshListOfPlayers();

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
        ColorManager.singleton.RpcRefreshListOfPlayers();
    }
    public void ToggleReadySolo(bool state)
    {
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
