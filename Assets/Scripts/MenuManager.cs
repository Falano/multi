using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
	public Scene[] scenes;
	public static int enemyNumber = 3;
	public static int startHp = 100;
	public static int playersNumber;
	public static string startLevel;
	public static int teamwork; // number of different teams; 0 is chacun pour soi
	public static float chrono = 0; // en minutes
	//

	public Text enemyText;
	public Text hpText;
	public Text chronoText;

	public void Quit(){
		Application.Quit ();
	}

	public void LoadLevel(string lvl){
		SceneManager.LoadScene (lvl);
	}

	public void LoadRandomLevel(){
		int randomScene = Random.Range (1, scenes.Length);
		SceneManager.LoadScene (randomScene);
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
