﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutoManager : MonoBehaviour
{
    public static TutoManager singleton;
    public enum gameState { lobby, playing, deadPlayer, scores };
    public enum toDo { A_menu, B_changeTeam, C_launchGame, D_move, E_bully, F_kill, G_mice, H_ground, I_selfChange, J_unwillingTeam, K_willingTam, L_die, M_stalker, N_showScores, N_showScores2, O_changeScore, P_quit };
    public gameState currState;
    public toDo currTask;
    public TextMesh textNarr;
    public GameObject[] NPSs;
    private string[] localNames;
    public Sprite[] sprites;
    public AudioClip[] ChangeColSounds;
    private GameObject player;
    private TutoChangeCol playerChangeCol;
    private TutoAssignScores scoresManager;
    TutoChangeCol[] changeCols;

    [SerializeField]
    private Material[] colorsMats;
    public static Color[] colors;
    public Image healthGUI;
    public Text following;
    private Canvas lobbyCanvas;
    private Text launchGameTx;
    public Text instructionsTx;
    public TextMesh NarrTx;
    Text playerReadyTx;
    string localName = "Player";
    //public bool coroutinesRunning = false;
    Canvas menuOutCanvas;

    int i;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }

        colors = new Color[colorsMats.Length]; //I need this in a Start
        for (int i = 0; i < colorsMats.Length; i++)
        {
            colors[i] = colorsMats[i].color;
        }
        localNames = new string[] { "Romuald", "Zobeide", "Trompette", "Rotule", "Medor", "Jules", "Infarctule", "Gaelann", "Fossette", "Egoinne", "Ciboulette", "Cannelle", "Bretelle", "Bobinette", "Bidule", "Asphodèle", "Andromaque", "Articule", "Barnécide", "Catapulle", "Dardielle", "Escopette", "Frisquette", "Glavicule", "Houlette", "Juliette", "Lola", "Marionnette", "Notule", "Operette", "Poutchinette", "Tentacule", "Vesicule" };
        Destroy(GameObject.FindGameObjectWithTag("NetworkManager"));
        //textNarr = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<TextMesh>(); // since it's in the update if I leave it in Start it throws half a dozen error messages
        foreach (GameObject gui in GameObject.FindGameObjectsWithTag("GUI")) //cause other scripts use it and if in the Start they'll try to grab it before TutoManager knows what they are
        {
            switch (gui.name)
            {
                case "NarrText":
                    NarrTx = gui.GetComponent<TextMesh>();
                    break;
                case "healthGUI":
                    healthGUI = gui.GetComponent<Image>();
                    break;
                case "following":
                    following = gui.GetComponent<Text>();
                    break;
                case "LobbyCanvas":
                    lobbyCanvas = gui.GetComponent<Canvas>();
                    break;
                case "LaunchGameTx":
                    launchGameTx = gui.GetComponent<Text>();
                    break;
                case "PlayerReady":
                    playerReadyTx = gui.GetComponent<Text>();
                    break;
                case "MenuOutCanvas":
                    menuOutCanvas = gui.GetComponent<Canvas>();
                    break;
                case "Instructions":
                    instructionsTx = gui.GetComponent<Text>();
                    break;
            }
            instructionsTx.text = "";
        }
    }

    // Use this for initialization
    void Start()
    {
        currState = gameState.lobby;
        player = GameObject.FindGameObjectWithTag("Player");
        playerChangeCol = player.GetComponent<TutoChangeCol>();
        NPSs = GameObject.FindGameObjectsWithTag("NPS");
        int k = 0;
        changeCols = new TutoChangeCol[NPSs.Length + 1];
        changeCols[NPSs.Length] = playerChangeCol;
        foreach (GameObject NPS in NPSs)
        {
            changeCols[k] = NPS.GetComponent<TutoChangeCol>();
            changeCols[k].ChangeCol(colors[1]);
            k += 1;
            while (NPS.GetComponent<TutoChangeCol>().localName == null || NPS.GetComponent<TutoStats>().localName == "")
            {
                int i = Random.Range(0, localNames.Length - 1);
                NPS.GetComponent<TutoStats>().localName = localNames[i];
                localNames[i] = null;
            }
        }
        launchGameTx.text = "";
        instructions("Press <b>" + MenuManager.menu + "</b> twice to toggle the menu. Do it.", toDo.A_menu);
        if (PlayerPrefs.HasKey("playerName"))
        {
            localName = PlayerPrefs.GetString("playerName");
        }
        playerReadyTx.text = localName + " (team " + playerChangeCol.team + ") : not ready";
    }

    public void speak(string sentence, TextMesh texte, float duration)
    {
        StopCoroutine("speak");
        texte.text = sentence;
        //texte.color = new Color(0, 0, 0, 0); // I left the textFade out because it stops working after the player changes colour even once. (the fuck?)
        //StartCoroutine(fadeText(1, texte));
        StartCoroutine(finishSpeaking(sentence, texte, duration));
    }


    public void instructions(string sentence, toDo state)
    {
        NarrTx.text = instructionsTx.text;
        instructionsTx.text = sentence;
        currTask = state;
    }


    //IEnumerator fadeText(int goal, TextMesh texte)
    //{
    //    int mult = 1;
    //    if (goal == 0)
    //    {
    //        mult = -1;
    //    }
    //    while (texte.color.a != goal)
    //    {
    //        yield return new WaitForEndOfFrame();
    //        texte.color = new Color(texte.color.r, texte.color.g, texte.color.b, texte.color.a + .05f);
    //    }
    //}

    IEnumerator finishSpeaking(string sentence, TextMesh texte, float duration)
    {
        //coroutinesRunning = true;
        yield return new WaitForSeconds(duration);
        //StartCoroutine(fadeText(0, texte));
        //        yield return new WaitUntil(texte.color.a == 0);
        //yield return new WaitForSeconds(1);
        if (texte && texte.text == sentence)
        {
            texte.text = "";
        }
        //coroutinesRunning = false;
    }

    void ToggleMenuButton()
    {
        menuOutCanvas.enabled = !menuOutCanvas.enabled;
    }

    public void backToMenu()
    {
        SceneManager.LoadScene("menu");
    }

    IEnumerator startingGame()
    {
        playerReadyTx.text = localName + " (team " + playerChangeCol.team + ") : ready!";
        playerReadyTx.color = Color.green;
        launchGameTx.text = "LaunchingGame...";
        yield return new WaitForSeconds(2);
        StartGame();
    }

    private void StartGame()
    {
        currState = gameState.playing;
        lobbyCanvas.enabled = false;
        launchGameTx.text = "";
        if (playerChangeCol.team == -1)
        {
            playerChangeCol.team = Random.Range(0, 3);
        }
        //textNarr.text = "";
    }

    private void ShowScores()
    {
        currState = gameState.scores;
        lobbyCanvas.enabled = true;
        string deathText = "Liquefied at " + scoresManager.scores[i].TimeOfDeath + " seconds.";
        if (scoresManager.scores[i].TimeOfDeath == "0")
        {
            deathText = "Solid to the End!";
            //Scores[i].playerName = Scores[i].behaviour.localName;

        }
        following.text = "<size=52><b> " + scoresManager.scores[i].playerName + " </b></size>\n" +
            "team " + scoresManager.scores[i].team
            + "\n" +
            deathText + "\n\n\n " +
            "<b><i>Changed another's colour </i></b> <color=lime><b> " + scoresManager.scores[i].colorChangesToOthers + " </b></color> times.\n" +
            "Got their own color changed <b><i>by other sheep</i></b> <color=lime><b>" + scoresManager.scores[i].colorChangesFromOthers + "</b></color> times.\n" +
            "Got their own color changed <b><i>by mice</i></b> <color=lime><b>" + scoresManager.scores[i].colorChangesFromMice + "</b></color> times.\n" +
            "Got their own color changed <b><i>by staying still for too long </i></b> <color=lime><b>" + scoresManager.scores[i].colorChangesFromGround + "</b></color> times.\n" +
            "<b><i>Decided to change their own color</i></b> <color=lime><b>" + scoresManager.scores[i].colorChangesFromSelf + "</b></color> times.\n" +
            "<b><i>Gave an extra colour change</i></b> to one of their team<color=lime><b> " + scoresManager.scores[i].colorChangesGiftedToTeam + "</b></color> times.\n" +
            "<b><i>Received an extra colour change</i></b> from one of their team<color=lime><b> " + scoresManager.scores[i].colorChangesGiftedByTeam + "</b></color> times.\n" +
        "\n\n\n Congrats!\n\n" +
            "(press <b>" + MenuManager.left + "</b> or <b>" + MenuManager.right + "</b> to see other's scores)";

    }

    void Update()
    {
        if (Input.GetKeyDown(MenuManager.interact) && currState == gameState.lobby && currTask == toDo.C_launchGame)
        {
            StartCoroutine(startingGame());
            instructions("You can move with the <b>" + MenuManager.forward + "</b> key\nand turn with <b>" + MenuManager.left + "</b> and <b>" + MenuManager.right + "</b>", toDo.D_move);
        }
        if (Input.GetKeyDown(MenuManager.menu))
        {
            ToggleMenuButton();
            if (currTask == toDo.A_menu)
                instructions("Press <b>" + MenuManager.left + "</b> or <b>" + MenuManager.right + "</b> to choose your team. \n '?' lets the computer choose.\n White-legged sheep are in your team.", toDo.B_changeTeam);
            else if (currTask == toDo.N_showScores)
                currTask = toDo.N_showScores2;
            else if (currTask == toDo.N_showScores2)
            {
                ShowScores();
                instructions("Press <b>" + MenuManager.right + "</b> or <b>" + MenuManager.left + "</b> to see the other's scores.", toDo.O_changeScore);
            }
        }
        if (Input.GetKeyDown(MenuManager.left) || Input.GetKeyDown(MenuManager.right))
        {
            if (currTask == toDo.B_changeTeam)
                instructions("Press <b>" + MenuManager.interact + "</b> to signal you're ready. \n The game starts once all players are.", toDo.C_launchGame);
            else if (currTask == toDo.D_move)
                instructions("Find a monochromatic sheep. \nWhen close, look towards it and press <b>" + MenuManager.interact + "</b>\nOthers see you too with colored legs", toDo.E_bully);
            else if (currTask == toDo.O_changeScore)
                instructions("You can disconnect using the menu in the top left corner.", toDo.P_quit);

            if (Input.GetKeyDown(MenuManager.left))
            {
                if (currState == gameState.lobby)
                {
                    playerChangeCol.team -= 1;
                    playerChangeCol.team = (playerChangeCol.team < -1) ? 3 - 1 : playerChangeCol.team; //il y a trois teams en tout
                    playerChangeCol.team = (playerChangeCol.team > 3 - 1) ? -1 : playerChangeCol.team;
                    playerReadyTx.text = localName + " (team " + ((playerChangeCol.team == -1) ? "?" : playerChangeCol.team.ToString()) + ") : not ready";
                }
                else if (currState == gameState.scores)
                {
                    i--;
                    ShowScores();
                }
            }
            else if (Input.GetKeyDown(MenuManager.right))
            {
                if (currState == gameState.lobby)
                {
                    playerChangeCol.team += 1;
                    playerChangeCol.team = (playerChangeCol.team < -1) ? 3 - 1 : playerChangeCol.team; //il y a trois teams en tout
                    playerChangeCol.team = (playerChangeCol.team > 3 - 1) ? -1 : playerChangeCol.team;
                    playerReadyTx.text = localName + " (team " + ((playerChangeCol.team == -1) ? "?" : playerChangeCol.team.ToString()) + ") : not ready";
                    foreach (TutoChangeCol change in changeCols)
                    {
                        change.ChangeCol(change.colors[1]);
                    }
                }

                else if (currState == gameState.scores)
                {
                    i--;
                    ShowScores();
                }
            }
        }



        if (Input.GetKeyDown(KeyCode.P))// debug
            print("current state: " + currState);




    }
}
