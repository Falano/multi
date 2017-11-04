using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// à mettre sur le sheep prefab

public class ScoreKeeper : NetworkBehaviour {
    public Score currentPlayer;

    public override void OnStartLocalPlayer()
    {
        if (!isLocalPlayer) { return; }
        bool done = false;

        Prototype.NetworkLobby.LobbyPlayer lobby = (Prototype.NetworkLobby.LobbyPlayer)GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Prototype.NetworkLobby.LobbyManager>().lobbyPlayerPrefab;
        currentPlayer = new Score(gameObject, lobby.playerName);
        for (int i = 0; done == true; i++){ // mais si je choisis une case vide, je la remplis et l'envoie au serveur, pour qu'il dise à tous les clients qu'elle est remplie, en même temps que je fais ça d'autres clients peuvent choisir la même; ou alors j'attends entre chaque (selon quel ordre?)
            if (ColorManager.playersList[i] != null) { return; }
            currentPlayer.SetI(i);
            ColorManager.playersList[i] = currentPlayer;
        }
    }
}
