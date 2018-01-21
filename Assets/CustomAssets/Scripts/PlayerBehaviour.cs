using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class PlayerBehaviour : NetworkBehaviour
{
    [SyncVar] bool _isReady = false;
    public int idNumber;
    [SyncVar] public string localName;
    public GameObject ScoreTx;
    public Score ScoreObj;
    [SyncVar]
    public int team = -1;
    public bool localAlly = false;
    private TextMesh DebugTxFloating;

    public void DebugFloating(string sentence)
    {
        //DebugTxFloating.text = sentence;
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


    private void Awake()
    {
        if (isLocalPlayer) //needs to be before ColorManager's Start
        {
            ColorManager.singleton.localPlayer = gameObject;
            CameraMover.singleton.activePlayer = transform; // on dit à la camera que c'est lui ici le player à suivre
            //CmdSyncGameState();
        }
    }


    void Start()
    {
        DebugTxFloating = gameObject.GetComponentInChildren<TextMesh>();
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
        ScoreObj = ColorManager.singleton.SpawnScore(localName, gameObject).GetComponent<Score>();
        name = "sheep-" + localName;
        if (isLocalPlayer)
        {
            CmdRefreshListOfPlayers();
        }
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
        CmdTogglePlayerReady(gameObject, state);
    }
    [Command]
    public void CmdTogglePlayerReady(GameObject player, bool state)
    {
        ColorManager.singleton.RpcTogglePlayerReady(player, state);
        ColorManager.singleton.RpcRefreshListOfPlayers();
    }
    public void ToggleReadySolo(bool state)
    {
        _isReady = state;
    }
    public void ChangeTeam(int change)
    {
        CmdChangeTeam(gameObject, change);
    }
    [Command]
    public void CmdChangeTeam(GameObject player, int change)
    {
        ColorManager.singleton.RpcChangeTeam(player, change);
        ColorManager.singleton.RpcRefreshListOfPlayers();
    }

    public void ChangeTeamSolo(int change)
    {
        team += change;
        team = (team < -1) ? ColorManager.singleton.teamsNbLocal - 1 : team;
        team = (team > ColorManager.singleton.teamsNbLocal - 1) ? -1 : team;

    }

    [Command]
    public void CmdRefreshPlayerMidGame() { ColorManager.singleton.RpcRefreshPlayerMidGame(); }


    /*
    [Command]
    public void CmdSyncGameState()
    {
        ColorManager.singleton.currStateString = ColorManager.singleton.currState.ToString();
        ColorManager.singleton.RpcSyncGameState(ColorManager.singleton.currStateString);
    }
    */

    void Update()
    {

        if (!isLocalPlayer)
        {
            return;
        }

        if (ColorManager.singleton.CurrState == ColorManager.gameState.lobby)
        {
            if (Input.GetKeyDown(MenuManager.interact))
            {
                ToggleReady(!_isReady);
            }
            if (Input.GetKeyDown(MenuManager.right))
            {
                ChangeTeam(+1);
            }
            if (Input.GetKeyDown(MenuManager.left))
            {
                ChangeTeam(-1);
            }
        }
    }
}
