using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// get a list of all players somehow
// follow the played player
// when they die, follow another

public class CameraMover : NetworkBehaviour {
    public static CameraMover singleton;
    public Transform activePlayer; //assigné au début du jeu dans PlayerMove
    public Transform posRotOffset;
    private int i = 0;

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

	void Update () {
        if (activePlayer != null)
        {
            transform.position = activePlayer.position + posRotOffset.position;
            transform.rotation = posRotOffset.rotation;
        }
        /*
        // for following not-dead players; currently purging scores, so commenting it.
        if(Input.GetKeyDown(KeyCode.Space) && ColorManager.singleton.isPlayerDead){ // attention: implémenter: si le joueur local est mort, alors exécuter le code
            print("space"); // clairement ça ne marche pas
            while (activePlayer == null)
            {
                activePlayer = ColorManager.playersList[i].PlayerObj.transform;
                if (i >= ColorManager.playersList.Length)
                {
                    i = -1;
                }
                i++;
            }
            activePlayer = ColorManager.playersList[i].PlayerObj.transform;
        }
        */
    }
}
