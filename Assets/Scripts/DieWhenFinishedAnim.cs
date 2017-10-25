using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieWhenFinishedAnim : MonoBehaviour
{

    private Animation death;
    public bool playAnim;

    private void OnEnable()
    {
        death = GetComponent<Animation>();
        death.Play();
        IEnumerator wait = waitForEndAnim(2);
        StartCoroutine(wait);
    }

    IEnumerator waitForEndAnim(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(GetComponentInParent<GameObject>());
    }

    void playDeath() {
        death = GetComponent<Animation>();
        death.Play("sheepDeath");
        IEnumerator wait = waitForEndAnim(2);
        StartCoroutine(wait);
    }
    /*
    // Update is called once per frame
    void Update()
    {
        //if (death["deathSheep"].time >= death["deathSheep"].length)
        //{
        //    Destroy(GetComponentInParent<GameObject>());
        //}
        if (playAnim)
        {
            playDeath();        }
    }*/
}
