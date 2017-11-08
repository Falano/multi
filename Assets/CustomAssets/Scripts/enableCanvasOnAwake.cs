using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableCanvasOnAwake : MonoBehaviour {

	// Use this for initialization
	void Awake () {
        GetComponent<Canvas>().enabled = true;
	}
}
