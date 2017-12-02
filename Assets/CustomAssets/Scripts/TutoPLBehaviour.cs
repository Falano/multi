using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoPLBehaviour : MonoBehaviour {
    RaycastHit hit;
    public float hitDistance = 1.5f;
    Vector3 offsetPos;
    Renderer rd;
    TutoChangeCol changeCol;

    public bool paintReady = true;
    public float cooldown = .5f;
    // mettre un son pour quand tu as essayé de changer la couleur de quelqu'un et tu as mal visé
    // mettre un son pour quand tu as essayé de changer la couleur de quelqu'un et le cooldown était encore en cours (un petit po-pow)

    // Use this for initialization
    void Start()
    {
        rd = GetComponentInChildren<Renderer>();
        offsetPos = new Vector3(0, .5f, 0);
        changeCol = GetComponent<TutoChangeCol>();
        TutoCameraMover.singleton.activePlayer = transform;
    }



    IEnumerator paintCooldown(float cooldown)
    {
        paintReady = false;
        yield return new WaitForSeconds(cooldown);
        paintReady = true;
    }



    void Update()
    {
        if(TutoManager.singleton.currState != TutoManager.gameState.playing)
        {
            return;
        }
        // changing their own colour
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            changeCol.ChangeCol(gameObject);
        }

        Debug.DrawRay(transform.position + offsetPos, transform.forward * hitDistance, Color.green);
        // changing another's colour
        if (Physics.Raycast(transform.position + offsetPos, transform.forward * 5, out hit) && hit.transform.CompareTag("NPS"))
        {
            if (Vector3.Distance(hit.transform.position, transform.position) >= hitDistance && changeCol.speech.text == "")
            {
                TutoManager.singleton.speak("Is there something there?\nI can't see; let's get closer", changeCol.speech, .5f); // should I get closer?
            }
            if (paintReady && Vector3.Distance(hit.transform.position, transform.position) <= hitDistance)
            {
                if (changeCol.speech.text == "" || changeCol.speech.text == "Is there something there?\nI can't see; let's get closer")
                {
                    TutoManager.singleton.speak("I wonder what would happen\nif I pressed the <b>Space</b> key\nright now", changeCol.speech, 1);// I totes should bully my neighbour
                }
                if (Input.GetKeyDown(KeyCode.Space) && paintReady == true)
                {
                    hit.transform.gameObject.GetComponent<TutoChangeCol>().ChangeCol(gameObject);
                    StartCoroutine(paintCooldown(cooldown));
                }
            }
        }

        if (rd.materials[1].color != Color.black)
        {
            rd.materials[1].color = Color.black;
        }
    }
}
