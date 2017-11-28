using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// sur le mouton 
// keeps player's health value and has TakeDamage() function 
// also manages the GUI's healthbar 
// has KillSolo() (because isLocalPlayer doesn't work if it's on the gameManager)

public class TutoHealth : PlayerHealth
{
    [SerializeField]

    new public void KillSolo()
    {
        base.KillSolo();
        ColorManager.singleton.tutoSpeech(TutoManager.singleton.speechDuration, "Nope, I'm out!", tutoText);
    }
}
