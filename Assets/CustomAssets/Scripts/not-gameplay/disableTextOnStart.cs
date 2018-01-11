using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// à mettre sur les couches du menu que je ne veux pas avoir au lancement du jeu
// au cas où j'aurais été trifouiller des trucs dans le menu de départ et aurais oublié de les remettre comme il faut

public class disableTextOnStart : MonoBehaviour {

	void Start () {
        GetComponent<Text>().enabled = false;
	}
}
