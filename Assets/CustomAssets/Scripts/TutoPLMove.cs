using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// player move in the tuto

[RequireComponent(typeof(Animator))]
public class TutoPLMove : MonoBehaviour
{

    public float rotationSpeed = 5;
    public Animator animator;
    [SerializeField]
    private float baseSpeed = 5;
    public float speed;
    public Rigidbody rb;

    public float BaseSpeed
    {
        get
        {
            return baseSpeed;
        }
    }

    void  Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        speed = 0;
    }

    void Update()
    {
        if (TutoManager.singleton.currState != TutoManager.gameState.playing)
        {
            return;
        }

        animator.SetBool("moving", Input.GetKey(KeyCode.UpArrow));

        if (Input.GetKey(KeyCode.RightArrow))
        {
            // add a "rotating right without advancing" anim?
            transform.Rotate(0, rotationSpeed, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // add a "rotating left without advancing" anim?
            transform.Rotate(0, -rotationSpeed, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(0, 0, speed * Time.deltaTime);
        }
    }
}
