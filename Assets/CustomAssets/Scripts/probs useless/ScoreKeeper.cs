using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// à mettre sur le sheep prefab

    /*
public class ScoreKeeper : NetworkBehaviour {
    public Score currentPlayer;

    public override void OnStartLocalPlayer()
    {
        if (!isLocalPlayer) { return; }
        string playerName = PlayerPrefs.GetString(name);
        currentPlayer = new Score(gameObject, playerName); // need somewhere to ask it of the player
        CmdUpdatePlayerList(gameObject);
    }

    [Command]
    public void CmdUpdatePlayerList(GameObject obj)
    {
        Score player = obj.GetComponent<ScoreKeeper>().currentPlayer;
        bool done = false;
        for (int i = 0; done == false; i++)
        {
            if (ColorManager.playersList[i] == null)
            {
                player.SetI(i);
                ColorManager.playersList[i] = player;
                done = true;
            }
        }

        ColorManager.singleton.RpcUpdatePlayersList(obj);

    }
}
*/