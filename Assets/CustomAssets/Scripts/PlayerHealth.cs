using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// sur le mouton 
// keeps player's health value and has TakeDamage() function 
// also manages the GUI's healthbar 
// has KillSolo() (because isLocalPlayer doesn't work if it's on the gameManager)

public class PlayerHealth : NetworkBehaviour {
    [SerializeField]
    float hp;
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
        hp = MenuManager.startHp+1;
        healthGUI = GameObject.FindGameObjectWithTag("hGUI").GetComponent<Image>();
    }

    // both for hp value and GUI's healthbar 
    public void TakeDamage(int dmg = 1){
		hp -= dmg;
        print("hp = " + hp);
        if (Hp <= 0)
        {
            print("dead");
            ColorManager.singleton.Kill (this.gameObject);
		}
        spritesIndex = (int)Mathf.Floor((Hp / MenuManager.startHp) * 10);
        if (isLocalPlayer) {
            healthGUI.sprite = sprites[spritesIndex];
        }
    }

    public void KillSolo()
    {
        if (isLocalPlayer)
        {
            ColorManager.singleton.isLocalPlayerDead = true;
        }
        GetComponent<PlayerMove>().speed = 0;
        GameObject mesh = transform.GetChild(0).gameObject;
        mesh.SetActive(false);
        GameObject death = transform.GetChild(2).gameObject;
        death.SetActive(true);
        //        Score player = obj.GetComponent<ScoreKeeper>().currentPlayer;
        //        player.SetTimeOfDeath(); // pour le score
        //        CameraMover.singleton.activePlayer = null; // pour si la caméra ne comprend pas qu'il est mort
        //        print(player.PlayerName + " est mort après " + player.TimeOfDeath + "secondes." );
        //        print("You dissolved into paint after " + player.TimeOfDeath.ToString("F1") + " seconds. You changed colour " + player.colorChangesFromMice + " times because of mice, " + player.colorChangesFromOthers + " times because of other players, " + player.colorChangesFromSelf  + " times of your own volition, and you made other players change colour " + player.colorChangesToOthers + " times." );
        death.GetComponent<SpriteRenderer>().color = mesh.GetComponent<Renderer>().material.color;
        //print("deathcol = " + death.GetComponent<SpriteRenderer>().color);
        //print("meshcol = " + mesh.GetComponent<Renderer>().material.color);
        GetComponent<BoxCollider>().enabled = false; //careful il y a deux box colliders, l'un trigger; ne pas changer leur place
                                                         //the object destroy itself is on a script on the child
    }
}
