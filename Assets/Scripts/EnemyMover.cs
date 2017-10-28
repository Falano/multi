using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMover : MonoBehaviour
{
    NavMeshAgent ag;
    public Vector3 goal;
    NavMeshHit hit;
    public Vector3 lvlSize;
    Animator animator;
    public Vector2 waitRange = new Vector2(3,6);
    public int rotationSpeed = 5;
    private bool readyToChangeDestination = true; 

    void Start()
    {
        lvlSize = GameObject.FindGameObjectWithTag("ColorManager").GetComponent<ColorManager>().LvlSize;
        ag = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        IEnumerator wait = waitForChangeDir(Random.Range(waitRange.x, waitRange.y));
        StartCoroutine(wait);        
    }

    void Update()
    {
        if (ag.remainingDistance <= ag.stoppingDistance && readyToChangeDestination) {
            readyToChangeDestination = false;
            IEnumerator wait = waitForChangeDir(Random.Range(waitRange.x, waitRange.y));
            StartCoroutine(wait);
        }

        Debug.DrawLine(transform.position, goal, Color.red);
    }

    IEnumerator waitForChangeDir(float time)
    {
        animator.SetBool("moving", false);
        yield return new WaitForSeconds(time);
        ChangeDestination();
        }

    void ChangeDestination()
    {
        animator.SetBool("moving", true);
        Vector3 randomPoint = new Vector3(Random.Range(lvlSize.x, -lvlSize.x), Random.Range(0, lvlSize.y), Random.Range(lvlSize.z, -lvlSize.z));
        goal = randomPoint;
        if (lvlSize.y >= .2)
        {
            while (!NavMesh.SamplePosition(randomPoint, out hit, 1, NavMesh.AllAreas))
            {
                randomPoint = new Vector3(Random.Range(lvlSize.x, -lvlSize.x), Random.Range(lvlSize.y, -lvlSize.y), Random.Range(lvlSize.z, -lvlSize.z));
            }
            goal = hit.position;
        }
        ag.SetDestination(goal);
        readyToChangeDestination = true;
    }
}
