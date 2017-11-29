using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class TutoBehaviour : PlayerBehaviour
{
    public static int nameIndex;

    void Start()
    {
        if (CompareTag("Player"))
        {
            TutoManager.singleton.localPlayer = gameObject;
            CameraMover.singleton.activePlayer = transform; // on dit à la camera que c'est lui ici le player à suivre
            if (PlayerPrefs.HasKey("playerName"))
            {
                localName = PlayerPrefs.GetString("playerName");
            }
        }
        else
        {
            localName = "NPS-" + nameIndex;
            nameIndex++;
        }

        ScoreObj = TutoManager.singleton.SpawnScore(localName, gameObject);
        name = "sheep-" + localName;
        CmdRefreshListOfPlayers();
    }

    [Command]
    new protected void CmdRefreshListOfPlayers()
    {
        TutoManager.singleton.RpcRefreshListOfPlayers();
    }

    IEnumerator waitToAssignScore()
    {
        yield return new WaitForSeconds(.1f);
        ScoreObj = TutoManager.singleton.SpawnScore(localName, gameObject);
        name = "sheep-" + localName;
        CmdRefreshListOfPlayers();

    }

    [Command]
    new public void CmdSetLocalName(string name, GameObject obj)
    {
        TutoManager.singleton.RpcSetLocalName(name, obj);
    }


    [Command]
    new public void CmdTogglePlayerReady(GameObject player, bool state)
    {
        TutoManager.singleton.RpcTogglePlayerReady(gameObject, state);
        TutoManager.singleton.RpcRefreshListOfPlayers();
    }

}
