using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// follow the played player
// when they die, follow another

public class CameraMover : NetworkBehaviour
{
    public static CameraMover singleton;
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
        if (Input.GetKeyDown(MenuManager.interact) && ColorManager.singleton.isLocalPlayerDead && ColorManager.singleton.numberOfPlayersPlaying>0)
        {
            i += 1;
            if (i >= ColorManager.singleton.Scores.Length)
            {
                i = 0;
            }
            while (ColorManager.singleton.Scores[i].PlayerObj == null)
            {
                i++;
                if (i >= ColorManager.singleton.Scores.Length)
                {
                    i = 0;
                }
            }
            activePlayer = ColorManager.singleton.Scores[i].PlayerObj.transform;
            ColorManager.singleton.following.text = "following " + activePlayer.GetComponent<PlayerBehaviour>().localName;
        }
    }
}
