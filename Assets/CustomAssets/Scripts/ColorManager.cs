//using System;
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
    public static ColorManager singleton;
    public bool isLocalPlayerDead = false;
    public GameObject ScorePrefab;
    [Header("supposed to be empty in the Editor")]
    public Canvas lobbyCanvas;
    public Text launchGameTx;
    public GameObject listOfPlayersParent;
    public GameObject playerStatePrefab;
    public GameObject ScoresHolderParent;
    public GameObject ratKing;
    public Image healthGUI;
    public GameObject localPlayer;
    public int numberOfPlayersPlaying;
    public Score[] Scores;
    public Text following;
    [Header("tmp: sound stuff")]
    public static AudioClip[] ChangeColSounds;
    public static AudioClip currentMusic;
    private AudioSource audioSource;

    private float refreshFrequency = 2.5f;

    public enum gameState { menu, lobby, playing, scores };
    private gameState currState;

    [SyncVar]
    public string currStateString;

    public gameState CurrState
    {
        get
        {
            return currState;
        }

        set
        {
            currState = value;
            currStateString = currState.ToString();
        }
    }

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
        ratKing = new GameObject("ratKing") { tag = "ThingsHolder" };
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = currentMusic;
        audioSource.loop = true;
    }

    void Start()
    {
        if (isServer){
            CurrState = gameState.lobby;
            currStateString = CurrState.ToString();
        } // should absolutely happen before the local player's PlayerBehaviour Start()
        else
        {
            CurrState = (gameState)System.Enum.Parse(typeof(gameState), currStateString);
        }
        print("current state at start: " + CurrState);
        audioSource.Play();
        checkIfNetworkHUD.ToggleNetworkGUI();
        GameObject[] GUIs = GameObject.FindGameObjectsWithTag("GUI");
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
                case "listOfPlayers":
                case "listOfPlayers(Clone)":
                    listOfPlayersParent = gui;
                    break;
                case "LaunchGameTx":
                case "LaunchGameTx(Clone)":
                    launchGameTx = gui.GetComponent<Text>();
                    break;
                case "following":
                case "following(Clone)":
                    following = gui.GetComponent<Text>();
                    break;
            }
        }

        launchGameTx.text = "";

        /*
        //GameObject[] listPlayersGO = GameObject.FindGameObjectsWithTag("Player");
        if (CurrState != gameState.lobby) // s'il arrive dans un jeu en cours 
        {
            print("GAME IS PLAYING");

            print("destroying score obj");
            Destroy(localPlayer.GetComponent<PlayerBehaviour>().ScoreObj); // that was so assholes who come mid-game died but could still follow it; don't think it works though // parce que pour le ColorManager qui vient d'arriver, le jeu n'est pas isPlaying
            print("destroying score tx");
            Destroy(localPlayer.GetComponent<PlayerBehaviour>().ScoreTx); // that was so assholes who come mid-game died but could still follow it; don't think it works though // parce que pour le ColorManager qui vient d'arriver, le jeu n'est pas isPlaying
            print("destroying player");
            Destroy(localPlayer); // that was so assholes who come mid-game died but could still follow it; don't think it works though // parce que pour le ColorManager qui vient d'arriver, le jeu n'est pas isPlaying


            Scores = ScoresHolderParent.GetComponentsInChildren<Score>();
            foreach (Score sco in Scores)
            {
                sco.ScoreTx = sco.PlayerObj.GetComponent<PlayerBehaviour>().ScoreTx.GetComponent<Text>();
                sco.SetStartTime();
            }
            numberOfPlayersPlaying = GameObject.FindGameObjectsWithTag("Player").Length;
            CurrState = gameState.playing;
            launchGameTx.text = "";
            listOfPlayersParent.SetActive(false);
            lobbyCanvas.enabled = false;



            //LaunchGameSolo(); //il désactive la GUI du lobby
            return;
        }
        */
        Invoke("checkIfGamePlaying", 0.1f);
        if(CurrState == gameState.lobby)
        {
            Invoke("RefreshListOfPlayersSolo", 0.2f);
            if (isServer)
            {
                Invoke("RpcRefreshListOfPlayers", .7f);
            }
            else
            {
                Invoke("RefreshListOfPlayersSolo", .7f);
            }

        }
    }


    void checkIfGamePlaying()
    {
        if (CurrState != gameState.lobby) // s'il arrive dans un jeu en cours 
        {
            print("GAME IS PLAYING");

            print("destroying score obj");
            Destroy(localPlayer.GetComponent<PlayerBehaviour>().ScoreObj); // that was so assholes who come mid-game died but could still follow it; don't think it works though // parce que pour le ColorManager qui vient d'arriver, le jeu n'est pas isPlaying
            print("destroying score tx");
            Destroy(localPlayer.GetComponent<PlayerBehaviour>().ScoreTx); // that was so assholes who come mid-game died but could still follow it; don't think it works though // parce que pour le ColorManager qui vient d'arriver, le jeu n'est pas isPlaying
            print("destroying player");
            Destroy(localPlayer); // that was so assholes who come mid-game died but could still follow it; don't think it works though // parce que pour le ColorManager qui vient d'arriver, le jeu n'est pas isPlaying


            Scores = ScoresHolderParent.GetComponentsInChildren<Score>();
            foreach (Score sco in Scores)
            {
                sco.ScoreTx = sco.PlayerObj.GetComponent<PlayerBehaviour>().ScoreTx.GetComponent<Text>();
                sco.SetStartTime();
            }
            numberOfPlayersPlaying = GameObject.FindGameObjectsWithTag("Player").Length;
            CurrState = gameState.playing;
            launchGameTx.text = "";
            listOfPlayersParent.SetActive(false);
            lobbyCanvas.enabled = false;



            //LaunchGameSolo(); //il désactive la GUI du lobby
            return;
        }
    }

    /*
    [ClientRpc]
    public void RpcSyncGameState(string state)
    {
        print(2);
        currState = (gameState) System.Enum.Parse(typeof(gameState), state);
    }
    */

    public GameObject SpawnScore(string name, GameObject obj)
    {
        GameObject score = Instantiate(ScorePrefab);
        score.name = "score-" + name;
        Score currScore = score.GetComponent<Score>();
        currScore.playerObj = obj;
        currScore.playerName = name;

        return score;
    }


    [ClientRpc]
    public void RpcChangeCol(GameObject obj, Color col, GameObject attacker)
    {
        Score score = obj.GetComponent<PlayerBehaviour>().ScoreObj.GetComponent<Score>();
        obj.GetComponent<PlayerHealth>().TakeDamage();
        PlayerChangeCol objChangeCol = obj.GetComponent<PlayerChangeCol>();
        if (CurrState == gameState.playing)
        { // sound stuff
            AudioSource sound = obj.GetComponent<AudioSource>();
            sound.clip = ChangeColSounds[Random.Range(0, ChangeColSounds.Length)];
            sound.Play();
        }
        if (obj.GetComponent<PlayerHealth>().Hp > 0)
        { // pour que la flaque de peinture soit de la dernière couleur vue et pas d'une nouvelle couleur random (cf Kill() ci-dessous)
            Renderer rd = obj.GetComponentInChildren<Renderer>();
            foreach (Material mat in rd.materials)
            {
                mat.color = col;
            }

            IEnumerator paintCooldownNow = paintCooldown(objChangeCol.cooldown, attacker);
            StartCoroutine(paintCooldownNow);

            if (attacker == obj)
            {
                score.colorChangesFromSelf += 1;
            }
            else if (attacker.CompareTag("AttackChangeCol"))
            {
                score.colorChangesFromMice += 1;
            }
            else if (attacker.CompareTag("Player"))
            {
                score.colorChangesFromOthers += 1;
                attacker.GetComponent<PlayerBehaviour>().ScoreObj.GetComponent<Score>().colorChangesToOthers += 1;
                    attacker.GetComponent<PlayerChangeCol>().paintReady = false;
            }
            if (obj.GetComponent<PlayerBehaviour>().isLocalPlayer)
            {
                IEnumerator speedBoostNow = speedBoost(objChangeCol.speedBoostDuration, objChangeCol.speedBoostStrength, obj, attacker);
                StartCoroutine(speedBoostNow);
            }
        }
    }

    IEnumerator paintCooldown(float cooldown, GameObject attacker)
    {
        yield return new WaitForSeconds(cooldown);
        if (attacker.CompareTag("Player"))
        {
            attacker.GetComponent<PlayerChangeCol>().paintReady = true;
        }
    }

    IEnumerator speedBoost(float duration, float strength, GameObject obj, GameObject attacker)
    {
        PlayerChangeCol objChangeCol = obj.GetComponent<PlayerChangeCol>();
        if (obj == attacker) // so it's twice as expensive to speedBoost to chase someone (if you changed your own colour) as it is if you're running away (if you've been attacked)
        {
            duration *= .5f;
        }
        objChangeCol.currBoost += 1;
        int prevBoost = objChangeCol.currBoost;
        PlayerMove playerMove = obj.GetComponent<PlayerMove>();
        Animator animator = playerMove.animator;
        playerMove.speed = strength;
        animator.speed = 2;
        yield return new WaitForSeconds(duration);
        if (playerMove.speed == strength && prevBoost == objChangeCol.currBoost) // pour qu'il ne sache pas re-bouger s'il est en train de mourir
        {
            playerMove.speed = playerMove.BaseSpeed;
            animator.speed = 1;
        }
    }



    [ClientRpc]
    public void RpcKill(GameObject obj)
    {
        obj.GetComponent<PlayerHealth>().KillSolo();
    }

    public IEnumerator launchingGame() // un message d'erreur dit qu'il ne sait pas se lancer sur le host? mais ça marche quand même, so whatever (check if that's still true) // to be launched only on the server
    {
        RpcLaunchGameTx();
        yield return new WaitForSeconds(2);
        RpcLaunchGame();
    }

    [ClientRpc]
    public void RpcLaunchGameTx()
    {
        launchGameTx.text = "Launching Game...";
        localPlayer.GetComponent<PlayerChangeCol>().startWhite();
    }

    public void LaunchGameSolo()
    {
        Scores = ScoresHolderParent.GetComponentsInChildren<Score>();
        foreach (Score sco in Scores)
        {
            sco.ScoreTx = sco.PlayerObj.GetComponent<PlayerBehaviour>().ScoreTx.GetComponent<Text>();
            sco.SetStartTime();
        }
        numberOfPlayersPlaying = GameObject.FindGameObjectsWithTag("Player").Length;
        CurrState = gameState.playing;
        localPlayer.GetComponent<PlayerMove>().speed = localPlayer.GetComponent<PlayerMove>().BaseSpeed;
        launchGameTx.text = "";
        listOfPlayersParent.SetActive(false);
        lobbyCanvas.enabled = false;
        if (isServer)
        {
            foreach (EnemyMover enemy in EnemySpawner.enemyList)
            {
                IEnumerator wait = enemy.waitForChangeDir(Random.Range(enemy.waitRange.x, enemy.waitRange.y));
                StartCoroutine(wait); //it works ONLY IF I create the coroutine on the previous line and set it up in here instead of in EnemyMover
            }
        }
    }

    [ClientRpc]
    void RpcLaunchGame()
    {
        LaunchGameSolo();
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

    [ClientRpc]
    public void RpcRefreshListOfPlayers() { RefreshListOfPlayersSolo(); }

    public void RefreshListOfPlayersSolo()
    {
        print("refreshing list of players");
        int numberOfPlayersReady = 0;
        GameObject[] listPlayersGO = GameObject.FindGameObjectsWithTag("Player");
        PlayerBehaviour[] listPlayers = new PlayerBehaviour[listPlayersGO.Length];
        for (int i = 0; i < listPlayers.Length; i++)
        {
            listPlayers[i] = listPlayersGO[i].GetComponent<PlayerBehaviour>();
            listPlayers[i].idNumber = i;
            listPlayers[i].ScoreObj.GetComponent<Score>().idNumber = i;
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
            //print(listPlayers[i].localName + " : " + readyState);
            if (listPlayers[i].ScoreTx == null)
            {
                float posX = listOfPlayersParent.transform.position.x;
                float posY = listOfPlayersParent.transform.position.y;
                int offset = (int)Mathf.Round(0.05f * Screen.height);
                listPlayers[i].ScoreTx = Instantiate(playerStatePrefab, listOfPlayersParent.transform);
                listPlayers[i].ScoreTx.transform.position = new Vector2(posX, posY - 20 + i * -offset);
            }
            listPlayers[i].ScoreTx.GetComponent<Text>().text = listPlayers[i].localName + " : " + readyState;
            listPlayers[i].ScoreTx.GetComponent<Text>().color = txColor;
        }
        if (numberOfPlayersReady == listPlayersGO.Length && isServer)
        {
            StartCoroutine("launchingGame");
        }
    }

    IEnumerator waitForGameEnd()
    {
        yield return new WaitForSeconds(1);
        /* //pour si on veut un titre; mais c'est chiant à aligner, donc non.
        launchGameTx.text = "Name/ Time of Death/ Player Changes/ Players Changed/ Mice changes/ Self Changes";
        launchGameTx.fontSize = (int) Mathf.Round(launchGameTx.fontSize*0.7f);
        launchGameTx.transform.position = new Vector2(launchGameTx.transform.position.x - Screen.width*2/5, launchGameTx.transform.position.y + Screen.height * 2 / 5);
        */
        ShowScores();

    }

    private void ShowScores()
    { if (localPlayer && localPlayer.GetComponent<PlayerBehaviour>().isLocalPlayer) { CmdShowScores(); } }
    [Command] private void CmdShowScores() {
        CurrState = gameState.scores;
        RpcShowScores();
    }
    [ClientRpc] private void RpcShowScores() { ShowScoresSolo(); }

    public void ShowScoresSolo()
    {
        lobbyCanvas.enabled = true;
        listOfPlayersParent.SetActive(true);
        for (int i = 0; i < Scores.Length; i++)
        {
            print(Scores[i]);
            print(Scores[i].ScoreTx);
            float PosX = Scores[i].ScoreTx.transform.position.x;
            float PosY = Scores[i].ScoreTx.transform.position.y;
            Scores[i].ScoreTx.color = Color.white;
            Scores[i].ScoreTx.transform.position = new Vector2(PosX - Screen.width * 0.33f, PosY);
            string deathText;
            if (Scores[i].TimeOfDeath == "0")
            {
                deathText = ": Survived To The End! ";
            }
            else
            {
                deathText = ": died at " + Scores[i].TimeOfDeath + "s; ";
            }

            if (!MenuManager.shortScore)
            {
                Scores[i].ScoreTx.text = Scores[i].playerName +
                    deathText +
                    "changed " + Scores[i].colorChangesToOthers +
                    " colors; others changed theirs " + Scores[i].colorChangesFromOthers +
                    " times, mice " + Scores[i].colorChangesFromMice +
                    " times; themselves " + Scores[i].colorChangesFromSelf + " times.";
            }
            else
            {
                Scores[i].ScoreTx.text = Scores[i].playerName +
                    deathText +
                    "Changed others' color " + Scores[i].colorChangesToOthers +
                    " times ";
            }
        }
    }
    

    private void Update()
    {

        if (CurrState == gameState.playing && numberOfPlayersPlaying <= 1 && !MenuManager.soloGame)
        {
            CurrState = gameState.scores;
            StartCoroutine("waitForGameEnd");
        }

        if (Input.GetKeyDown(KeyCode.A)) // testing area //////////////////////////////////////////////////////////////////////////////////
        {
            lobbyCanvas.enabled = !lobbyCanvas.enabled;
            launchGameTx.text = "number of Players Playing: " + numberOfPlayersPlaying;
        }

    }
}