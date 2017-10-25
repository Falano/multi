using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class disableCanvasOnStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Canvas>().enabled = false;
	}
}
