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
    public Vector3 posOffset;

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
	/*
	// Update is called once per frame
	void Update () {
        if (activePlayer != null)
        {
            transform.position = activePlayer.position + posOffset;
        }
        else
        {
            activePlayer = listPlayers [x]...
        }
        if(Input.GetKeyDown(KeyCode.Y)){
         x+=1;
         activePlayer = listPlayers[x];
    }

    }
    */
}
