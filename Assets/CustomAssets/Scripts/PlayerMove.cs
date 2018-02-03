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
        speed = baseSpeed*.1f;
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
            animator.SetBool("right", Input.GetKey(MenuManager.right));
            animator.SetBool("left", Input.GetKey(MenuManager.left));
        }

        if (Input.GetKey(MenuManager.right))
        {
            transform.Rotate(0, rotationSpeed*Time.deltaTime, 0);
        }
        if (Input.GetKey(MenuManager.left))
        {
            transform.Rotate(0, -rotationSpeed*Time.deltaTime, 0);
        }
        if (Input.GetKey(MenuManager.forward))
        {
            transform.Translate(0, 0, speed *Time.deltaTime);
        }
    }
}
