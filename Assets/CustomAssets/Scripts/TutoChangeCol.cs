using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// à mettre sur tous les moutons dans le tuto
// change la couleur des gens

public class TutoChangeCol : MonoBehaviour
{
    Renderer rd;
    GameObject deathAnim;
    Color[] colors;
    Color prevColor;
    AudioSource source;

    void Start()
    {
        colors = TutoManager.colors;
        rd = GetComponentInChildren<Renderer>();
        deathAnim = GetComponentInChildren<DieWhenFinishedAnim>(true).gameObject;
        prevColor = rd.materials[0].color;
        if (CompareTag("NPS"))
        {
            ChangeCol(gameObject);
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
        source = GetComponent<AudioSource>();
        source.clip = TutoManager.singleton.ChangeColSounds[Random.Range(0, TutoManager.singleton.ChangeColSounds.Length)];
        source.Play();

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

        }
        if (attacker.CompareTag("AttackChangeCol"))
        {

        }
        if (attacker.CompareTag("Player"))
        {

        }
    }
}
