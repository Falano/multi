using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// follow the played player
// when they die, follow another

public class TutoCameraMover : MonoBehaviour
{
    public static TutoCameraMover singleton;
    public Transform activePlayer; //assigné au début du jeu dans PlayerMove
    public Transform posRotOffset;
    private int i = -1;

    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }
    }

    void Update()
    {
        if (activePlayer != null)
        {
            transform.position = activePlayer.position + posRotOffset.position;
            transform.rotation = posRotOffset.rotation;
        }

        // for following not-dead players when you died
        if (Input.GetKeyDown(MenuManager.interact) && TutoManager.singleton.currState == TutoManager.gameState.deadPlayer)
        {
            if (TutoManager.singleton.currTask == TutoManager.toDo.M_stalker)
                TutoManager.singleton.instructions("Press <b>" + MenuManager.interact + "</b> to see the others.\n Again. Again.", TutoManager.toDo.M_stalker2);
            else if (TutoManager.singleton.currTask == TutoManager.toDo.M_stalker2)
                TutoManager.singleton.instructions("The game ends when only one team is left corporeal.\n In this tutorial, you can end it by pressing <b>" + MenuManager.selfChange + "</b> twice.", TutoManager.toDo.N_showScores);
            i += 1;
            if (i >= TutoManager.singleton.NPSs.Length)
            {
                i = 0;
            }
            while (TutoManager.singleton.NPSs[i] == null)
            {
                i++;
                if (i >= TutoManager.singleton.NPSs.Length)
                {
                    i = 0;
                }
            }
            activePlayer = TutoManager.singleton.NPSs[i].transform;
            TutoManager.singleton.following.text = "following " + activePlayer.GetComponent<TutoStats>().localName;
        }
    }
}
