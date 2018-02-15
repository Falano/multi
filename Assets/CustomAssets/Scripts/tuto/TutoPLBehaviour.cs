using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// à ne mettre que sur le mouton du joueur dans le tuto

public class TutoPLBehaviour : MonoBehaviour
{
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
        if (TutoManager.singleton.currState != TutoManager.gameState.playing)
        {
            return;
        }
        // changing their own colour
        if (Input.GetKeyDown(MenuManager.selfChange))
        {
            changeCol.ChangeCol(gameObject);
            if (TutoManager.singleton.currTask == TutoManager.toDo.I_selfChange)
            {
                TutoManager.singleton.instructions("White-legged sheep are your allies.\n Try to press <b>" + MenuManager.interact + "</b> while looking at one.\n Try with one that isn't looking at you", TutoManager.toDo.J_unwillingTeam);
            }
        }

        Debug.DrawRay(transform.position + offsetPos, transform.forward * hitDistance, Color.green);
        Debug.DrawRay(transform.position + offsetPos, (transform.forward + transform.right / 6).normalized * hitDistance, Color.green);
        Debug.DrawRay(transform.position + offsetPos, (transform.forward + -transform.right / 6).normalized * hitDistance, Color.green);
        Debug.DrawRay(transform.position + offsetPos, (transform.forward + transform.up / 4).normalized * hitDistance, Color.green);
        Debug.DrawRay(transform.position + offsetPos, (transform.forward + -transform.up / 4).normalized * hitDistance, Color.green);
        // changing another's colour
        if ((Physics.Raycast(transform.position + offsetPos, transform.forward, out hit) ||
                    Physics.Raycast(transform.position + offsetPos, transform.forward + transform.right / 6, out hit) ||
                    Physics.Raycast(transform.position + offsetPos, transform.forward - transform.right / 6, out hit) ||
                    Physics.Raycast(transform.position + offsetPos, transform.forward + transform.up / 4, out hit) ||
                    Physics.Raycast(transform.position + offsetPos, transform.forward - transform.up / 4, out hit)
                    )
             && hit.transform.CompareTag("NPS"))
        {
            if (paintReady && Vector3.Distance(hit.transform.position, transform.position) <= hitDistance)
            {
                if (changeCol.speech.text == "")
                {
                    TutoManager.singleton.speak("?", changeCol.speech, 1);// I totes should bully my neighbour
                }
                if (Input.GetKeyDown(MenuManager.interact))
                {
                    hit.transform.gameObject.GetComponent<TutoChangeCol>().ChangeCol(gameObject);
                    StartCoroutine(paintCooldown(cooldown));
                }
            }
            else if (Input.GetKeyDown(MenuManager.interact)) // if you looked at the right sheep but from too far away or before being ready
            {
                TutoManager.singleton.speak(":/", changeCol.speech, 1);
                if (TutoManager.singleton.currTask == TutoManager.toDo.E_bully)
                    TutoManager.singleton.instructions("You weren't close enough to the sheep.\n Press <b>" + MenuManager.interact + "</b> when you're closer, looking at a colored sheep.", TutoManager.toDo.E_bully);
            }
        }
        else if (Input.GetKeyDown(MenuManager.interact)) //if you're looking at nothing?
        {
            TutoManager.singleton.speak("'\\(:/)/'", changeCol.speech, 1);
        }


        if (rd.materials[1].color != Color.black)
        {
            rd.materials[1].color = Color.black;
        }
    }
}
