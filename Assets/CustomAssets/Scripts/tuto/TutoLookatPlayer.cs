﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoLookatPlayer : MonoBehaviour
{

    TutoChangeCol player;
    TutoChangeCol ccol;
    bool isAlly;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<TutoChangeCol>();
        ccol = GetComponent<TutoChangeCol>();
        ccol.sharing = true;
        RefreshAlly();
    }

    public void RefreshAlly()
    {
        isAlly = (player.team == ccol.team);
        ccol.StartHp = isAlly ? 50 : 10;
        ccol.reinitializeHp();
    }


    // Update is called once per frame
    void Update()
    {
        if(TutoManager.singleton.currState != TutoManager.gameState.playing)
        {
            return;
        }
        if (isAlly && player)
            transform.LookAt(player.transform);
    }
}
