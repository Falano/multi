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
    bool moving;
    Animator animator;
    public Vector2 waitRange = new Vector2(3,6);
    public int rotationSpeed = 5;
    private bool readyToChangeDestination = true; 

    void Start()
    {
        ag = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ChangeDestination();
        lvlSize = GameObject.FindGameObjectWithTag("ColorManager").GetComponent<ColorManager>().LvlSize;
    }

    void Update()
    {
        if (ag.remainingDistance <= ag.stoppingDistance && readyToChangeDestination) {
            readyToChangeDestination = false;
            IEnumerator wait = waitForChangeDir(Random.Range(waitRange.x, waitRange.y));
            StartCoroutine(wait);
        }

        Debug.DrawLine(transform.position, goal, Color.red);

        /*
        if ( transform.forward != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(transform.forward),
                Time.deltaTime * rotationSpeed
            );
        }
        */

    }

    IEnumerator waitForChangeDir(float time)
    {
        animator.SetBool("moving", false);
        print("stopping mice; time = " + Time.fixedTime + " , waiting between " + waitRange.x + " and " + waitRange.y + " seconds.");
        yield return new WaitForSeconds(time);
        //yield return new WaitForSeconds(0);
        print("ok, looking for a new goal");
        ChangeDestination();
        }

    void ChangeDestination()
    {
        animator.SetBool("moving", true);
        Vector3 randomPoint = new Vector3(Random.Range(lvlSize.x, -lvlSize.x), Random.Range(lvlSize.y, -lvlSize.y), Random.Range(lvlSize.z, -lvlSize.z));
        goal = randomPoint;
        if (lvlSize.y >= .1)
        {
            while (!NavMesh.SamplePosition(randomPoint, out hit, 1, NavMesh.AllAreas))
            {
                randomPoint = new Vector3(Random.Range(lvlSize.x, -lvlSize.x), Random.Range(lvlSize.y, -lvlSize.y), Random.Range(lvlSize.z, -lvlSize.z));
            } //not sure about that one
            goal = hit.position;
        }
        ag.SetDestination(goal);
        print("I just set a new goal!");
        readyToChangeDestination = true;
    }

}
