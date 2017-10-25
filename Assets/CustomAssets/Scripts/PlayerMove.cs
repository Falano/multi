using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerMove : NetworkBehaviour
{

    public float rotationSpeed;
    public Vector3 posOffset;
    private Animator animator;
    public float speed = 5;
    public Rigidbody rb;

    [SyncVar] private bool isAnimated;
    [SyncVar] private string animationName;

    private GameObject worldCamera;

    public override void OnStartLocalPlayer()
    {
        worldCamera = GameObject.FindGameObjectWithTag("MainCamera");
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        worldCamera.transform.position = transform.position + posOffset;

        if (animator)
        {
            animator.SetBool("moving", Input.GetKey(KeyCode.UpArrow));
        }

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
            transform.Translate(0, 0, speed * 0.01f);
        }

    }
}
