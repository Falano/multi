using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// à mettre sur le color manager
// dit à tout le monde que le mouton a changé de couleur (rpc) (probs car n'est pas localPlayerAuthority?)
// dit à tout le monde que le player a changé d'état ready (rpc) (probs car n'est pas localPlayerAuthority?)
// contient la fonction finale Kill qui désactive le renderer du mouton et active death anim; the object destroy itself is on a script on the child
// LaunchGame and RefreshListOfPlayers are essentially a lobby

[RequireComponent(typeof(NetworkIdentity))] //everything unchecked
public class ColorManager : NetworkBehaviour
{
    public Vector3 LvlSize;
    public static ColorManager singleton;
    public bool isLocalPlayerDead = false;
    public static bool isGamePlaying = false;
    public static PlayerBehaviour[] listPlayers;
    public GameObject ScorePrefab;
    [Header("supposed to be empty")]
    public Canvas lobbyCanvas;
    public Text launchGameTx;
    public GameObject listOfPlayersParent;
    public GameObject playerStatePrefab;
    public GameObject ScoresHolderParent;
    public GameObject ratKing;
    public Image healthGUI;
    public int maxPlayersNumber;

    private float refreshFrequency = 2.5f;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }
        ScoresHolderParent = new GameObject("ScoresHolder") { tag = "ThingsHolder" }; ///////////////////////cause I'm using it in the Score's start

    }

    void Start()
    {
        ratKing = new GameObject("ratKing") { tag = "ThingsHolder" };
        maxPlayersNumber = 20; //ok c'est laid mais c'est juste pour tester si ça marche /////////////////////////////////////

        GameObject[] GUIs = GameObject.FindGameObjectsWithTag("GUI");
        foreach (GameObject gui in GUIs)
        {
            switch (name)
            {
                case "healthGUI":
                case "healthGUI(Clone)":
                    healthGUI = gui.GetComponent<Image>();
                    break;
                case "LobbyCanvas":
                case "LobbyCanvas(Clone)":
                    lobbyCanvas = gui.GetComponent<Canvas>();
                    break;
                case "listOfPlayers":
                case "listOfPlayers(Clone)":
                    listOfPlayersParent = gui;
                    break;
                case "LaunchGameTx":
                case "LaunchGameTx(Clone)":
                    launchGameTx = gui.GetComponent<Text>();
                    break;
            }
        }


        listPlayers = new PlayerBehaviour[maxPlayersNumber];
        InvokeRepeating("RefreshListOfPlayers", 0, refreshFrequency);
        launchGameTx.text = "";
    }

    
    public GameObject SpawnScore(string name, GameObject obj)
    {
        GameObject score = Instantiate(ScorePrefab);
        score.name = "score-" + name;
        Score currScore = score.GetComponent<Score>();
        currScore.playerObj = obj;
        currScore.playerName = name;
        NetworkServer.Spawn(score);

        return score;

    }


    [ClientRpc]
    public void RpcChangeCol(GameObject obj, Color col/*, GameObject attacker*/)
    {
        obj.GetComponent<PlayerHealth>().TakeDamage();
        if (obj.GetComponent<PlayerHealth>().Hp > 0)
        { // pour que la flaque de peinture soit de la dernière couleur vue et pas d'une nouvelle couleur random (cf Kill() ci-dessous)
            Renderer rd = obj.GetComponentInChildren<Renderer>();
            foreach (Material mat in rd.materials)
            {
                mat.color = col;
            }

            // if I keep this, after the second player attack, sheep bleed to death FOR SOME REASON
            /*
            if (attacker == obj)
            {
                obj.GetComponent<ScoreKeeper>().currentPlayer.colorChangesFromSelf += 1;
            }
            else if (attacker.CompareTag("AttackChangeCol"))
            {
                obj.GetComponent<ScoreKeeper>().currentPlayer.colorChangesFromMice += 1;
            }
            else if (attacker.CompareTag("Player"))
            {
                obj.GetComponent<ScoreKeeper>().currentPlayer.colorChangesFromOthers += 1;
                attacker.GetComponent<ScoreKeeper>().currentPlayer.colorChangesToOthers += 1;
            }
            */
        }
    }


    [ClientRpc]
    public void RpcKill(GameObject obj) { obj.GetComponent<PlayerHealth>().KillSolo(); }

    public IEnumerator launchingGame() // un message d'erreur dit qu'il ne sait pas se lancer sur le host? mais ça marche quand même, so whatever
    {
        launchGameTx.text = "Launching Game...";
        yield return new WaitForSeconds(4);
        LaunchGame();
    }

    public void LaunchGameSolo()
    {
        CancelInvoke("RefreshListOfPlayers");
        isGamePlaying = true;
        launchGameTx.text = "";
        listOfPlayersParent.SetActive(false);
        if (isServer)
        {
            foreach (EnemyMover enemy in EnemySpawner.enemyList)
            {
                IEnumerator wait = enemy.waitForChangeDir(Random.Range(enemy.waitRange.x, enemy.waitRange.y));
                StartCoroutine(wait); //it works ONLY IF I create the coroutine on the previous line and set it up in here instead of in EnemyMover
            }
        }
        lobbyCanvas.enabled = false;
    }
    [Command]
    void CmdLaunchGame()
    {
        RpcLaunchGame(); // 2) de dire aux clients
    }
    [ClientRpc]
    void RpcLaunchGame()
    {
        LaunchGameSolo(); // 3) de lancer le jeu
    }
    void LaunchGame()
    {
        CmdLaunchGame(); // 1) on dit au server
    }




    [ClientRpc]
    public void RpcSetLocalName(string name, GameObject obj)
    {
        obj.GetComponent<PlayerBehaviour>().SetLocalNameSolo(name);
    }



    [ClientRpc]
    public void RpcTogglePlayerReady(GameObject player, bool state)
    {
        player.GetComponent<PlayerBehaviour>().ToggleReadySolo(state);
    }



    public void RefreshListOfPlayers()
    {
        print("refreshing list of players");
        int numberOfPlayersReady = 0;
        GameObject[] listPlayersGO = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < listPlayers.Length; i++)
        {
            if (i >= listPlayersGO.Length)
            {
                listPlayers[i] = null;
            }
            else
            {
                listPlayers[i] = listPlayersGO[i].GetComponent<PlayerBehaviour>();
                listPlayers[i].idNumber = i;
                if (listPlayers[i].localName == null || listPlayers[i].localName == "")
                {
                    listPlayers[i].localName = "Player" + i.ToString();
                }
                string readyState = "not ready...";
                Color txColor = Color.white;
                if (listPlayers[i].IsReady == true)
                {
                    readyState = "ready!";
                    numberOfPlayersReady++;
                    txColor = Color.green;
                }
                print(listPlayers[i].localName + " : " + readyState);
                if (listPlayers[i].ScoreTx == null)
                {
                    float posX = listOfPlayersParent.transform.position.x;
                    float posY = listOfPlayersParent.transform.position.y;
                    listPlayers[i].ScoreTx = Instantiate(playerStatePrefab, listOfPlayersParent.transform);
                    listPlayers[i].ScoreTx.transform.position = new Vector2(posX, posY - 20 + i * -20);
                }
                listPlayers[i].ScoreTx.GetComponent<Text>().text = listPlayers[i].localName + " : " + readyState;
                listPlayers[i].ScoreTx.GetComponent<Text>().color = txColor;
            }
        }
        if (numberOfPlayersReady == listPlayersGO.Length)
        {
            StartCoroutine("launchingGame");
        }
    }
}