﻿using System.Collections;
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

    void Update()
    {
        if (activePlayer != null)
        {
            transform.position = activePlayer.position + posRotOffset.position;
            transform.rotation = posRotOffset.rotation;
        }

        // for following not-dead players when you died
        //if (Input.GetKeyDown(KeyCode.Space) && ColorManager.singleton.isPlayerDead)
        //{
        //    print("i = " + i);
        //    print("player list length" + ColorManager.listPlayers.Length);
        //    print("is local player dead? " + ColorManager.singleton.isPlayerDead);
        //    print("space"); // clairement ça ne marche pas
        //    while (!(ColorManager.listPlayers[i] != null &&
        //        ColorManager.listPlayers[i].PlayerObj != null &&
        //        ColorManager.listPlayers[i].TimeOfDeath != 0))  //ça. ça bugge. régler ça en commentant des petits bouts de la condition jusqu'à ce que ça marche
        //    {

        //        /*
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



        // VERSION DE FLO

        if (Input.GetKeyDown(KeyCode.Space) && ColorManager.singleton.isPlayerDead)
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
        }
    }
}
