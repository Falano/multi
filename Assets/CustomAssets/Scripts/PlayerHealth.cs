using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// sur le mouton 
// keeps player's health value and has TakeDamage() function 
// also manages the GUI's healthbar 
// has KillSolo() (because isLocalPlayer doesn't work if it's on the gameManager)

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField]
    float hp;
    public Image healthGUI;
    public Sprite[] sprites;
    private bool isAlive = true;
    [SerializeField]
    private int spritesIndex = 10;
    Text tutoText;

    public float Hp
    {
        get
        {
            return hp;
        }
    }

    // Use this for initialization
    void Start()
    {
        hp = MenuManager.startHp + 2;
        healthGUI = ColorManager.singleton.healthGUI;
        tutoText = transform.parent.GetComponentInChildren<Text>();

    }

    // both for hp value and GUI's healthbar 
    public void TakeDamage(int dmg = 1)
    {
        if (!isAlive)
        {
            return;
        }
        hp -= dmg;
        //print("hp = " + hp);
        if (Hp <= 0)
        {
            print("dead");
            isAlive = false;
            Kill();
        }
        spritesIndex = (int)Mathf.Floor((Hp / MenuManager.startHp) * 10);
        if (isLocalPlayer)
        {
            healthGUI.sprite = sprites[spritesIndex]; // (si healthGUI est mal défini line 34-38 dans le Start():) ça. ça fait tout foirer. C'est la racine du mal. C'est à cause de lui que (ALORS QUE JE N'AI PAS DE WHILE NI DE FOR NI RIEN QUI EVOQUE UNE BOUCLE INFINIE) au deuxième ChangeCOl il s'emballe et re-TakeDamage() à l'infini
        }
    }

    void Kill() {
        if (isLocalPlayer)
        {
            CmdKill(gameObject);
        }
    }
    [Command]
    void CmdKill(GameObject obj) {
        ColorManager.singleton.RpcKill(obj); }
        
    public void KillSolo()
    {
        ColorManager.singleton.tutoSpeech(ColorManager.singleton.speechDuration, "Nope, I'm out!", tutoText);

        ColorManager.singleton.numberOfPlayersPlaying--;

        Score score = GetComponent<PlayerBehaviour>().ScoreObj.GetComponent<Score>();
        score.SetTimeOfDeath();
        print(score.playerName + "'s time of death: " + score.TimeOfDeath);
        
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
