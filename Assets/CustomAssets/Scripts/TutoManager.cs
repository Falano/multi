using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoManager : MonoBehaviour {
    public static TutoManager singleton;
    public enum gameState {lobby, playing, scoreScreen};
    public gameState currState;
    public TextMesh textNarr;

    public AudioClip[] ChangeColSounds;

    [SerializeField]
    private Material[] colorsMats;
    public static Color[] colors;

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

        colors = new Color[colorsMats.Length]; //I need this in a Start
        for (int i = 0; i < colorsMats.Length; i++)
        {
            colors[i] = colorsMats[i].color;
        }
    }

    // Use this for initialization
    void Start () {
        currState = gameState.playing;
        textNarr = GameObject.FindGameObjectWithTag("MainCamera").GetComponentInChildren<TextMesh>();

       

        speak("This is a test\nOh a test\nso sad\nso scary.", textNarr, 5);
	}
	

    /*
    public void ChangeCol(GameObject obj, GameObject attacker)
    {
        TutoChangeCol ColChanger = obj.GetComponent<TutoChangeCol>();
        ColChanger.ChangeCol(attacker);
        

    }
    */




        public void speak(string sentence, TextMesh texte, float duration)
    {
        texte.text = sentence;
        StartCoroutine(finishSpeaking(sentence, texte, duration));
    }

    IEnumerator finishSpeaking (string sentence, TextMesh texte, float duration)
    {
        yield return new WaitForSeconds(duration);
        if(texte.text == sentence)
        {
            texte.text = "";
        }
    }


	// Update is called once per frame
	void Update () {
		
	}
}
