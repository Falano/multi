using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutoManager : MonoBehaviour {
    public static TutoManager singleton;
    public enum gameState {lobby, playing, deadPlayer};
    public gameState currState;
    public TextMesh textNarr;
    public GameObject[] NPSs;
    private string[] localNames;

    public AudioClip[] ChangeColSounds;

    [SerializeField]
    private Material[] colorsMats;
    public static Color[] colors;
    public Image healthGUI;
    public Text following;
    private Canvas lobbyCanvas;
    private Text launchGameTx;
    Text playerReadyTx;
    string localName = "Player";
    public bool coroutinesRunning = false;
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
    }

    // Use this for initialization
    void Start () {
        currState = gameState.lobby;
        textNarr = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<TextMesh>();
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
       foreach (GameObject gui in GameObject.FindGameObjectsWithTag("GUI"))
        {
            switch (gui.name) {

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
            }
        }
        launchGameTx.text = "";
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
        StartCoroutine(finishSpeaking(sentence, texte, duration));
    }

    IEnumerator finishSpeaking (string sentence, TextMesh texte, float duration)
    {
        coroutinesRunning = true;
        yield return new WaitForSeconds(duration);
        if(texte && texte.text == sentence)
        {
            texte.text = "";
        }
        coroutinesRunning = false;
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
    }

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space) && currState == gameState.lobby)
        {
            StartCoroutine(startingGame());
        }
        if (Input.GetKeyDown(KeyCode.Escape)){
            ToggleMenuButton();
        }

    }
}
