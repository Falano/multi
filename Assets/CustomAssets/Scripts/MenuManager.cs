using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Audio;
using System.Text;

public class MenuManager : MonoBehaviour
{
    public static MenuManager singleton;
    public Scene[] scenes;
    private Dictionary<int, string> levelsComments; //comments in the menu to help choose an appropriate level
    // Default Game Options that you can't change in the editor because they're static
    // should I have a non-static variable that the static ones take after so the designer can change it?
    public static int enemyNumber = 10;
    public static int startHp = 30;
    public static bool soloGame = false;
    public static string startLevel;
    public static int teamsNb; // number of different teams; 0 is chacun pour soi
    public static float chrono = 0; // en minutes
    public static int activeScene = 0;
    public static int chosenPaletteNr = 0; // the palette color theme chosen
    public static int nbScenes;
    public static int musicIndex = 0;
    public static Color[] curr6Colors; //the 6 colors available to the sheep to change into, aka those of the backgrounds materials
    public Material[] curr6Mats; // the background's materials
    [SerializeField]
    private Color[] colorsOfFullPalette; // all of the sixty colors to choose from in the menu // gonna be useful when I turn the menu all keyboard-friendly
    public Texture2D palette; // the palette (lots of squares in a rectangle shape) image in my project
    public Image paletteObj; // the palette (lotsa squares inna rectangle) object in my scene
    public Image chosenPalettePreview; // the preview of the chosen palette applied to a level

    public Image[] chooseEachColorButtons; // the small clouds that let you choose each color individually
    public Color[] colorsOfEachPresetPalette; // a list that contains all the presets palettes for the colours, as long as you take them by groups of 6

    [Header("All the texts buttons and stuff")]
    public Sprite[] lvlPreviews;
    public Sprite[] palettesPreviews; // the liste of preview images available
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
    public Text teamsNbText;
    public Text chosenPaletteText;
    public Text startLevelText;

    private Image lvlImg;
    private int foleyVolumeInt = 60;
    private int musicVolumeInt = 0;
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
        levelsComments = new Dictionary<int, string>() { // descriptions of each level
            { 1, "big, flat, wide areas" },
            { 2, "medium, flat, wide areas"},
            { 3, "medium, cliffs, wide areas" },
            { 4, "very small, cliffs, wide areas" },
            { 5, "medium, cliffs, narrow paths" },
            { 6, "medium, cliff, narrow paths" },
            { 7, "small, hills, paths" },
            { 8, "medium, hills, paths" },
            { 9, "medium, hills, wide areas" },
            { 10, "big, hills and cliffs, wide areas" },
            { 11, "small, flat, paths" },
            { 12, "medium, flat, patchwork" },
            { 13, "small, flat, paths" },
            { 14, "medium, flat, patchwork" },
            { 15, "medium, flat, wide areas" },
            { 16, "small, flat, narrow paths" } };

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
        if (PlayerPrefs.HasKey("musicVol"))
        {
            musicVolumeInt = PlayerPrefs.GetInt("musicVol");
        }
        if (PlayerPrefs.HasKey("foleyVol"))
        {
            foleyVolumeInt = PlayerPrefs.GetInt("foleyVol");
        }
        if (PlayerPrefs.HasKey("favePalette"))
        {
            chosenPaletteNr = PlayerPrefs.GetInt("favePalette");
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
        StartCoroutine(initCols()); // because if we don't wait they haven't initialized all they need to and it bugs it up

        //colors
        GetColorPalette(palette, 10, 6);
        for (int i = 0; i < colorsOfEachPresetPalette.Length; i++) // put the colors at full alpha (cause I fucked up and they aren't already)
        {
            colorsOfEachPresetPalette[i].a = 1;
        }


        curr6Colors = new Color[curr6Mats.Length]; //colors: the 6 colors available to the sheep to change into, aka those of the backgrounds materials
        nbScenes = SceneManager.sceneCountInBuildSettings;
        lvlImg = lvlText.transform.parent.GetComponent<Image>();
        lvlImg.sprite = lvlPreviews[activeScene];
        ChangeMusicVolumeAbsolute(musicVolumeInt);
        ChangeMusicVolumeAbsolute(foleyVolumeInt);
        //initializing the texts with the default values:
        enemyText.text = enemyNumber.ToString();
        hpText.text = (startHp / 2).ToString();
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
        teamsNbText.text = teamsNb.ToString();
        startLevelText.text = levelsComments[activeScene + 1];
        chosenPaletteText.text = chosenPaletteNr.ToString();
        chosenPalettePreview.sprite = palettesPreviews[chosenPaletteNr];


        if (soloGame)
        {
            soloGameText.text = "yes";
        }
        else
        {
            soloGameText.text = "no";
        }
    }

