using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// sur le mouton 
// keeps player's health value and has TakeDamage() function 
// also manages the GUI's healthbar 
// has KillSolo() (because isLocalPlayer doesn't work if it's on the gameManager)

public class TutoHealth : PlayerHealth
{


    void Start()
    {
        hp = MenuManager.startHp + 2;
        healthGUI = TutoManager.singleton.healthGUI;
        if (TutoManager.isInTuto)
        {
            tutoText = GetComponentInChildren<Text>();
        }
    }

    void Kill()
    {
        KillSolo();
    }

    new public void KillSolo()
    {
            TutoManager.singleton.tutoSpeech(TutoManager.singleton.speechDuration, "Nope, I'm out!", tutoText);

        TutoManager.singleton.numberOfPlayersPlaying--;

        Score score = GetComponent<TutoBehaviour>().ScoreObj.GetComponent<Score>();
        score.SetTimeOfDeath();
        print(score.playerName + "'s time of death: " + score.TimeOfDeath);

        if (isLocalPlayer)
        {
            TutoManager.singleton.isLocalPlayerDead = true;
        }
        GetComponent<PlayerMove>().speed = 0;
        GameObject mesh = transform.GetChild(0).gameObject;
        mesh.SetActive(false);
        GameObject death = transform.GetChild(2).gameObject;
        death.SetActive(true);
        death.GetComponent<SpriteRenderer>().color = mesh.GetComponent<Renderer>().material.color;

        GetComponent<BoxCollider>().enabled = false; //careful il y a deux box colliders, l'un trigger; ne pas changer leur place
                                                     //the object destroy itself is on a script on the child
    TutoManager.singleton.tutoSpeech(TutoManager.singleton.speechDuration, "Nope, I'm out!", tutoText);
    }
}
