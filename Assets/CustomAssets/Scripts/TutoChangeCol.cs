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


    [Command]
    void CmdChangeCol(GameObject obj, Color col, GameObject attacker)
    {
        TutoManager.singleton.RpcChangeCol(obj, col, attacker);
    }



    void Update()
    {
            // changing their own colour
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ChangeCol(gameObject, gameObject);
            }

            Debug.DrawRay(transform.position + offsetPos, transform.forward * hitDistance, Color.green);

            if (rd.materials[1].color != Color.black)
            {
                rd.materials[1].color = Color.black;
            }

            if (Physics.Raycast(transform.position + offsetPos, transform.forward, out hit) && hit.transform.CompareTag("tutoPlayerNPC"))
            {
                //ColorManager.singleton.tutoSpeech(ColorManager.singleton.speechDuration, "What is it? Should I get closer?", tutoText);
                if (Vector3.Distance(hit.transform.position, transform.position) <= hitDistance)
                {
                    TutoManager.singleton.tutoSpeech(TutoManager.singleton.speechDuration, "I wonder what would happen if I pressed MenuManager.InteractKey.ToString() right now...", tutoText);
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        ChangeCol(hit.transform.gameObject, gameObject);
                    }
                }
            }
    
    }
}
