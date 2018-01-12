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
    private PlayerBehaviour behaviour;

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
        behaviour = gameObject.GetComponent<PlayerBehaviour>();
        if(ColorManager.singleton.CurrState != ColorManager.gameState.lobby)
        {
            isAlive = false;
            Kill();
        }
    }

    // both for hp value and GUI's healthbar 
    public void TakeDamage(int dmg = 1)
    {
        print("damage");
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
        spritesIndex = (int)Mathf.Floor((Hp / MenuManager.startHp) * sprites.Length);
        if (isLocalPlayer)
        {
            print("spritesIndex: " + spritesIndex);
            healthGUI.sprite = sprites[spritesIndex];
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

        bool SeveralTeamsPlaying = false;
        bool firstLivePlayerFound = true;
        int prevTeam = 0;
        foreach (Score sco in ColorManager.singleton.Scores)
        {
            if (sco.PlayerObj != null && sco.PlayerObj != gameObject)
            {
                if (firstLivePlayerFound)
                {
                    prevTeam = sco.behaviour.team;
                    firstLivePlayerFound = false;
                }
                if (prevTeam != sco.behaviour.team)
                {
                    SeveralTeamsPlaying = true;
                }
                prevTeam = sco.behaviour.team;
            }
        }
        ColorManager.singleton.SeveralTeamsPlaying = SeveralTeamsPlaying;


        Score score = behaviour.ScoreObj;
        //score.playerName = GetComponent<PlayerBehaviour>().localName;//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////:::::::::::::::
        score.SetTimeOfDeath();
        
        if (isLocalPlayer)
        {
            ColorManager.singleton.isLocalPlayerDead = true;
        }
        GetComponent<PlayerMove>().speed = 0;
        GameObject mesh = transform.GetChild(0).gameObject;
        mesh.SetActive(false);
        GameObject death = transform.GetChild(2).gameObject;
        death.SetActive(true);
        death.GetComponent<SpriteRenderer>().color = mesh.GetComponent<Renderer>().material.color;
        GetComponent<BoxCollider>().enabled = false; //careful il y a deux box colliders, l'un trigger; ne pas changer leur place
                                                     //the object destroy itself is on a script on the child
    }
}
