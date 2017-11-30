using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private Image healthGUI;
    public Text following;
    private Canvas lobbyCanvas;
    private Text launchGameTx;

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
    }

    // Use this for initialization
    void Start () {
        currState = gameState.lobby;
        textNarr = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<TextMesh>();
        NPSs = GameObject.FindGameObjectsWithTag("NPS");
       foreach(GameObject NPS in NPSs)
        {
            while(NPS.GetComponent<TutoStats>().localName == null)
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
            }
        }
        launchGameTx.text = "";
    }

    public void speak(string sentence, TextMesh texte, float duration)
    {
        texte.text = sentence;
        StartCoroutine(finishSpeaking(sentence, texte, duration));
    }

    IEnumerator finishSpeaking (string sentence, TextMesh texte, float duration)
    {
        yield return new WaitForSeconds(duration);
        if(texte.text == sentence)
        {
            texte.text = "";
        }
    }

    IEnumerator startingGame()
    {
        launchGameTx.text = "LaunchingGame";
        yield return new WaitForSeconds(2);
        StartGame();
    }

    private void StartGame()
    {
        currState = gameState.playing;
        lobbyCanvas.enabled = false;
        launchGameTx.text = "";
    }

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space) && currState == gameState.lobby)
        {
            StartGame();
        }
	}
}
