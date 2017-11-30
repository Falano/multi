using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


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

    int StartHp = 10;
    int hp;

    void Start()
    {
        ag = GetComponent<NavMeshAgent>();
        colors = TutoManager.colors;
        rd = GetComponentInChildren<Renderer>();
        deathAnim = GetComponentInChildren<DieWhenFinishedAnim>(true).gameObject;
        prevColor = rd.materials[0].color;
        if (CompareTag("NPS"))
        {
            ChangeCol(gameObject);
        }
        hp = StartHp;
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
        if (hp <= 0)
        {
            Kill();
            return;
        }
        if(TutoManager.singleton.currState == TutoManager.gameState.playing) {
        

        source = GetComponent<AudioSource>();
        source.clip = TutoManager.singleton.ChangeColSounds[Random.Range(0, TutoManager.singleton.ChangeColSounds.Length)];
        source.Play();

        hp -= 1;
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

            }
        }
        if (attacker.CompareTag("AttackChangeCol"))
        {

        }
        if (attacker.CompareTag("Player"))
        {

        }
    }
    void Kill()
    {
        if (CompareTag("Player"))
        {
            GetComponent<TutoPLMove>().speed = 0;
        }
        else
        {
            ag.speed = 0;
        }
        rd.gameObject.SetActive(false);
        deathAnim.SetActive(true);
        deathAnim.GetComponent<SpriteRenderer>().color = rd.GetComponent<Renderer>().material.color;
        GetComponent<BoxCollider>().enabled = false; //careful il y a deux box colliders, l'un trigger; ne pas changer leur place

    }
}