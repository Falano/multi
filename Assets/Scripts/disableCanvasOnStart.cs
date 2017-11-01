using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// à mettre sur les couches du menu que je ne veux pas avoir au lancement du jeu
// au cas où j'aurais été trifouiller des trucs dans le menu de départ et aurais oublié de les remettre comme il faut

public class disableCanvasOnStart : MonoBehaviour {

	void Start () {
        GetComponent<Canvas>().enabled = false;
	}
}
