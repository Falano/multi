using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// get a list of all players
// follow the played player
// when they die, follow another

public class CameraMover : NetworkBehaviour {
    public static CameraMover singleton;
    public Transform activePlayer; //assigné au début du jeu dans PlayerMove
    public Transform posRotOffset;
    private int i = 0;
    private bool isPlayerDead = false;

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



    // Use this for initialization
    void Start () {

	}

	void Update () {
        if (!isLocalPlayer) { return; }
        if (activePlayer != null)
        {
            transform.position = activePlayer.position + posRotOffset.position;
            transform.rotation = posRotOffset.rotation;
        }
        else
        {
            isPlayerDead = true;
            while(activePlayer == null)
            {
                activePlayer = ColorManager.playersList[i].PlayerObj.transform;
                if(i >= ColorManager.playersList.Length)
                {
                    i = -1;
                }
                i++;
            }
        }
        if(Input.GetKeyDown(KeyCode.Space) && isPlayerDead){ // attention: implémenter: si le joueur local est mort, alors exécuter le code
         i++;
            print("space"); // clairement ça ne marche pas
         activePlayer = ColorManager.playersList[i].PlayerObj.transform;
        }
    }
}
