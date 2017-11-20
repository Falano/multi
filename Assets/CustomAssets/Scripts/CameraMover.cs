using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// get a list of all players somehow
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
        if (Input.GetKeyDown(KeyCode.Space) && ColorManager.singleton.isLocalPlayerDead)
        {
            i += 1;
            if (i >= ColorManager.singleton.Scores.Length)
            {
                i = 0;
            }
            print("i = " + i);
            print("player list length" + ColorManager.singleton.Scores.Length);
            print("is local player dead? " + ColorManager.singleton.isLocalPlayerDead);
            print("space");
            while (ColorManager.singleton.Scores[i].PlayerObj == null)
            {
                i++;
                if (i >= ColorManager.singleton.Scores.Length)
                {
                    i = 0;
                }
            }
            activePlayer = ColorManager.singleton.Scores[i].PlayerObj.transform;

            //        if (ColorManager.listPlayers[i]!= null && ColorManager.listPlayers[i].PlayerObj != null)
            //        {
            //            print("trying to attach camera to " + ColorManager.listPlayers[i].PlayerName);
            //            activePlayer = ColorManager.listPlayers[i].PlayerObj.transform;
            //        }*/
            //        if (i >= ColorManager.listPlayers.Length)
            //        {
            //            i = -1;
            //        }
            //        i++;
            //    }
            //    print("i = " + i + "; trying to attach camera to " + ColorManager.listPlayers[i].PlayerName);
            //    activePlayer = ColorManager.listPlayers[i].PlayerObj.transform;
            //}


            /*
            // VERSION DE FLO

            if (Input.GetKeyDown(KeyCode.Space) && ColorManager.singleton.isLocalPlayerDead)
            {
                Score playerToFollow;
                bool searchingForPlayer = true;
                while (searchingForPlayer)
                {
                    i++;
                    if (i >= ColorManager.listPlayers.Length)
                    {
                        i = 0;
                    }

                    playerToFollow = ColorManager.listPlayers[i];

                    if (playerToFollow != null && playerToFollow.Alive)  // XXX here I use the bool Alive created in Score.cs
                    {
                        searchingForPlayer = false;
                        activePlayer = playerToFollow.transform;
                        // here add text about who you are spectating
                    }

                }
            }*/
        }
    }
}
