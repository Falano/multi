using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Audio;

public class MenuManager : MonoBehaviour
{
    public static MenuManager singleton;
    public Scene[] scenes;
    // Default Game Options that you can't change in the editor because they're static
    // should I have a non-static variable that the static ones take after so the designer can change it?
    public static int enemyNumber = 10;
    public static int startHp = 20;
    public static bool soloGame = false;
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
    public Text foleyVolumeText;
    public Text musicVolumeText;

    private Image lvlImg;
    private int foleyVolumeInt = 60;
    private int musicVolumeInt = 60;
    private AudioSource foley;
    private AudioSource music;
    public AudioClip[] musics;
    public AudioClip[] changeColSounds;
    public AudioMixerGroup foleyMixer;
    public AudioMixerGroup musicMixer;


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
        //get the audiosources right
        foreach (AudioSource audio in GetComponents<AudioSource>())
        {
            if (audio.outputAudioMixerGroup == foleyMixer)
            {
                foley = audio;
            }
            else if (audio.outputAudioMixerGroup == musicMixer)
            {
                music = audio;
            }
        }

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
        lvlText.text = (activeScene+1).ToString();
        foleyVolumeText.text = foleyVolumeInt.ToString();
        musicVolumeText.text = musicVolumeInt.ToString();
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
        ColorManager.ChangeColSounds = changeColSounds;
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

    public void Tuto()
    {
        SceneManager.LoadScene("tuto");
    }

    public void ChangeStartScene(int change)
    {
        activeScene = (activeScene + change + (nbScenes - 2)) % (nbScenes - 2); //parce qu'il ne faut pas tomber sur le menu
        if (activeScene < 0)
        {
            activeScene = nbScenes - 3;
        }
        else if (activeScene > nbScenes - 3)
        {
            activeScene = 0;
        }
        lvlText.text = (activeScene+1).ToString();
        NetworkManager.singleton.onlineScene = (activeScene+1).ToString();
        lvlImg.sprite = lvlPreviews[activeScene];
    }

    public void TogglePlayMusic()
    {
        if (!music.isPlaying)
        {
            music.clip = musics[musicIndex];
            music.Play();
            playMusicText.text = "Stop";
        }
        else
        {
            music.Stop();
            playMusicText.text = "Play";
        }
    }

    // check all the sound variables, I may have made some mistakes

    public void TogglePlayFoley()
    {
        foley.clip = changeColSounds[Random.Range(0, changeColSounds.Length)];
        foley.Play();
    }

    public void ChangeMusicIndex(int index)
    {
        ChangeSetting(index, ref musicIndex, musicText, 0, musics.Length - 1);
    }

    public void ChangeMusicVolume(int volume) {

        ChangeSetting(volume, ref musicVolumeInt, musicVolumeText, 0, 100);
        musicMixer.audioMixer.SetFloat("musicVol", musicVolumeInt - 40 - musicVolumeInt * 0.5f); //là ça va de +10 à -40 db; 0db est 80 foleyVolumeInt; is it ok? Later
        if(musicVolumeInt == 0){ // 0 le mute completement
        musicMixer.audioMixer.SetFloat("musicVol", -70);
        }
    }

    public void ChangeFoleyVolume(int volume)
    {
        ChangeSetting(volume, ref foleyVolumeInt, foleyVolumeText, 0, 100);
        foleyMixer.audioMixer.SetFloat("foleyVol", foleyVolumeInt - 40 - foleyVolumeInt * 0.5f); // là ça va de +10 à -40 db; 0db est 80 foleyVolumeInt 
        if (foleyVolumeInt == 0)
        {
            foleyMixer.audioMixer.SetFloat("foleyVol", -70);
        }
    }

    public void ChangeNbrEnemies(int nb)
    {
        ChangeSetting(nb, ref enemyNumber, enemyText, 0, 200);
    }

    public void ChangeStartHp(int nb)
    {
        ChangeSetting(nb, ref startHp, hpText, 1, 250);
    }
    public void ChangeSoloGame()
    {
        ChangeSetting(ref soloGame, soloGameText);
    }

    public void ChangeChrono(float nb)
    {
        ChangeSetting(nb, ref chrono, chronoText, 0, 240);
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

    public void ChangeSetting(int nb, ref int setting, Text settingText, int min, int max)
    {
        setting += nb;
        settingText.text = setting.ToString();
        if(setting < min)
        {
            ChangeSettingAbsolute(max, ref setting, settingText);
        }
        else if (setting > max)
        {
            ChangeSettingAbsolute(min, ref setting, settingText);
        }
    }

    public void ChangeSettingAbsolute(int nb, ref int setting, Text settingText)
    {
        setting = nb;
        settingText.text = setting.ToString();
    }

    public void ChangeSetting(float nb, ref float setting, Text settingText, float min, float max)
    {
        setting += nb;
        settingText.text = setting.ToString("F1");

        if (setting < min)
        {
            ChangeSettingAbsolute(max, ref setting, settingText);
        }
        else if (setting > max)
        {
            ChangeSettingAbsolute(min, ref setting, settingText);
        }
    }

    public void ChangeSettingAbsolute(float nb, ref float setting, Text settingText)
    {
        setting = nb;
        settingText.text = setting.ToString();
    }


    public void ToggleNetworkManagerHUD(bool state)
    {
        checkIfNetworkHUD.ToggleNetworkGUI(state);
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
