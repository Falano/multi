﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


// à mettre sur tous les moutons dans le tuto
// change la couleur des gens

public class TutoChangeCol : MonoBehaviour
{
    Renderer rd;
    GameObject deathAnim;
    Color[] colors;
    Color prevColor;
    AudioSource source;
    NavMeshAgent ag;
    Image healthGUI;
    [SerializeField]
    private Sprite[] sprites;
    int spritesIndex;
    public TextMesh speech;
    private TutoChangeCol PLchangeCol;

    int StartHp = 10;
    float hp = 10;

    void Start()
    {
        ag = GetComponent<NavMeshAgent>();
        colors = TutoManager.colors;
        rd = GetComponentInChildren<Renderer>();
        deathAnim = GetComponentInChildren<DieWhenFinishedAnim>(true).gameObject;
        prevColor = rd.materials[0].color;
        speech = GetComponentInChildren<TextMesh>();
        speech.text = "";
        PLchangeCol = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<TutoChangeCol>();
        if (CompareTag("NPS"))
        {
            ChangeCol(colors[0]);
            StartHp = 5;
        }
        hp = StartHp;
        healthGUI = TutoManager.singleton.healthGUI;
        TutoManager.singleton.speak("Press <b>Space</b> when you\'re ready. The game starts when every <i>logged in</i> player\nis ready (<i>even if</i> everyone you planned to play with hasn\'t logged in yet).\nThis tutorial is offline though,so don\'t worry about forgetting people,\n just remember: <b>Space</b> when you\'re ready.", TutoManager.singleton.textNarr, 50);
        if (CompareTag("Player"))
        {
            spritesIndex = (int)Mathf.Floor((hp / StartHp) * 10)-1;
            healthGUI.sprite = sprites[spritesIndex];
            TutoManager.singleton.speak("I should try pressing <b>Escape</b> several times...", speech, 10);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AttackChangeCol"))
        {
            ChangeCol(collision.gameObject);
        }
    }

    public void ChangeCol(GameObject attacker)
    {
        if (hp <= 1)
        {
            Kill();
            return;
        }

        source = GetComponent<AudioSource>();
        source.clip = TutoManager.singleton.ChangeColSounds[Random.Range(0, TutoManager.singleton.ChangeColSounds.Length)];
        source.Play();

        hp -= 1;
        if (CompareTag("Player"))
        {
            spritesIndex = (int)Mathf.Floor((hp / StartHp) * 10)-1;
            healthGUI.sprite = sprites[spritesIndex];
        }

        while (prevColor == rd.material.color)
        {
            prevColor = colors[Random.Range(0, colors.Length)];
        }

        rd.materials[0].color = prevColor;
        if (CompareTag("NPS"))
        {
            rd.materials[1].color = prevColor;
        }


        if(attacker == gameObject)
        {
            if (CompareTag("Player"))
            {
                TutoManager.singleton.speak("Oww, I better not do that too often", speech, 3);
                TutoManager.singleton.speak("see <i>the ball in the top-right corner</i>? \nThat's how manycolour changes you have left\nbefore you turn back into paint.", TutoManager.singleton.textNarr, 10);
            }
        }
        if (attacker.CompareTag("AttackChangeCol"))
        {
            if (CompareTag("Player"))
            {
                TutoManager.singleton.speak("I can run,\nand I can hide!", speech, 2);
                TutoManager.singleton.speak("It looks like changing colour produces adrenaline,\nwhether induced by mice or other sheep.", TutoManager.singleton.textNarr, 10);
            }
        }
        if (attacker.CompareTag("Player") && gameObject != attacker)
        {
            TutoManager.singleton.speak("So that's what I look like to others...", attacker.GetComponent<TutoChangeCol>().speech, 3);
        }
    }


    public void ChangeCol(Color color)
    {
        rd.materials[0].color = color;
        rd.materials[1].color = color;
    }


    void Kill()
    {
        StopAllCoroutines();
        speech.text = "Bye!";
        if (CompareTag("Player"))
        {
            GetComponent<TutoPLMove>().speed = 0;
            TutoManager.singleton.currState = TutoManager.gameState.deadPlayer;
        }
        else
        {
            ag.speed = 0;
            TutoManager.singleton.speak("I have been told there were both pros and cons\nto pressing the <b>Left Ctrl</b> key.\nIs doing it a good idea? Who knows.", PLchangeCol.speech, 10);
        }
        rd.gameObject.SetActive(false);
        deathAnim.SetActive(true);
        deathAnim.GetComponent<SpriteRenderer>().color = rd.GetComponent<Renderer>().material.color;
        GetComponent<BoxCollider>().enabled = false; //careful il y a deux box colliders, l'un trigger; ne pas changer leur place

    }
}