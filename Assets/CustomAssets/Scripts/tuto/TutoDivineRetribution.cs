using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoDivineRetribution : MonoBehaviour {

    RaycastHit ground;
    TutoChangeCol changeCol;
    private Color prevGroundColor;
    private Color currGroundColor;
    [SerializeField]
    float stillTime = 20;
    IEnumerator divineRetribution;



    // Use this for initialization
    void Start () {
        currGroundColor = Color.white;
        changeCol = GetComponent<TutoChangeCol>();
        InvokeRepeating("CheckGroundColor", 3, 1);
    }

    void CheckGroundColor() // so people don't stay too long in the same place
    {
        if (Physics.Raycast(transform.position, -transform.up, out ground))
        {
            prevGroundColor = currGroundColor;
            currGroundColor = ground.transform.GetComponent<Renderer>().material.color;
            if (prevGroundColor != currGroundColor && TutoManager.singleton.currState == TutoManager.gameState.playing)
            { // if you just moved ground colors, we launch a new countdown
                StopAllCoroutines();
                divineRetribution = autoChangeCol(stillTime);
                StartCoroutine(divineRetribution);
            }
        }
    }

    IEnumerator autoChangeCol(float time)
    {
        divineRetribution = autoChangeCol(stillTime);
        yield return new WaitForSeconds(time);
        changeCol.ChangeCol(TutoManager.singleton.gameObject);
        StartCoroutine(divineRetribution);
    }




    // Update is called once per frame
    void Update () {
		
	}
}
