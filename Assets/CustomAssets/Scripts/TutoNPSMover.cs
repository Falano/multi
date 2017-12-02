using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

// enemy mover.

public class TutoNPSMover : MonoBehaviour
{
    NavMeshAgent ag;
    public Vector3 goal;
    NavMeshHit hit;
    public Vector3 lvlSize;
    Animator animator;
    public Vector2 waitRange = new Vector2(3, 6);
    public int rotationSpeed = 5;
    private bool readyToChangeDestination = true;
    private IEnumerator wait;
    private TutoChangeCol changeCol;

    void Start()
    {
        lvlSize = LvlSize.singleton.size;
        ag = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        wait = waitForChangeDir(Random.Range(waitRange.x, waitRange.y));
        changeCol = GetComponent<TutoChangeCol>();
        StartCoroutine(wait);
    }

    void Update()
    {
        if (ag.remainingDistance <= ag.stoppingDistance && readyToChangeDestination)
        {
            readyToChangeDestination = false;
            wait = waitForChangeDir(Random.Range(waitRange.x, waitRange.y));
            StartCoroutine(wait);
        }


        Debug.DrawLine(transform.position, goal, Color.red);
        

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AttackChangeCol"))
        {
            MetMice(other.gameObject);
        }
    }
    

    public IEnumerator waitForChangeDir(float time)
    {
        animator.SetBool("moving", false);
        yield return new WaitForSeconds(time);
        ChangeDestination();
    }


    void ChangeDestination()
    {
        if (TutoManager.singleton.currState == TutoManager.gameState.lobby)
        {
            return;
        }
        animator.SetBool("moving", true);
        Vector3 randomPoint = new Vector3(Random.Range(lvlSize.x, -lvlSize.x), Random.Range(0, lvlSize.y), Random.Range(lvlSize.z, -lvlSize.z));
        goal = randomPoint;
        if (lvlSize.y >= .2)
        {
            while (!NavMesh.SamplePosition(randomPoint, out hit, 1, NavMesh.AllAreas)) // si le point random choisi n'est pas près du sol (genre s'il y a plusieurs étages), on recommence
            {
                randomPoint = new Vector3(Random.Range(lvlSize.x, -lvlSize.x), Random.Range(lvlSize.y, -lvlSize.y), Random.Range(lvlSize.z, -lvlSize.z));
            }
            goal = hit.position;
        }
        ag.SetDestination(goal);
        readyToChangeDestination = true;
    }


    void MetMice(GameObject mice)
    {
        StopAllCoroutines();
        TutoManager.singleton.speak("I SAID NO MICE.\nI'M OUT. HAVE FUN. BYE.", changeCol.speech, 2);
        goal = transform.position + (transform.position - mice.transform.position);
        Debug.Log("sheep pos: " + transform.position + ", mice pos: " + mice.transform.position + ", goal: " + goal);
        Mathf.Clamp(goal.x, -lvlSize.x, lvlSize.x); //so they don't try to wander too far
        Mathf.Clamp(goal.z, -lvlSize.z, lvlSize.z);
        ChangeDestination(goal);
    }

    void ChangeDestination(Vector3 direction)
    {
        if (TutoManager.singleton.currState == TutoManager.gameState.lobby)
        {
            return;
        }
        animator.SetBool("moving", true);
        goal = direction;
        ag.SetDestination(goal);
        readyToChangeDestination = true;
    }
}
