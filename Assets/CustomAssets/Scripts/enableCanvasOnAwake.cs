using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enableCanvasOnAwake : MonoBehaviour {

	void Awake () {
        GetComponent<Canvas>().enabled = true;
    }
}
