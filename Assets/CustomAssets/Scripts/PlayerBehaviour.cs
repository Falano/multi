using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// à mettre sur le sheep prefab

    
public class PlayerBehaviour: NetworkBehaviour {
    [SyncVar]
    public string localName;
    [SyncVar]
    public bool IsReady;
    public bool IsLocalPlayer = false;

    override public void OnStartLocalPlayer() //le override pourrait éventuellement faire de la merde?
    {
        if (isServer)
        {
            localName = "HostPlayer";
        }
        IsLocalPlayer = true;
        localName = "Player";
        if (PlayerPrefs.HasKey("playerName"))
        {
            localName = PlayerPrefs.GetString("playerName");
        }
    }

    /*public Score playerScore;
    public GameObject localScore;

    private void Start()
    {
        if (!isLocalPlayer) { return; }
        ColorManager.singleton.SetScoreHolder(gameObject, PlayerPrefs.GetString("playerName"));


        localScore = Instantiate(ColorManager.singleton.ScorePrefab);
        NetworkServer.Spawn(localScore);
        localScore.name = "score-" + name;
        Score playerScore = localScore.GetComponent<Score>();
        playerScore.PlayerName = name;
        playerScore.PlayerObj = gameObject;
        localScore.transform.SetParent(ColorManager.singleton.Scores.transform);
        gameObject.GetComponent<ScoreKeeper>().playerScore = playerScore;



    }



    /*
        public override void OnStartLocalPlayer()
        {
            if (!isLocalPlayer) { return; }
            string playerName = PlayerPrefs.GetString(name);
            currentPlayer = new Score(gameObject, playerName); // need somewhere to ask it of the player
            CmdUpdatePlayerList(gameObject);
        }

        [Command]
        public void CmdUpdatePlayerList(GameObject obj)
        {
            Score player = obj.GetComponent<ScoreKeeper>().currentPlayer;
            bool done = false;
            for (int i = 0; done == false; i++)
            {
                if (ColorManager.playersList[i] == null)
                {
                    player.SetI(i);
                    ColorManager.playersList[i] = player;
                    done = true;
                }
            }

            ColorManager.singleton.RpcUpdatePlayersList(obj);

        }*/



    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space) && !ColorManager.isGamePlaying)
        {
           IsReady = !IsReady;
        }
    }
}