﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// à mettre sur le color manager
// dit à tout le monde que le mouton a changé de couleur (rpc)
// contient la fonction finale Kill qui désactive le renderer du mouton et active death anim; the object destroy itself is on a script on the child

[RequireComponent(typeof(NetworkIdentity))] //everything unchecked
public class ColorManager : NetworkBehaviour
{
    public static Score[] playersList; // trouver un moyen de la synchroniser à travers le réseau
	int i;
    public Vector3 LvlSize;
    public static ColorManager singleton;

    void Awake()
    {
        if (singleton == null) {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }
            playersList = new Score[MenuManager.maxPlayersNumber];
    }

    [ClientRpc]
    public void RpcChangeCol(GameObject obj, Color col, GameObject attacker) {
		obj.GetComponent<PlayerHealth> ().TakeDamage();
        if (obj.GetComponent<PlayerHealth>().Hp > 0) { // pour que la flaque de peinture soit de la dernière couleur vue et pas d'une nouvelle couleur random (cf Kill() ci-dessous)
            Renderer rd = obj.GetComponentInChildren<Renderer>();
            foreach (Material mat in rd.materials)
            {
                mat.color = col;
            }
            
            if (attacker == obj)
            {
                obj.GetComponent<ScoreKeeper>().currentPlayer.colorChangesFromSelf += 1;
            }
            else if (attacker.CompareTag("AttackChangeCol"))
            {
                obj.GetComponent<ScoreKeeper>().currentPlayer.colorChangesFromMice += 1;
            }
            else if (attacker.CompareTag("Player"))
            {
                obj.GetComponent<ScoreKeeper>().currentPlayer.colorChangesFromOthers += 1;
                attacker.GetComponent<ScoreKeeper>().currentPlayer.colorChangesToOthers += 1;

            }
        }
		
	}
    
	public void Kill(GameObject obj){
        GameObject mesh = obj.transform.GetChild(0).gameObject;
        mesh.SetActive(false);
        GameObject death = obj.transform.GetChild(2).gameObject;
        death.SetActive(true);
        Score player = obj.GetComponent<ScoreKeeper>().currentPlayer;
        player.SetTimeOfDeath(); // pour le score
        print(player.PlayerName + " est mort après " + player.TimeOfDeath + "secondes." );
        print("You dissolved into paint after " + player.TimeOfDeath.ToString("F1") + " seconds. You changed colour " + player.colorChangesFromMice + " times because of mice, " + player.colorChangesFromOthers + " times because of other players, " + player.colorChangesFromSelf  + " times of your own volition, and you made other players change colour " + player.colorChangesToOthers + " times." );
        death.GetComponent<SpriteRenderer>().color = mesh.GetComponent<Renderer>().material.color;
        //print("deathcol = " + death.GetComponent<SpriteRenderer>().color);
        //print("meshcol = " + mesh.GetComponent<Renderer>().material.color);
        obj.GetComponent<BoxCollider>().enabled = false; //careful il y a deux box colliders, l'un trigger; ne pas changer leur place
        //the object destroy itself is on a script on the child
	}
}