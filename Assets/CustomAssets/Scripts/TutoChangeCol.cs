using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// à mettre sur le player
// triggers the change col 

public class TutoChangeCol : PlayerChangeCol
{
    TextMesh tutoText;

    new void Start()
    {
        base.Start();
        tutoText = GetComponentInChildren<TextMesh>();
        print("tutoText = " + tutoText.gameObject.name);
        if (isLocalPlayer)
        {
            TutoManager.tutoText = tutoText;
        }
    }

    new public void startWhite()
    {
        ChangeCol(gameObject, colors[0], TutoManager.singleton.gameObject);
    }


    [Command]
    void CmdChangeCol(GameObject obj, Color col, GameObject attacker)
    {
        TutoManager.singleton.RpcChangeCol(obj, col, attacker);
    }


    new void ChangeCol(GameObject obj, GameObject attacker)
    {
        if (!TutoManager.isGamePlaying)
        {
            return;
        }
        if (obj.GetComponent<TutoHealth>().Hp <= 0) // comme ça s'il est en train de jouer l'anim death, il ne remeurt pas.
        {
            return;
        }

        prevColor = currColor;
        // so it doesn't "change" to the same colour:
        while (prevColor == currColor)
        {
            currColor = colors[Random.Range(0, colors.Length)];
        }
        CmdChangeCol(obj, currColor, attacker);
    }

    // so I can choose to change to one specific colour
    void ChangeCol(GameObject obj, Color col, GameObject attacker)
    {
        if (obj.GetComponent<TutoHealth>().Hp <= 0) // comme ça s'il est en train de jouer l'anim death, il ne remeurt pas.
        {
            return;
        }
        CmdChangeCol(obj, col, attacker);
    }




    void Update()
    {
            // changing their own colour
            if (Input.GetKeyDown(KeyCode.LeftControl) && gameObject.CompareTag("Player"))
            {
                ChangeCol(gameObject, gameObject);
            }

            Debug.DrawRay(transform.position + offsetPos, transform.forward * 5/*hitDistance*/, Color.green);

            if (rd.materials[1].color != Color.black && CompareTag("Player"))
            {
                rd.materials[1].color = Color.black;
            }

            if (Physics.Raycast(transform.position + offsetPos, transform.forward*5, out hit) && hit.transform.CompareTag("tutoPlayerNPC") && gameObject.CompareTag("Player"))
            {
            tutoText.text = "What is it? Should I get closer?";
                if (Vector3.Distance(hit.transform.position, transform.position) <= hitDistance)
                {
                    TutoManager.singleton.tutoSpeech(TutoManager.singleton.speechDuration, "I wonder what would happen if I pressed MenuManager.InteractKey.ToString() right now...", tutoText);
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        ChangeCol(hit.transform.gameObject, gameObject);
                    }
                }
            }
            else if(tutoText.text == "What is it? Should I get closer?")
        {
            tutoText.text = "";
        }
    
    }
}
