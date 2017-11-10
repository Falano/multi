using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

// enemy mover.
// the lvlSize that says where they spawn and move is on the colormanager (which is really a game manager)

public class EnemyMover : NetworkBehaviour
{
    NavMeshAgent ag;
    public Vector3 goal;
    NavMeshHit hit;
    public Vector3 lvlSize;
    Animator animator;
    public Vector2 waitRange = new Vector2(3,6);
    public int rotationSpeed = 5;
    private bool readyToChangeDestination = true;
    private IEnumerator wait;
    GameObject ratKing;

    void Start()
    {
        ratKing = ColorManager.singleton.ratKing;
        transform.SetParent(ratKing.transform);
        if (!isServer)
        {
            return;
        }
        lvlSize = ColorManager.singleton.LvlSize;
        ag = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        wait = waitForChangeDir(Random.Range(waitRange.x, waitRange.y));
        //animator.SetBool("moving", false);

        //StartCoroutine(wait);        
    }

    void Update()
    {
        if (!isServer)
        {
            return;
        }
        if (ag.remainingDistance <= ag.stoppingDistance && readyToChangeDestination) {
            readyToChangeDestination = false;
            wait = waitForChangeDir(Random.Range(waitRange.x, waitRange.y));
            StartCoroutine(wait);
        }

        Debug.DrawLine(transform.position, goal, Color.red);
    }

    public IEnumerator waitForChangeDir(float time)
    {
        animator.SetBool("moving", false);
        yield return new WaitForSeconds(time);
        ChangeDestination();
    }

    void ChangeDestination()
    {
        if (!ColorManager.isGamePlaying)
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
}
