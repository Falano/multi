using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MenuManager : MonoBehaviour
{
    public static MenuManager singleton;
    public Scene[] scenes;
    // Default Game Options that you can't change in the editor because they're static
    // should I have a non-static variable that the static ones take after so the designer can change it?
    public static int enemyNumber = 10;
    public static int startHp = 20;
    public static bool soloGame = true;
    public static string startLevel;
    public static int teamwork; // number of different teams; 0 is chacun pour soi
    public static float chrono = 0; // en minutes
    [SerializeField]
    public static int activeScene = 0;
    public static int nbScenes;
    public static int musicIndex = 0;
    private string playerName;
    public static bool shortScore = true; // not really useful, I should just settle on one way to show the score
    public static Color[] colors;
    [SerializeField]
    private Material[] colorsMats;

    //private NetworkLobbyManager lobbyManager; //only useful for lobby version

    [Header("All the texts buttons and stuff")]
    public Sprite[] lvlPreviews;
    [Tooltip("the 'enemy number' text object")]
    public Text enemyText;
    [Tooltip("the 'lives' text object")]
    public Text hpText;
    [Tooltip("the 'chrono' text object; aka if the game has a fixed duration")]
    public Text chronoText;
    [Tooltip("the 'Level' text object; aka which level we're playing (duh)")]
    public Text lvlText;
    public Text soloGameText;
    public Text musicText;
    public Text playMusicText;
    private Image lvlImg;
    private AudioSource audiosource;    
    public AudioClip[] musics;


    public string PlayerName
    {
        get
        {
            return PlayerPrefs.GetString("playerName");
        }
        set
        {
            PlayerPrefs.SetString("playerName", value);
            playerName = value;
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
    }



    public void Start()
    {
        if (PlayerPrefs.HasKey("faveMusic"))
        {
            musicIndex = PlayerPrefs.GetInt("faveMusic");
        }
        colors = new Color[colorsMats.Length];
        nbScenes = SceneManager.sceneCountInBuildSettings;
        lvlImg = lvlText.transform.parent.GetComponent<Image>();
        lvlImg.sprite = lvlPreviews[activeScene];
        //now: initializing the texts with the default values:
        enemyText.text = enemyNumber.ToString();
        hpText.text = startHp.ToString();
        chronoText.text = chrono.ToString();
        if (PlayerPrefs.HasKey("faveMusic"))
        {
            musicIndex = PlayerPrefs.GetInt("faveMusic");
        }
        musicText.text = musicIndex.ToString();
        if (soloGame)
        {
            soloGameText.text = "yes";
        }
        else
        {
            soloGameText.text = "no";
        }



        audiosource = gameObject.GetComponent<AudioSource>();
        audiosource.clip = musics[musicIndex];
        SetInputField();
    }

    public void RefreshColors()
    {
        for (int i = 0; i < colorsMats.Length; i++)
        {
            colors[i] = colorsMats[i].color;
        }
    }

    public void setMusic()
    {
        ColorManager.currentMusic = musics[musicIndex];
        PlayerPrefs.SetInt("faveMusic", musicIndex);
    }


    public void SetInputField()
    {
        InputField nameField = GetComponentInChildren<InputField>();
        if (PlayerPrefs.HasKey("playerName"))
        {
            nameField.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            nameField.GetComponentsInChildren<Text>()[0].text = "";
            nameField.text = PlayerName;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void ChangeStartScene(int change)
    {
        activeScene = (activeScene + change + (nbScenes - 1)) % (nbScenes - 1); //parce qu'il ne faut pas tomber sur le menu
        //print("activeScene = " + activeScene + ", change = "+change+", nbScenes = "+nbScenes+ "; \n(activeScene+change+nbScenes)%nbScenes = " + (activeScene+change+nbScenes)%nbScenes);
        lvlText.text = (activeScene + 1).ToString();
        NetworkManager.singleton.onlineScene = (activeScene + 1).ToString();
        lvlImg.sprite = lvlPreviews[activeScene];
    }

    public void TogglePlayMusic()
    {
        if (!audiosource.isPlaying)
        {
            audiosource.clip = musics[musicIndex];
            audiosource.Play();
            playMusicText.text = "Stop";
        }
        else
        {
            audiosource.Stop();
            playMusicText.text = "Play";
        }
    }

    public void ChangeMusicIndex(int index)
    {
        if (musicIndex+index < 0 || musicIndex+index >= musics.Length)
        {
            if(index < 0)
            {
                musicIndex = 0 - index; 
            }
            else if (index > 0)
            {
                musicIndex = musics.Length -1 - index;

            }
        }
        ChangeSetting(index, ref musicIndex, musicText);
    }


    public void ChangeNbrEnemies(int nb)
    {
        ChangeSetting(nb, ref enemyNumber, enemyText);
    }

    public void ChangeStartHp(int nb)
    {
        ChangeSetting(nb, ref startHp, hpText);
    }
    public void ChangeSoloGame()
    {
        ChangeSetting(ref soloGame, soloGameText);
    }

    public void ChangeChrono(float nb)
    {
        ChangeSetting(nb, ref chrono, chronoText);
    }

    public void ChangeSetting(ref bool setting, Text settingText)
    {
        setting = !setting;
        string settingValue;
        if(setting == true)
        {
            settingValue = "yes";
        }
        else
        {
            settingValue = "no";
        }
        settingText.text = settingValue;
    }

    public void ChangeSetting(int nb, ref int setting, Text settingText)
    {
        setting += nb;
        settingText.text = setting.ToString();
    }

    public void ChangeSetting(float nb, ref float setting, Text settingText)
    {
        setting += nb;
        settingText.text = setting.ToString("F1");
    }

    public void ToggleNetworkManagerHUD(bool state)
    {
        checkIfNetworkHUD.singleton.GetComponent<NetworkManagerHUD>().enabled = state;
    }

    public void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //testing area
            print(PlayerPrefs.GetString("playerName"));
        }
    }
}