    IEnumerator initCols()
    {
        yield return new WaitForSeconds(.1f);
            for (int i = 0; i < 6; i++) {
                if (PlayerPrefs.HasKey("col" + (i + 1).ToString())) {
                    curr6Mats[i].color = StringToVector4(PlayerPrefs.GetString("col" + (i + 1).ToString()));
                    chooseEachColorButtons[i].color = curr6Mats[i].color;
                }
            }
        //if(!(PlayerPrefs.HasKey("col1") || PlayerPrefs.HasKey("col2") || PlayerPrefs.HasKey("col3") || PlayerPrefs.HasKey("col4") || PlayerPrefs.HasKey("col5") || PlayerPrefs.HasKey("col6") ))
        //{
        //    ChangePresetColNrAbsolute(presetColNr);
        //}
        RefreshColors();

    }

    public void RefreshColors()
    {
        for (int i = 0; i < curr6Mats.Length; i++)
        {
            curr6Colors[i] = curr6Mats[i].color;
        }
    }

    public void setMusic()
    {
        ColorManager.currentMusic = musics[musicIndex];
        PlayerPrefs.SetInt("faveMusic", musicIndex);
        ColorManager.ChangeColSounds = changeColSounds;
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
        startLevelText.text = levelsComments[activeScene + 1];
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
        PlayerPrefs.SetInt("musicVol", musicVolumeInt);
    }

    public void ChangeMusicVolumeAbsolute(int volume)
    {
        ChangeSettingAbsolute(volume, ref musicVolumeInt, musicVolumeText);
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
        PlayerPrefs.SetInt("FoleyVol", foleyVolumeInt);
    }

