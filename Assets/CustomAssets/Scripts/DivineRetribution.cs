using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineRetribution : MonoBehaviour {

    RaycastHit ground;

    Vector3 offsetPos;
    [SerializeField]
    float stillTime = 20;
    IEnumerator divineRetribution;
    private int prevGroundColorIndex;
    private int currGroundColorIndex;
    PlayerChangeCol changeCol;

    // Use this for initialization
    void Start () {
        changeCol = GetComponent<PlayerChangeCol>();
        offsetPos = changeCol.offsetPos;
        currGroundColorIndex = 0;
        InvokeRepeating("CheckGroundColor", 1, 1);
	}



    void CheckGroundColor() // so people don't stay too long in the same place
    {
        if (Physics.Raycast(transform.position + offsetPos, -transform.up, out ground))
        {
            prevGroundColorIndex = currGroundColorIndex;
            currGroundColorIndex = System.Array.IndexOf(MenuManager.curr6Colors, ground.transform.GetComponent<Renderer>().material.color);
            if (prevGroundColorIndex != currGroundColorIndex && ColorManager.singleton.CurrState == ColorManager.gameState.playing)
            { // if you just moved ground colors, we launch a new countdown
                StopAllCoroutines();
                StartCoroutine(divineRetribution = autoChangeCol(stillTime));
            }
        }
    }

    IEnumerator autoChangeCol(float time)
    {
        yield return new WaitForSeconds(time);

        changeCol.ChangeCol(gameObject, ColorManager.singleton.gameObject);
        StartCoroutine(autoChangeCol(stillTime));
    }
}
