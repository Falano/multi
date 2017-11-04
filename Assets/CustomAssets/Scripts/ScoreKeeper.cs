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
        //Prototype.NetworkLobby.LobbyPlayer lobby = (Prototype.NetworkLobby.LobbyPlayer)GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<Prototype.NetworkLobby.LobbyManager>().lobbyPlayerPrefab;
        currentPlayer = new Score(gameObject, "test"/*lobby.playerName*/);
        Invoke("playersListWithParameters", 2); //have invoke with parameters
    }

    void playersListWithParameters()
    {
        ColorManager.singleton.RpcUpdatePlayersList(currentPlayer);
    }
}
