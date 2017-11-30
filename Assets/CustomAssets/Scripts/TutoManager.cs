using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoManager : MonoBehaviour {
    public static TutoManager singleton;
    public enum gameState {lobby, playing, scoreScreen};
    public gameState currState;


    public AudioClip[] ChangeColSounds;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }
    }

    // Use this for initialization
    void Start () {
        currState = gameState.lobby;	
	}
	


    public void ChangeCol(GameObject obj, GameObject attacker)
    {
        AudioSource source = obj.GetComponent<AudioSource>();
        source.clip = ChangeColSounds[Random.Range(0, ChangeColSounds.Length)];
        source.Play();

        TutoChangeCol.ChangeCol();
        


    }



	// Update is called once per frame
	void Update () {
		
	}
}
