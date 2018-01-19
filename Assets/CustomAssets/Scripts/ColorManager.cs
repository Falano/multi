﻿//using System;
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
    public Text DebugTx;
    [SyncVar]
    public bool SeveralTeamsPlaying = true;
    public Score[] Scores;
    public Text following;
    [Header("tmp: sound stuff")]
    public static AudioClip[] ChangeColSounds;
    public static AudioClip currentMusic;
    private AudioSource audioSource;
    [SyncVar]
    public int teamsNbLocal;

    private float refreshFrequency = 2.5f;
    private int scoreShown;

    public enum gameState { menu, lobby, loading, playing, scores };
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
        ScoresHolderParent = new GameObject("ScoresHolder") { tag = "ThingsHolder" }; //cause I'm using it in the Score's start
        ratKing = new GameObject("ratKing") { tag = "ThingsHolder" };
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = currentMusic;
        audioSource.loop = true;
    }

    void Start()
    {
        if (isServer)
        {
            CurrState = gameState.lobby;
            currStateString = CurrState.ToString();
            //Debug("I'm the host player");
            teamsNbLocal = MenuManager.teamsNb;
        } // should absolutely happen before the local player's PlayerBehaviour Start()
        else
        {
            CurrState = (gameState)System.Enum.Parse(typeof(gameState), currStateString);
        }
        //print("current state at start: " + CurrState);
        audioSource.Play();
        checkIfNetworkHUD.singleton.ToggleNetworkGUI();
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
                case "DebugText":
                case "DebugText(Clone)":
                    DebugTx = gui.GetComponent<Text>();
                    break;
            }
        }

        launchGameTx.text = "";

        Invoke("checkIfGamePlaying", 0.1f);
        if (CurrState == gameState.lobby)
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
        else
        {
            localPlayer.GetComponent<PlayerBehaviour>().CmdRefreshPlayerMidGame();
            LaunchGameSolo();
        }
    }

    public void Debug(string sentence)
    {
        DebugTx.text += "\n" + sentence;
    }

    void checkIfGamePlaying()
    {
        if (CurrState != gameState.lobby) // s'il arrive dans un jeu en cours 
        {
            Debug("player started mid-game");

            //Destroy(localPlayer.GetComponent<PlayerBehaviour>().ScoreObj.gameObject); // that was so assholes who come mid-game died but could still follow it; don't think it works though // parce que pour le ColorManager qui vient d'arriver, le jeu n'est pas isPlaying
            //Destroy(localPlayer.GetComponent<PlayerBehaviour>().ScoreTx); // that was so assholes who come mid-game died but could still follow it; don't think it works though // parce que pour le ColorManager qui vient d'arriver, le jeu n'est pas isPlaying
            // I don't remember what that is this up there

            Scores = ScoresHolderParent.GetComponentsInChildren<Score>();
            foreach (Score sco in Scores)
            {
                sco.ScoreTx = sco.behaviour.ScoreTx.GetComponent<Text>();
                sco.SetStartTime();
            }
            CurrState = gameState.playing;
            launchGameTx.text = "";
            listOfPlayersParent.SetActive(false);
            lobbyCanvas.enabled = false;
            return;
        }
    }

    public GameObject SpawnScore(string name, GameObject obj)
    {
        GameObject score = Instantiate(ScorePrefab);
        score.name = "score-" + name;
        Score currScore = score.GetComponent<Score>();
        currScore.SetPlayerObj(obj);

        return score;
    }


    [ClientRpc]
    public void RpcChangeCol(GameObject obj, int colIndex, GameObject attacker)
    {
        Score score = obj.GetComponent<PlayerBehaviour>().ScoreObj;
        PlayerChangeCol objChangeCol = obj.GetComponent<PlayerChangeCol>();
        PlayerChangeCol atkChangeCol = attacker.GetComponent<PlayerChangeCol>();
        PlayerBehaviour objBehaviour = obj.GetComponent<PlayerBehaviour>();
        PlayerBehaviour atkBehaviour = attacker.GetComponent<PlayerBehaviour>();
        PlayerHealth objHealth = obj.GetComponent<PlayerHealth>();

        int damage = 2;
        if (CurrState == gameState.playing)
        { // sound stuff
            AudioSource sound = obj.GetComponent<AudioSource>();
            sound.clip = ChangeColSounds[Random.Range(0, ChangeColSounds.Length)];
            sound.Play();

            if (objHealth.Hp > 0)
            { // pour que la flaque de peinture soit de la dernière couleur vue et pas d'une nouvelle couleur random (cf Kill() ci-dessous)
                Renderer rd = obj.GetComponentInChildren<Renderer>();
                Color col = MenuManager.curr6Colors[colIndex];
                rd.materials[0].color = col;
                if (objBehaviour.localAlly)
                {
                    col = Color.white;
                }
                if (objBehaviour.isLocalPlayer)
                {
                    col = Color.black;
                }
                rd.materials[1].color = col;

                StartCoroutine(paintCooldown(objChangeCol.cooldown, attacker));

                if (attacker == obj)
                {
                    score.colorChangesFromSelf += 1;
                    damage = 1;
                }
                else if (attacker.CompareTag("AttackChangeCol"))
                {
                    score.colorChangesFromMice += 1;
                }
                else if (attacker.CompareTag("Player"))
                {
                    if (atkBehaviour.team == objBehaviour.team) // quand ce sont deux gens de la même équipe, s'ils sont tous les deux d'accord, ils s'échangent des points de vie
                    {
                        StartCoroutine(sharingCooldown(10, attacker)); // on met l'attacker en sharing
                        damage = 0;
                        if (objChangeCol.Sharing) // si l'obj est en sharing (donc s'ils le sont tous les deux)
                        {
                            if (objHealth.Hp > attacker.GetComponent<PlayerHealth>().Hp)
                            {
                                Debug("given lives from team; obj hp: " + objHealth.Hp + "atk hp: " + attacker.GetComponent<PlayerHealth>().Hp);
                                damage = 1;
                                score.colorChangesGiftedToTeam += 1;
                                atkBehaviour.ScoreObj.colorChangesGiftedByTeam += 1;
                            }
                            else if (objHealth.Hp < attacker.GetComponent<PlayerHealth>().Hp)
                            {
                                Debug("giving lives to team; obj hp: " + objHealth.Hp + "atk hp: " + attacker.GetComponent<PlayerHealth>().Hp);
                                damage = -1;
                                score.colorChangesGiftedByTeam += 1;
                                atkBehaviour.ScoreObj.colorChangesGiftedToTeam += 1;
                            }
                            attacker.GetComponent<PlayerHealth>().TakeDamage(-damage);
                        }
                    }
                    else
                    {
                        score.colorChangesFromOthers += 1;
                        atkBehaviour.ScoreObj.colorChangesToOthers += 1;
                        atkChangeCol.paintReady = false;
                    }
                }
                else
                {
                    score.colorChangesFromGround += 1;
                    damage = 3;

                }
                if (objBehaviour.isLocalPlayer)
                {
                    IEnumerator speedBoostNow = speedBoost(objChangeCol.speedBoostDuration, objChangeCol.speedBoostStrength, obj, attacker);
                    StartCoroutine(speedBoostNow);
                }
            }
            objHealth.TakeDamage(damage);
            //Debug("attacker: " + atkBehaviour.localName + "'s hps: " + attacker.GetComponent<PlayerHealth>().Hp + "\nvictim: " + objBehaviour.localName + "'s hps: " + objHealth.Hp);
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

    IEnumerator sharingCooldown(float duration, GameObject attacker)
    {
        PlayerChangeCol atkChangeCol = attacker.GetComponent<PlayerChangeCol>();
        atkChangeCol.currShare += 1;
        int prevShare = atkChangeCol.currShare;
        atkChangeCol.Sharing = true;
        yield return new WaitForSeconds(duration);
        if (prevShare == atkChangeCol.currShare)
        {
            atkChangeCol.Sharing = false;
            Debug("Imma stop sharing now");
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
        if (CurrState == ColorManager.gameState.playing)
        {
            animator.speed = 2;
        }
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
        currState = gameState.loading;
    }

    public void LaunchGameSolo()
    {
        Scores = ScoresHolderParent.GetComponentsInChildren<Score>();
        for (int i = 0; i < Scores.Length; i++)
        {
            PlayerBehaviour currBehaviour = Scores[i].behaviour;
            Scores[i].ScoreTx = currBehaviour.ScoreTx.GetComponent<Text>();
            Scores[i].SetStartTime();
            if (teamsNbLocal == 0)
            {
                teamsNbLocal = Scores.Length;
            }
            Scores[i].team = (i + teamsNbLocal) % teamsNbLocal;
            currBehaviour.team = Scores[i].team;
        }
        for (int i = 0; i < Scores.Length; i++) // cause if I don't make two different loops, it tries to compare them before localPlyers' team has been assigned
        {
            PlayerBehaviour currBehaviour = Scores[i].behaviour;
            if (currBehaviour.team == localPlayer.GetComponent<PlayerBehaviour>().team)
            {
                currBehaviour.localAlly = true;
            }
            //Debug(currBehaviour.localName + " is in team " + currBehaviour.team + ", local is " + localPlayer.GetComponent<PlayerBehaviour>().team + ", so localAlly is " + currBehaviour.localAlly);
            //Debug(Scores[i].playerName + " is in team " + Scores[i].team + " (from " + teamsNbLocal + " teams total)"); // behaviour.localName is more trustworthy than score.playerName smh
            currBehaviour.DebugFloating(Scores[i].playerName);
        }
        localPlayer.GetComponent<PlayerChangeCol>().startWhite();
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
            if (MenuManager.chrono != 0)
            {
                StartCoroutine(launchChrono(MenuManager.chrono));
            }
        }
        CurrState = gameState.playing;
    }

    IEnumerator launchChrono(float time)
    {
        yield return new WaitForSeconds(time * 60);
        CmdShowScores();
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
            listPlayers[i].ScoreObj.idNumber = i;
            if (listPlayers[i].isLocalPlayer)
            {
                scoreShown = listPlayers[i].idNumber;
            }
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

    [ClientRpc]
    public void RpcRefreshPlayerMidGame() { RefreshPlayerMidGameSolo(); }

    public void RefreshPlayerMidGameSolo()
    {
        GameObject[] listPlayersGO = GameObject.FindGameObjectsWithTag("Player");
        PlayerBehaviour[] listPlayers = new PlayerBehaviour[listPlayersGO.Length];
        for (int i = 0; i < listPlayers.Length; i++)
        {
            listPlayers[i] = listPlayersGO[i].GetComponent<PlayerBehaviour>();
            listPlayers[i].idNumber = i;
            listPlayers[i].ScoreObj.idNumber = i;
            if (listPlayers[i].localName == null || listPlayers[i].localName == "")
            {
                listPlayers[i].localName = "Player" + i.ToString();
            }
            string readyState = "not ready...";
            Color txColor = Color.white;
            if (listPlayers[i].ScoreTx == null)
            {
                float posX = listOfPlayersParent.transform.position.x;
                float posY = listOfPlayersParent.transform.position.y;
                int offset = (int)Mathf.Round(0.05f * Screen.height);
                listPlayers[i].ScoreTx = Instantiate(playerStatePrefab, listOfPlayersParent.transform);
                listPlayers[i].ScoreTx.transform.position = new Vector2(posX, posY - 20 + i * -offset);
                listPlayers[i].ScoreTx.GetComponent<Text>().text = " ";
            }
        }
    }

    IEnumerator waitForGameEnd()
    {
        yield return new WaitForSeconds(1);
        ShowScores();

    }

    private void ShowScores()
    { if (localPlayer && localPlayer.GetComponent<PlayerBehaviour>().isLocalPlayer) { CmdShowScores(); } }
    [Command]
    private void CmdShowScores()
    {
        CurrState = gameState.scores;
        RpcShowScores();
    }

    [ClientRpc] private void RpcShowScores() { ShowScoresSolo(); }

    public void ShowScoresSolo()

    {
        CurrState = gameState.scores;
        lobbyCanvas.enabled = true;
        PrintScoresText(scoreShown);
    }

    private void PrintScoresText(int i)
    {
        string deathText = "Liquefied at " + Scores[i].TimeOfDeath + " seconds.";
        if (Scores[i].TimeOfDeath == "0") {
            deathText = "Solid to the End!";
            //Scores[i].playerName = Scores[i].behaviour.localName;

        }
        following.text = "<size=52><b> " + Scores[i].playerName + " </b></size>\n" +
            deathText + "\n\n\n " +
            "<b><i>Changed another's colour </i></b> <color=lime><b> " + Scores[i].colorChangesToOthers + " </b></color> times.\n" +
            "Got their own color changed <b><i>by other sheep</i></b> <color=lime><b>" + Scores[i].colorChangesFromOthers + "</b></color> times.\n" +
            "Got their own color changed <b><i>by mice</i></b> <color=lime><b>" + Scores[i].colorChangesFromMice + "</b></color> times.\n" +
            "Got their own color changed <b><i>by staying still for too long </i></b> <color=lime><b>" + Scores[i].colorChangesFromGround + "</b></color> times.\n" +
            "<b><i>Decided to change their own color</i></b> <color=lime><b>" + Scores[i].colorChangesFromSelf + "</b></color> times.\n" +
            "<b><i>Gave an extra colour change</i></b> to one of their team<color=lime><b> " + Scores[i].colorChangesGiftedToTeam + "</b></color> times.\n" +
            "<b><i>Received an extra colour change</i></b> from one of their team<color=lime><b> " + Scores[i].colorChangesGiftedByTeam + "</b></color> times.\n" +
        "\n\n\n Congrats!\n\n" +
            "(press <b>" + MenuManager.left + "</b> or <b>" + MenuManager.right + "</b> to see other's scores)";
    }


    private void Update()
    {
        if (CurrState == gameState.playing && !MenuManager.soloGame &&
            !SeveralTeamsPlaying /*nope: number of teams playing*/)
        {
            CurrState = gameState.scores;
            StartCoroutine("waitForGameEnd");
        }

        if (CurrState == gameState.scores)
        {
            if (Input.GetKeyDown(MenuManager.left))
            {
                scoreShown--;
                if(scoreShown < 0)
                {
                    scoreShown = Scores.Length - 1;
                }
                PrintScoresText(scoreShown);
            }
            else if (Input.GetKeyDown(MenuManager.right))
            {
                scoreShown++;
                if(scoreShown >= Scores.Length)
                {
                    scoreShown = 0;
                }
                PrintScoresText(scoreShown);
            }
        }

        if (Input.GetKeyDown(MenuManager.debug)) // testing area //////////////////////////////////////////////////////////////////////////////////
        {
            lobbyCanvas.enabled = !lobbyCanvas.enabled;
        }
    }
}