    public void ChangeFoleyVolumeAbsolute(int volume)
    {
        ChangeSettingAbsolute(volume, ref foleyVolumeInt, foleyVolumeText);
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

    public void ChangeNbrteams(int nb)
    {
        ChangeSetting(nb, ref teamsNb, teamsNbText, 0, 20);
    }

    public void ChangeStartHp(int nb)
    {
        ChangeSetting(nb * 2, ref startHp, hpText, 2, 500);
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
        ChangeSetting(nb, ref chosenPaletteNr, chosenPaletteText, 0, (colorsOfEachPresetPalette.Length / 6) - 1);
        for (int i = 0; i < chooseEachColorButtons.Length; i++)
        {
            chooseEachColorButtons[i].color = colorsOfEachPresetPalette[(chosenPaletteNr * 6) + i]; //j'ai six presetColsImg, qui sont les boutons pour changer chaque couleur individuellement; j'ai autant de colorPresets que 6 * les palettes que j'ai préparé, parce qu'il y a 6 couleurs par palette
            chooseEachColorButtons[i].GetComponent<ChangeMatColor>().ChangeMatCol();
            PlayerPrefs.SetString(chooseEachColorButtons[i].name.ToString(), Vector4ToString(chooseEachColorButtons[i].color));
        }
        chosenPalettePreview.sprite = palettesPreviews[chosenPaletteNr];
        PlayerPrefs.SetInt("favePalette", chosenPaletteNr);
    }

    public void ChangePresetColNrAbsolute(int nb)
    {
        ChangeSettingAbsolute(nb, ref chosenPaletteNr, chosenPaletteText);
        for (int i = 0; i < chooseEachColorButtons.Length; i++)
        {
            chooseEachColorButtons[i].color = colorsOfEachPresetPalette[(chosenPaletteNr * 6) + i]; //j'ai six presetColsImg, qui sont les boutons pour changer chaque couleur individuellement; j'ai autant de colorPresets que 6 * les palettes que j'ai préparé, parce qu'il y a 6 couleurs par palette
            chooseEachColorButtons[i].GetComponent<ChangeMatColor>().ChangeMatCol();
            PlayerPrefs.SetString(chooseEachColorButtons[i].name.ToString(), Vector4ToString(chooseEachColorButtons[i].color));
            print(chooseEachColorButtons[i].GetComponent<ChangeMatColor>().targetMat + "'s color is now " + chooseEachColorButtons[i].color + " because I'm using the preset number " + chosenPaletteNr);
        }
        chosenPalettePreview.sprite = palettesPreviews[chosenPaletteNr];
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
        if (!paletteObj.enabled) { // for if you misclick when you're choosing the colors, since the buttons are pretty close
            StartCoroutine(GetInteract());
        }
    }

    public IEnumerator GetInteract()
    {
        interactKeyTx.text = "press a key";
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
        if (!paletteObj.enabled)
        { // for if you misclick when you're choosing the colors, since the buttons are pretty close
            StartCoroutine(GetMenu());
        }
    }

    public IEnumerator GetMenu()
    {
        menuKeyTx.text = "press a key";
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
        if (!paletteObj.enabled)
        { // for if you misclick when you're choosing the colors, since the buttons are pretty close
            StartCoroutine(GetSelf());
        }
    }

    public IEnumerator GetSelf()
    {
        selfChangeKeyTx.text = "press a key";
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
        if (!paletteObj.enabled)
        { // for if you misclick when you're choosing the colors, since the buttons are pretty close
            StartCoroutine(GetFwd());
        }
    }

    public IEnumerator GetFwd()
    {
        forwardKeyTx.text = "press a key";
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
        if (!paletteObj.enabled)
        { // for if you misclick when you're choosing the colors, since the buttons are pretty close
            StartCoroutine(GetRight());
        }
    }

    public IEnumerator GetRight()
    {
        rightKeyTx.text = "press a key";
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
        if (!paletteObj.enabled)
        { // for if you misclick when you're choosing the colors, since the buttons are pretty close
            StartCoroutine(GetLeft());
        }
    }

    public IEnumerator GetLeft()
    {
        leftKeyTx.text = "press a key";
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
        Toggle(paletteObj);
        StartCoroutine(GetCol(img));
    }

    public IEnumerator GetCol(Image img) //pick a color
    {
        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        Color prevCol = img.color; 
        Color col = palette.GetPixel
            (Mathf.RoundToInt(
                (Input.mousePosition.x - paletteObj.rectTransform.position.x)
                * palette.width/ paletteObj.rectTransform.sizeDelta.x * 1920 / Screen.width),
                Mathf.RoundToInt(
                    (Input.mousePosition.y - paletteObj.rectTransform.position.y)
                    * palette.height / paletteObj.rectTransform.sizeDelta.y * 1920 / Screen.width // because the canvas scaler that holds the palette is 1920/1080, and scales fully with Width (not Height)
                    )
                    );

        img.color = col;
        img.GetComponent<ChangeMatColor>().ChangeMatCol();


        PlayerPrefs.SetString(img.name.ToString(), Vector4ToString(col));

        Toggle(paletteObj);
    }

    private void GetColorPalette(Texture2D tex, int nbStepsW, int nbStepsH)
    {
        colorsOfFullPalette = new Color[nbStepsH * nbStepsW];
        int stepW = palette.width / nbStepsW;
        int stepH = palette.height / nbStepsH;
        for (int i = 0; i < nbStepsW; i++)
        {
            for (int j = 0; j < nbStepsH; j++)
            {
                colorsOfFullPalette[(nbStepsH * i) + (j)] = palette.GetPixel(i * stepW, j * stepH);
            }
        }
    }

    public string Vector4ToString(Color v)
    {
        string result = v.r + "-" + v.g + "-" + v.b + "-" + 1;
        return result;
    }

    public Color StringToVector4(string s)
    {

        // split the items
        string[] sArray = s.Split('-');

        // store as a Color
        Color result = new Color(
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
        }
    }
}
