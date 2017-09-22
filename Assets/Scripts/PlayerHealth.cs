using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerHealth : NetworkBehaviour {
	[SerializeField]
	int hp;
	ColorManager cm;

	// Use this for initialization
	void Start () {
		hp = Options.StartHp;
		cm = GameObject.FindGameObjectWithTag ("ColorManager").GetComponent<ColorManager> ();
	}

	public void TakeDamage(int dmg = 1){
		hp -= dmg;
		if (hp <= 0) {
			cm.Kill (this.gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
