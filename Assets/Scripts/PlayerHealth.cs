using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// sur le mouton
// keeps player's health value and has TakeDamage() function
// also manages the GUI's healthbar

public class PlayerHealth : NetworkBehaviour {
	[SerializeField]
	float hp;
	ColorManager cm;
	public Image healthGUI;
	public Sprite[] sprites;
    [SerializeField]
	private int spritesIndex = 10;

    public int nummer;

    public float Hp
    {
        get
        {
            return hp;
        }
    }

    // Use this for initialization
    void Start () {
		hp = MenuManager.startHp;
		cm = GameObject.FindGameObjectWithTag ("ColorManager").GetComponent<ColorManager> ();
		healthGUI = GameObject.FindGameObjectWithTag ("hGUI").GetComponent<Image> ();
    }

    // both for hp value and GUI's healthbar
	public void TakeDamage(int dmg = 1){
		hp -= dmg;
		if (Hp <= 0) {
			cm.Kill (this.gameObject);
		}
		spritesIndex = (int) Mathf.Floor ((Hp / MenuManager.startHp) * 10);
        if (isLocalPlayer) {
            healthGUI.sprite = sprites[spritesIndex];
        }
    }
}
