using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutoManager : MonoBehaviour {
    public static TutoManager singleton;
    public enum gameState {lobby, playing, deadPlayer};
    public enum toDo {escape, space, rat, ctrl, nothing};
    public gameState currState;
    public toDo currTask;
    public TextMesh textNarr;
    public GameObject[] NPSs;
    private string[] localNames;
    public Sprite[] sprites;
    public AudioClip[] ChangeColSounds;

    [SerializeField]
    private Material[] colorsMats;
    public static Color[] colors;
    public Image healthGUI;
    public Text following;
    private Canvas lobbyCanvas;
    private Text launchGameTx;
    public Text instructionsTx;
    Text playerReadyTx;
    string localName = "Player";
    //public bool coroutinesRunning = false;
    Canvas menuOutCanvas;


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
        localNames = new string[] {"Romuald", "Zobeide", "Trompette", "Rotule", "Medor", "Jules", "Infarctule", "Gaelann", "Fossette", "Egoinne", "Ciboulette", "Cannelle", "Bretelle", "Bobinette", "Bidule", "Asphodèle", "Andromaque", "Articule", "Barnécide", "Catapulle", "Dardielle", "Escopette", "Frisquette", "Glavicule", "Houlette", "Juliette", "Lola", "Marionnette", "Notule", "Operette", "Poutchinette", "Tentacule", "Vesicule" } ;
        Destroy(GameObject.FindGameObjectWithTag("NetworkManager"));
        textNarr = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<TextMesh>(); // since it's in the update if I leave it in Start it throws half a dozen error messages
        foreach (GameObject gui in GameObject.FindGameObjectsWithTag("GUI")) //cause other scripts use it and if in the Start they'll try to grab it before TutoManager knows what they are
        {
            switch (gui.name)
            {

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
        }
    }

    // Use this for initialization
    void Start () {
        currState = gameState.lobby;
        NPSs = GameObject.FindGameObjectsWithTag("NPS");
       foreach(GameObject NPS in NPSs)
        {
            while(NPS.GetComponent<TutoStats>().localName == null || NPS.GetComponent<TutoStats>().localName == "")
            {
                int i = Random.Range(0, localNames.Length);
                NPS.GetComponent<TutoStats>().localName = localNames[i];
                localNames[i] = null;
            } 
        }
        launchGameTx.text = "";
        instructions("Press <b>"+MenuManager.menu+"</b> to toggle the menu and those instructions\nDo it twice now.", toDo.escape);
        if (PlayerPrefs.HasKey("playerName"))
        {
            localName = PlayerPrefs.GetString("playerName");
        }
        playerReadyTx.text = localName + " : not ready"; 
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
        instructionsTx.text = sentence;
        currTask = state;
    }


    IEnumerator fadeText(int goal, TextMesh texte)
    {
        int mult = 1;
        if(goal == 0)
        {
            mult = -1;
        }
        while (texte.color.a != goal)
        {
            yield return new WaitForEndOfFrame();
            texte.color = new Color(texte.color.r, texte.color.g, texte.color.b, texte.color.a + .05f);
        }
    }

    IEnumerator finishSpeaking (string sentence, TextMesh texte, float duration)
    {
        //coroutinesRunning = true;
        yield return new WaitForSeconds(duration);
        //StartCoroutine(fadeText(0, texte));
        //        yield return new WaitUntil(texte.color.a == 0);
        //yield return new WaitForSeconds(1);
        if(texte && texte.text == sentence)
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
        playerReadyTx.text = localName + " : ready!";
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
        textNarr.text = "";
        instructions("when you're looking at another sheep from very close,\n <b>"+MenuManager.interact+"</b> makes them change color.\nOther players see you like you see them: \nall of one color; not like you see yourself", toDo.space);
    }

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(MenuManager.interact) && currState == gameState.lobby && currTask == toDo.space)
        {
            StartCoroutine(startingGame());
        }
        if (Input.GetKeyDown(MenuManager.menu)){
            ToggleMenuButton();
            if(currState == gameState.lobby)
            {
                instructions("Press <b>"+MenuManager.interact+"</b> to launch the game", toDo.space);
            }
        }
    }
}
