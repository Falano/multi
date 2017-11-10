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
    public static Score[] listPlayers;
    private GameObject listOfPlayersParent;

    [Header("scripts drag-and-drop variables")]
    public Canvas lobbyCanvas;
    public Text launchGameTx;
    public GameObject playerStatePrefab;
    public GameObject ratKingPrefab;
    public GameObject ScoresHolderPrefab;
    [Header("Customisable gameplay-ish options")]
    private float refreshFrequency = 2.5f;

    [Header("Supposed to be empty: ")]
    public GameObject ratKing;
    public GameObject Scores;


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
    }

    void Start()
    {
        listPlayers = new Score[MenuManager.maxPlayersNumber];
        InvokeRepeating("RefreshListOfPlayers", 0, refreshFrequency);
        launchGameTx.text = "";
        Scores = new GameObject("Scores");
        Scores.AddComponent<NetworkIdentity>();
        if (isServer)
        {
            NetworkServer.Spawn(Scores);
        }
        GameObject[] thingsHolders = GameObject.FindGameObjectsWithTag("ThingsHolder"); //histoire de ne pas avoir à drag-and-dropper quinze mille objects dans mon ColorManager
        foreach (GameObject holder in thingsHolders)
        {
            switch (holder.name)
            {
                case "ratKing":
                case "ratKing(Clone)":
                    ratKing = holder;
                    break;
                case "listOfPlayers":
                case "listOfPlayers(Clone)":
                    listOfPlayersParent = holder;
                    break;
                case "Scores":
                case "Scores(Clone)":
                    Scores = holder;
                    break;
            }


        }
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




    [Command]
    void CmdKill(GameObject obj) { RpcKill(obj); }
    [ClientRpc]
    void RpcKill(GameObject obj) { obj.GetComponent<PlayerHealth>().KillSolo(); }
    public void Kill(GameObject obj) { CmdKill(obj); }


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
    public void RpcTogglePlayerReady(GameObject player, bool state)
    {
        player.GetComponent<Score>().ToggleReadySolo(state);
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
                listPlayers[i] = listPlayersGO[i].GetComponent<Score>();
                listPlayers[i].SetI(i);
                if (listPlayers[i].PlayerName == null)
                {
                    listPlayers[i].SetPlayersName("Player" + i.ToString());
                }
                string readyState = "not ready...";
                Color txColor = Color.white;
                if (listPlayers[i].IsReady == true)
                {
                    readyState = "ready!";
                    numberOfPlayersReady++;
                    txColor = Color.green;
                }
                //print(listPlayers[i].PlayerName + " : " + readyState);
                if(listPlayers[i].ScoreTx == null)
                {
                    float posX = listOfPlayersParent.transform.position.x;
                    float posY = listOfPlayersParent.transform.position.y;
                    listPlayers[i].ScoreTx = Instantiate(playerStatePrefab, listOfPlayersParent.transform);
                    listPlayers[i].ScoreTx.transform.position = new Vector2(posX, posY - 20 + i * -20);
                }
                listPlayers[i].ScoreTx.GetComponent<Text>().text = listPlayers[i].PlayerName + " : " + readyState;
                listPlayers[i].ScoreTx.GetComponent<Text>().color = txColor;
            }            
        }
        if (numberOfPlayersReady == listPlayersGO.Length)
        {
            StartCoroutine("launchingGame");
        }
    }
}