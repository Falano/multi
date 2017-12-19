using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class goYellow : MonoBehaviour
{
    TutoChangeCol changeCol;
    NavMeshAgent ag;

    // Use this for initialization
    void Start()
    {
        ag = GetComponent<NavMeshAgent>();
        GetComponent<Animator>().SetBool("moving", true);
        changeCol = GetComponent<TutoChangeCol>();
        changeCol.ChangeCol(TutoManager.colors[5]);
    }

    // Update is called once per frame
    void Update()
    {
        if (TutoManager.singleton.currState == TutoManager.gameState.lobby)
        {
            return;
        }
        transform.Rotate(0, -ag.angularSpeed * Time.deltaTime* 0.5f, 0);
        transform.Translate(0, 0, ag.speed * Time.deltaTime);

    }
}
