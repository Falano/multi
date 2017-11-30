using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// player move

public class PlayerMove : NetworkBehaviour
{

    public float rotationSpeed;
    public Animator animator;
    [SerializeField]
    private float baseSpeed = 5;
    public float speed;
    public Rigidbody rb;

    [SyncVar] private bool isAnimated;
    [SyncVar] private string animationName;

    public float BaseSpeed
    {
        get
        {
            return baseSpeed;
        }
    }

    public override void OnStartLocalPlayer()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        speed = 0;
    }

    void Update()
    {
        if (!isLocalPlayer || !ColorManager.isGamePlaying)
        {
            return;
        }

        if (animator)
        {
            animator.SetBool("moving", Input.GetKey(KeyCode.UpArrow));
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            // add a "rotating right without advancing" anim?
            transform.Rotate(0, rotationSpeed*Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            // add a "rotating left without advancing" anim?
            transform.Rotate(0, -rotationSpeed*Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(0, 0, speed *Time.deltaTime);
        }
    }
}
