﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerHealth : NetworkBehaviour {
	[SerializeField]
	float hp;
	ColorManager cm;
	public Image healthGUI;
	public Sprite[] sprites;
    [SerializeField]
	private int spritesIndex = 10;

    public int nummer;

	// Use this for initialization
	void Start () {
        Invoke("Initialize", 0.2f);
    }

    void Initialize()
    {
        hp = MenuManager.startHp;
        cm = GameObject.FindGameObjectWithTag("ColorManager").GetComponent<ColorManager>();
        healthGUI = GameObject.FindGameObjectWithTag("hGUI").GetComponent<Image>();
    }

	public void TakeDamage(int dmg = 1){
		hp -= dmg;
		if (hp <= 0) {
			cm.Kill (this.gameObject);
		}
		spritesIndex = (int) Mathf.Floor ((hp / MenuManager.startHp) * 10);
        if (isLocalPlayer) {
            healthGUI.sprite = sprites[spritesIndex];
        }
    }

	// Update is called once per frame
	void Update () {
	}
}
