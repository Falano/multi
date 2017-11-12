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
    public static Score[] listScores;
    private GameObject listOfPlayersParent;
    public string localName; //do I actually use this?

    [Header("scripts drag-and-drop variables")]

    public GameObject playerStatePrefab;
    public GameObject ratKingPrefab;
    //public GameObject ScoresHolderPrefab; //soooo I never use this?
    public GameObject ScorePrefab;
    [Header("Customisable gameplay-ish options")]
    public float refreshFrequency = 2.5f;

    [Header("filled out on Start through code: supposed to be empty: ")]
    public GameObject ratKing;
    public GameObject Scores;
    public GameObject localScore;
    public Image healthGUI;
    public Canvas lobbyCanvas;
    public Text launchGameTx;

    void Awake() //singleton code
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
        if (PlayerPrefs.HasKey("playerName"))
        {
            localName = PlayerPrefs.GetString("playerName");
        }

        launchGameTx.text = "";
        Scores = new GameObject("Scores");
        Scores.AddComponent<NetworkIdentity>();
        Scores.tag = "ThingsHolder";
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
        GameObject[] GUIs = GameObject.FindGameObjectsWithTag("GUI"); //histoire de ne pas avoir quinze mille tags différents
        foreach (GameObject gui in GUIs)
        {
            switch (gui.name)
            {
                case "healthGUI":
                case "healthGUI(Clone)":
                    healthGUI = gui.GetComponent<Image>();
                    break;
                case "LobbyCanvas":
                case "LobbyCanvas(Clone)":
                    lobbyCanvas = gui.GetComponent<Canvas>();
                    break;
                case "LaunchGameTx":
                case "LaunchGameTx(Clone)":
                    launchGameTx = gui.GetComponent<Text>();
                    break;
            }
        }


        listScores = new Score[MenuManager.maxPlayersNumber]; // on crée une liste vide de scores
        if (isServer)
        {
            for (int i = 0; i < listScores.Length; i++) // on crée plein de scores
            {
                GameObject currScore = Instantiate(ScorePrefab);
                NetworkServer.Spawn(currScore);
            }
        }


        GameObject[] listScoresGO = GameObject.FindGameObjectsWithTag("Score"); // on chope les objects des scores qu'on vient de créer
        for (int i = 0; i < listScoresGO.Length; i++)//on met leurs composants score dans la liste
        {
            listScores[i] = listScoresGO[i].GetComponent<Score>();
        }


        InvokeRepeating("RefreshListOfPlayers", 0, refreshFrequency);
    }


    [ClientRpc]
    public void RpcChangeCol(GameObject obj, Color col/*, GameObject attacker*/)
    {
        obj.GetComponent<PlayerHealth>().TakeDamage();
        if (obj.GetComponent<PlayerHealth>().Hp > 0)
        { // pour que la flaque de peinture soit de la dernière couleur vue et pas d'une nouvelle couleur random (cf Kill() ci-dessous)
            Renderer rd = obj.GetComponentInChildren<Renderer>();
            rd.materials[0].color = col;
            if (!obj.GetComponent<PlayerBehaviour>().IsLocalPlayer)
            {
                rd.materials[1].color = col;
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


    /*
    [ClientRpc] public void RpcUpdateScoreHolder(GameObject obj, string name) {
    }

    [ClientRpc] public void RpcSetScoreHolder(GameObject obj, string name) { SetScoreHolderSolo(obj, name); }
    [Command] public void CmdSetScoreHolder(GameObject obj, string name) { RpcSetScoreHolder(obj, name); }
    public void SetScoreHolder(GameObject obj, string name) { CmdSetScoreHolder(obj, name); }
    public void SetScoreHolderSolo(GameObject obj, string name)
    {
        localScore = Instantiate(ScorePrefab);
        NetworkServer.Spawn(localScore);
        localScore.name = "score-" + name;
        Score playerScore = localScore.GetComponent<Score>();
        playerScore.PlayerName = name;
        playerScore.PlayerObj = obj;
        localScore.transform.SetParent(Scores.transform);
        //obj.GetComponent<ScoreKeeper>().playerScore = playerScore;
        //RpcUpdateScoreHolder(obj, name);
    }
    */

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
        GameObject[] listOfPlayers = GameObject.FindGameObjectsWithTag("Player");
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


    public void ResetScore(Score sco)
    {
        sco.i = -1;
        sco.PlayerName = null;
        sco.PlayerObj = null;
    }


    public void RefreshListOfPlayers()
    {
        print("refreshing list of players");
        int numberOfPlayersReady = 0;
        GameObject[] listPlayersGO = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < listPlayersGO.Length; i++)
        {

            if (listScores[i].PlayerObj == null || listScores[i].PlayerObj  != listPlayersGO[i])
            {
                listScores[i].i = i;
                listScores[i].PlayerObj = listPlayersGO[i];
                listScores[i].PlayerName = listPlayersGO[i].GetComponent<PlayerBehaviour>().localName;
                if(listScores[i].PlayerName == "Player")
                {
                    listScores[i].PlayerName = "Player" + i;
                }
                listScores[i].name = "score-" + listScores[i].PlayerName;

            }
                
                string readyState = "not ready...";
                Color txColor = Color.white;
                if (listPlayersGO[i].GetComponent<PlayerBehaviour>().IsReady == true)
                {
                    readyState = "ready!";
                    numberOfPlayersReady++;
                    txColor = Color.green;
                }
                //print(listPlayers[i].PlayerName + " : " + readyState);
                if(listScores[i].ScoreTx == null)
                {
                    float posX = listOfPlayersParent.transform.position.x;
                    float posY = listOfPlayersParent.transform.position.y;
                    listScores[i].ScoreTx = Instantiate(playerStatePrefab, listOfPlayersParent.transform);
                    listScores[i].ScoreTx.transform.position = new Vector2(posX, posY - 20 + i * -20);
                }
                listScores[i].ScoreTx.GetComponent<Text>().text = listScores[i].PlayerName + " : " + readyState;
                listScores[i].ScoreTx.GetComponent<Text>().color = txColor;
            //}
        }
        if (numberOfPlayersReady == listPlayersGO.Length)
        {
            StartCoroutine("launchingGame");
        }
    }
}