using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieWhenFinishedAnim : MonoBehaviour {

    private Animation death;

	// Use this for initialization
	void Start () {
        death = GetComponent<Animation>();
	}
	
	// Update is called once per frame
	void Update () {
        if (death["death"].time >= death["death"].length) {
            Destroy(GetComponentInParent<GameObject>());

        }

    }
}
