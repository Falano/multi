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
        if (!isLocalPlayer || ColorManager.singleton.CurrState != ColorManager.gameState.playing)
        {
            return;
        }

        if (animator)
        {
            animator.SetBool("moving", Input.GetKey(MenuManager.forward));
        }

        if (Input.GetKey(MenuManager.right))
        {
            // add a "rotating right without advancing" anim?
            transform.Rotate(0, rotationSpeed*Time.deltaTime, 0);
        }
        if (Input.GetKey(MenuManager.left))
        {
            // add a "rotating left without advancing" anim?
            transform.Rotate(0, -rotationSpeed*Time.deltaTime, 0);
        }
        if (Input.GetKey(MenuManager.forward))
        {
            transform.Translate(0, 0, speed *Time.deltaTime);
        }
    }
}
