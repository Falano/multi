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
    public static int startHp = 30;
    public static bool soloGame = false;
    public static string startLevel;
    public static int teamwork; // number of different teams; 0 is chacun pour soi
    public static float chrono = 0; // en minutes
    [SerializeField]
    public static int activeScene = 0;
    public static int presetColNr;
    public static int nbScenes;
    public static int musicIndex = 0;
    public static bool shortScore = true; // not really useful, I should just settle on one way to show the score
    public static Color[] colors; //the colors available to the sheep to change into, aka those of the backgrounds materials
    public Material[] colorsMats; // the background's materials
    [SerializeField]
    private Color[] colorsPalette; // all of the sixty colors to choose from in the menu // gonna be useful when I turn the menu all keyboard-friendly
    public Texture2D palette;
    public Image paletteImg;
    public Image presetPalettesImg;

    public Image[] presetColsImg;
    public Color[] colorPresets; // a list that contains all the presets for the colours, as long as you take them by groups of 6

    [Header("All the texts buttons and stuff")]
    public Sprite[] lvlPreviews;
    public Sprite[] palettesPreviews;
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
    public Text presetColsText;

    private Image lvlImg;
    private int foleyVolumeInt = 60;
    private int musicVolumeInt = 60;
    private AudioSource foley;
    private AudioSource music;
    public AudioClip[] musics;
    public AudioClip[] changeColSounds;
    public AudioMixerGroup foleyMixer;
    public AudioMixerGroup musicMixer;

    public static KeyCode interact = KeyCode.Space;
    public static KeyCode selfChange = KeyCode.LeftControl;
    public static KeyCode menu = KeyCode.Escape;
    public static KeyCode forward = KeyCode.UpArrow;
    public static KeyCode left = KeyCode.LeftArrow;
    public static KeyCode right = KeyCode.RightArrow;
    public static KeyCode debug = KeyCode.P;

    public Text interactKeyTx;
    public Text selfChangeKeyTx;
    public Text menuKeyTx;
    public Text forwardKeyTx;
    public Text rightKeyTx;
    public Text leftKeyTx;

    public string PlayerName
    {
        get
        {
            return PlayerPrefs.GetString("playerName");
        }
        set
        {
            PlayerPrefs.SetString("playerName", value);
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

        //setting the options according to the recorded preferences
        if (PlayerPrefs.HasKey("faveMusic"))
        {
            musicIndex = PlayerPrefs.GetInt("faveMusic");
        }
        if (PlayerPrefs.HasKey("interactKey"))
        {
            interact = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("interactKey"));
        }
        if (PlayerPrefs.HasKey("selfChangeKey"))
        {
            selfChange = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("selfChangeKey"));
        }
        if (PlayerPrefs.HasKey("menuKey"))
        {
            menu = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("menuKey"));
        }
        if (PlayerPrefs.HasKey("forwardKey"))
        {
            forward = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("forwardKey"));
        }
        if (PlayerPrefs.HasKey("rightKey"))
        {
            right = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKey"));
        }
        if (PlayerPrefs.HasKey("leftKey"))
        {
            left = (KeyCode)KeyCode.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKey"));
        }
        
         //colors
        GetColorPalette(palette, 10, 6);
        for(int i = 0; i <colorPresets.Length; i++) // mettre les cols à full alpha (parce que j'ai merdé et elle ne le sont pas déjà)
        {
            colorPresets[i].a = 1;
        }
        if (PlayerPrefs.HasKey("col1"))
        {
            colorsMats[1].color = StringToVector4(PlayerPrefs.GetString("col1"));
        }
        if (PlayerPrefs.HasKey("col2"))
        {
            colorsMats[1].color = StringToVector4(PlayerPrefs.GetString("col2"));
        }
        if (PlayerPrefs.HasKey("col3"))
        {
            colorsMats[1].color = StringToVector4(PlayerPrefs.GetString("col3"));
        }
        if (PlayerPrefs.HasKey("col4"))
        {
            colorsMats[1].color = StringToVector4(PlayerPrefs.GetString("col4"));
        }
        if (PlayerPrefs.HasKey("col5"))
        {
            colorsMats[1].color = StringToVector4(PlayerPrefs.GetString("col5"));
        }
        if (PlayerPrefs.HasKey("col6"))
        {
            colorsMats[1].color = StringToVector4(PlayerPrefs.GetString("col6"));
        }


        colors = new Color[colorsMats.Length];
        nbScenes = SceneManager.sceneCountInBuildSettings;
        lvlImg = lvlText.transform.parent.GetComponent<Image>();
        lvlImg.sprite = lvlPreviews[activeScene];
        //initializing the texts with the default values:
        enemyText.text = enemyNumber.ToString();
        hpText.text = (startHp/2).ToString();
        chronoText.text = chrono.ToString();
        lvlText.text = (activeScene + 1).ToString();
        foleyVolumeText.text = foleyVolumeInt.ToString();
        musicVolumeText.text = musicVolumeInt.ToString();
        interactKeyTx.text = interact.ToString();
        selfChangeKeyTx.text = selfChange.ToString();
        menuKeyTx.text = menu.ToString();
        forwardKeyTx.text = forward.ToString();
        rightKeyTx.text = right.ToString();
        leftKeyTx.text = left.ToString();
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
        lvlText.text = (activeScene + 1).ToString();
        NetworkManager.singleton.onlineScene = (activeScene + 1).ToString();
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

    public void TogglePlayFoley()
    {
        foley.clip = changeColSounds[Random.Range(0, changeColSounds.Length)];
        foley.Play();
    }

    public void ChangeMusicIndex(int index)
    {
        ChangeSetting(index, ref musicIndex, musicText, 0, musics.Length - 1);
    }

    public void ChangeMusicVolume(int volume)
    {
        ChangeSetting(volume, ref musicVolumeInt, musicVolumeText, 0, 100);
        musicMixer.audioMixer.SetFloat("musicVol", musicVolumeInt - 40 - musicVolumeInt * 0.5f); //là ça va de +10 à -40 db; 0db est 80 foleyVolumeInt; is it ok? Later
        if (musicVolumeInt == 0)
        { // 0 le mute completement
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
        ChangeSetting(nb*2, ref startHp, hpText, 2, 500);
    }
    public void ChangeSoloGame()
    {
        ChangeSetting(ref soloGame, soloGameText);
    }

    public void ChangeChrono(float nb)
    {
        ChangeSetting(nb, ref chrono, chronoText, 0, 240);
    }

    public void ChangePresetColNr(int nb)
    {
        ChangeSetting(nb, ref presetColNr, presetColsText, 0 , (colorPresets.Length/6)-1);
        for(int i = 0; i < presetColsImg.Length; i++)
        {
            presetColsImg[i].color = colorPresets[(presetColNr*6) +i]; //j'ai six presetColsImg, qui sont les boutons pour changer chaque couleur individuellement; j'ai autant de colorPresets que 6 * les palettes que j'ai préparé, parce qu'il y a 6 couleurs par palette
            presetColsImg[i].GetComponent<ChangeMatColor>().ChangeMatCol();
            print(presetColsImg[i].GetComponent<ChangeMatColor>().targetMat + "'s color is now " + presetColsImg[i].color + " because I'm using the preset number " + presetColNr);
        }
        presetPalettesImg.sprite = palettesPreviews[presetColNr];
    }

    public void ChangeSetting(ref bool setting, Text settingText)
    {
        setting = !setting;
        string settingValue;
        if (setting == true)
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
        if (setting < min)
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
        checkIfNetworkHUD.singleton.ToggleNetworkGUI(state);
    }

    public void ToggleText(Text option)
    {
        option.enabled = !option.enabled;
    }

    public void Toggle(Image img)
    {
        img.enabled = !img.enabled;
    }

    // for the custom keys. Since IEnumerators don't accept ref or out params, I have to do this fucking cumbersome thing. This could be six times as short and less prone to errors.

    public void SetInteractKey()
    {
        StartCoroutine(GetInteract());
    }

    public IEnumerator GetInteract()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);
        foreach (KeyCode keyPressed in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(keyPressed))
            {
                interact = keyPressed;
                PlayerPrefs.SetString("interactKey", keyPressed.ToString());
                interactKeyTx.text = keyPressed.ToString();
            }
        }
    }

    public void SetMenuKey()
    {
        StartCoroutine(GetMenu());
    }

    public IEnumerator GetMenu()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);
        foreach (KeyCode keyPressed in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(keyPressed))
            {
                menu = keyPressed;
                PlayerPrefs.SetString("menuKey", keyPressed.ToString());
                menuKeyTx.text = keyPressed.ToString();
            }
        }
    }
    public void SetSelfKey()
    {
        StartCoroutine(GetSelf());
    }

    public IEnumerator GetSelf()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);
        foreach (KeyCode keyPressed in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(keyPressed))
            {
                selfChange = keyPressed;
                PlayerPrefs.SetString("selfChangeKey", keyPressed.ToString());
                selfChangeKeyTx.text = keyPressed.ToString();
            }
        }
    }
    public void SetFwdKey()
    {
        StartCoroutine(GetFwd());
    }

    public IEnumerator GetFwd()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);
        foreach (KeyCode keyPressed in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(keyPressed))
            {
                forward = keyPressed;
                PlayerPrefs.SetString("forwardKey", keyPressed.ToString());
                forwardKeyTx.text = keyPressed.ToString();
            }
        }
    }
    public void SetRightKey()
    {
        StartCoroutine(GetRight());
    }

    public IEnumerator GetRight()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);
        foreach (KeyCode keyPressed in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(keyPressed))
            {
                right = keyPressed;
                PlayerPrefs.SetString("rightKey", keyPressed.ToString());
                rightKeyTx.text = keyPressed.ToString();
            }
        }
    }
    public void SetLeftKey()
    {
        StartCoroutine(GetLeft());
    }

    public IEnumerator GetLeft()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);
        foreach (KeyCode keyPressed in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKey(keyPressed))
            {
                left = keyPressed;
                PlayerPrefs.SetString("leftKey", keyPressed.ToString());
                leftKeyTx.text = keyPressed.ToString();
            }
        }
    }

    public void ChangeColSprite(Image img)
    {
        Toggle(paletteImg);
        StartCoroutine(GetCol(img));
    }

    public IEnumerator GetCol(Image img)
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        Color prevCol = img.color;
        Color col = palette.GetPixel //remplacer tous les 80 et 48 par la largeur et la hauteur de l'image source
            (Mathf.RoundToInt(
                (Input.mousePosition.x - paletteImg.rectTransform.position.x) 
                * 80/paletteImg.rectTransform.sizeDelta.x*1920/Screen.width), 
                Mathf.RoundToInt(
                    (Input.mousePosition.y - paletteImg.rectTransform.position.y) 
                    * 48/paletteImg.rectTransform.sizeDelta.y*1080 / Screen.height)
                    );
        /*if(((Input.mousePosition.x - paletteImg.rectTransform.position.x) * 80 / paletteImg.rectTransform.sizeDelta.x * 1920 / Screen.width) < 0
            || ((Input.mousePosition.x - paletteImg.rectTransform.position.x) * 80 / paletteImg.rectTransform.sizeDelta.x * 1920 / Screen.width) > 80 ||
                ((Input.mousePosition.y - paletteImg.rectTransform.position.y) * 48 / paletteImg.rectTransform.sizeDelta.y * 1920 / Screen.width) < 0 ||
                ((Input.mousePosition.y - paletteImg.rectTransform.position.y) * 48 / paletteImg.rectTransform.sizeDelta.y * 1920 / Screen.width) > 48
            )
        {
            col = prevCol;
        }*/
        img.color = col;
        img.GetComponent<ChangeMatColor>().ChangeMatCol();
        PlayerPrefs.SetString(img.name, col.ToString());
        Toggle(paletteImg);
    }

    private void GetColorPalette(Texture2D tex, int nbStepsW, int nbStepsH)
    {
        colorsPalette = new Color[nbStepsH * nbStepsW];
        int stepW = palette.width / nbStepsW;
        int stepH = palette.height / nbStepsH;
        for (int i = 0; i < nbStepsW; i++)
        {
            for (int j = 0; j < nbStepsH; j++)
            {
               colorsPalette[(nbStepsH * i) + (j)] = palette.GetPixel(i * stepW, j * stepH);
            }
        }
    }

    public static Vector4 StringToVector4(string sVector)
    {
        // Remove the parentheses
        if (sVector.StartsWith("(") && sVector.EndsWith(")"))
        {
            sVector = sVector.Substring(1, sVector.Length - 2);
        }

        // split the items
        string[] sArray = sVector.Split(',');

        // store as a Vector3
        Vector4 result = new Vector4(
            float.Parse(sArray[0]),
            float.Parse(sArray[1]),
            float.Parse(sArray[2]),
            float.Parse(sArray[3]));

        return result;
    }


    public void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
        interact = KeyCode.Space;
        //PlayerPrefs.SetString("interactKey", interact.ToString());
        interactKeyTx.text = interact.ToString();
        selfChange = KeyCode.LeftControl;
        //PlayerPrefs.SetString("selfChangeKey", selfChange.ToString());
        selfChangeKeyTx.text = selfChange.ToString();
        menu = KeyCode.Escape;
        //PlayerPrefs.SetString("menuKey", menu.ToString());
        menuKeyTx.text = menu.ToString();
        forward = KeyCode.UpArrow;
        //PlayerPrefs.SetString("forwardKey", forward.ToString());
        forwardKeyTx.text = forward.ToString();
        right = KeyCode.RightArrow;
        //PlayerPrefs.SetString("rightKey", right.ToString());
        rightKeyTx.text = right.ToString();
        left = KeyCode.LeftArrow;
        //PlayerPrefs.SetString("leftKey", left.ToString());
        leftKeyTx.text = left.ToString();
    }


    private void Update()
    {
        if (Input.GetKeyDown(debug))
        {
            soloGame = true;
            print("interact: " + interact);
            print("menu: " + menu);
            print("self: " + selfChange);
            print("up: " + forward);
        }
    }
}
