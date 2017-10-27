using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;

public class MenuManager : MonoBehaviour {
	public Scene[] scenes;
	public static int enemyNumber = 3;
	public static int startHp = 30;
	public static int playersNumber;
	public static string startLevel;
	public static int teamwork; // number of different teams; 0 is chacun pour soi
	public static float chrono = 0; // en minutes
    [SerializeField]
    public static int activeScene = 0;
    public static int nbScenes;

    public Sprite[] lvlPreviews;
    public Text enemyText;
	public Text hpText;
	public Text chronoText;
	public Text lvlText;
    private Image lvlImg;

    public void Start()
    {
        nbScenes = SceneManager.sceneCountInBuildSettings;
        lvlImg = lvlText.transform.parent.GetComponent<Image>();
        lvlImg.sprite = lvlPreviews[activeScene];
        }


    public void Quit(){
		Application.Quit ();
	}

	public void LoadLevel(int lvl){
        activeScene = lvl;
        NetworkManager.singleton.ServerChangeScene(lvl.ToString());
        //SceneManager.LoadScene (activeScene);
	}

    public void LoadNextScene()
    {
        activeScene += 1;
        NetworkManager.singleton.GetComponent<NetworkManagerHUD>().enabled = true;
        if (activeScene >= nbScenes)
        {
            activeScene = 1;
        }
        //NetworkManager.networkSceneName = activeScene.ToString();
        NetworkManager.singleton.ServerChangeScene(activeScene.ToString());
        //SceneManager.LoadScene(activeScene);
    }

    public void LoadRandomLevel(){
		int randomScene = Random.Range (1, scenes.Length);
        activeScene = randomScene;
		SceneManager.LoadScene (activeScene);
	}

    public void ChangeStartScene(int change) {
        activeScene = (activeScene+change+(nbScenes-1))%(nbScenes-1); //parce qu'il ne faut pas tomber sur le menu
        print("activeScene = " + activeScene + ", change = "+change+", nbScenes = "+nbScenes+ "; \n(activeScene+change+nbScenes)%nbScenes = " + (activeScene+change+nbScenes)%nbScenes);
        lvlText.text = (activeScene+1).ToString();
        NetworkManager.singleton.onlineScene = (activeScene + 1).ToString();
        lvlImg.sprite = lvlPreviews[activeScene];
    }

	public void ChangeNbrEnemies(int nb){
		enemyNumber += nb;
		enemyText.text = enemyNumber.ToString();
	}

	public void ChangeStartHp(int nb){
		startHp += nb;
		hpText.text = startHp.ToString();
	}

	public void ChangeChrono(float nb){
		chrono += nb;
		chronoText.text = chrono.ToString("F1");        
    }

}
