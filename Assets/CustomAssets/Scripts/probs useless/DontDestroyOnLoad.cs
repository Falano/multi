using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// don't destroy on load (don't think this is ever used actually)

public class DontDestroyOnLoad : MonoBehaviour {

	void Start () {
		DontDestroyOnLoad (this);
	}
	
	void Update () {
		
	}
}
