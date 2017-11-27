using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutoMiceColl : MonoBehaviour {

    Text PNJText;

	// Use this for initialization
	void Start () {
            PNJText = transform.parent.GetComponentInChildren<Text>();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AttackChangeCol"))
        {
            ColorManager.singleton.tutoSpeech(ColorManager.singleton.speechDuration, "NOOOO GET IT OFF GET IT OFF TAKE IT AWAY I HATE MICE NO RUN AWAY!!!", PNJText);
        }
    }


    // Update is called once per frame
    void Update () {
		
	}
}
