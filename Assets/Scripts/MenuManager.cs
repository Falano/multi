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
    //

    public Text enemyText;
	public Text hpText;
	public Text chronoText;
	public Text lvlText;

    public void Start()
    {
        //nbScenes = SceneManager.sceneCount;
        nbScenes = 3;
    }


    public void Quit(){
		Application.Quit ();
	}

	public void LoadLevel(int lvl){
        activeScene = lvl;
        NetworkManagerPersistence.NetworkManager.ServerChangeScene(lvl.ToString());
        //SceneManager.LoadScene (activeScene);
	}

    public void LoadNextScene()
    {
        activeScene += 1;
        NetworkManagerPersistence.NetworkManager.gameObject.GetComponent<NetworkManagerHUD>().enabled = true;
        if (activeScene >= nbScenes)
        {
            activeScene = 1;
        }
        NetworkManagerPersistence.NetworkManager.ServerChangeScene(activeScene.ToString());
        //SceneManager.LoadScene(activeScene);
    }

    public void LoadRandomLevel(){
		int randomScene = Random.Range (1, scenes.Length);
        activeScene = randomScene;
		SceneManager.LoadScene (activeScene);
	}

    public void ChangeStartScene(int change) {
        activeScene = (activeScene+change+nbScenes)%(nbScenes-1)+1;
        print("activeScene = " + activeScene + ", change = "+change+", nbScenes = "+nbScenes+ "; \n(activeScene+change+nbScenes)%nbScenes = " + (activeScene+change+nbScenes)%nbScenes);
        lvlText.text = (activeScene).ToString();
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
