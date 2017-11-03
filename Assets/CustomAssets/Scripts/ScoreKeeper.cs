using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// à mettre sur le sheep prefab

public class ScoreKeeper : NetworkBehaviour {
    public Score currentPlayer;
    
    public override void OnStartLocalPlayer()
    {
        
        Prototype.NetworkLobby.LobbyPlayer lobby = (Prototype.NetworkLobby.LobbyPlayer)GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Prototype.NetworkLobby.LobbyManager>().lobbyPlayerPrefab;
        currentPlayer = new Score(gameObject, lobby.playerName);
        ColorManager.playersList.Add(currentPlayer);
        currentPlayer.SetI(ColorManager.playersList.IndexOf(currentPlayer));
    }

}
