using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoAssignScores : MonoBehaviour {

    public Score[] scores;
    public GameObject scoreParent;
    public TutoChangeCol[] sheep;

	// Use this for initialization
	void Awake () {
    GameObject currScoreObj;
    Score currScore;

    GameObject[] sheepGO = GameObject.FindGameObjectsWithTag("NPS");
        sheep = new TutoChangeCol[sheepGO.Length + 1];
        for (int i = 0; i < sheepGO.Length; i++)// ne ho bisogno in un start
        {
            sheep[i] = sheepGO[i].GetComponent<TutoChangeCol>();
        }
        sheep[sheepGO.Length] = GameObject.FindGameObjectWithTag("Player").GetComponent<TutoChangeCol>();
        scores = new Score[sheep.Length];
		for(int i = 0; i<sheep.Length; i++)
        {
            currScoreObj = new GameObject("score-" + sheep[i].name, typeof(Score));
            currScoreObj.transform.SetParent(scoreParent.transform);
            currScore = currScoreObj.GetComponent<Score>();
            currScore.SetPlayerObjTuto(sheep[i].gameObject, sheep[i].gameObject.GetComponent<TutoChangeCol>().localName, sheep[i].team);
            sheep[i].score = currScore;
            scores[i] = currScore;
            currScore.colorChangesFromSelf += 1;
        }
	}
	
	// Update is called once per frame
	void Update () {
	}
}
