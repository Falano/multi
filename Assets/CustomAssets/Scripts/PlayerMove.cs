using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// player move

public class PlayerMove : NetworkBehaviour
{

    public float rotationSpeed;
    private Animator animator;
    public float speed = 5;
    public Rigidbody rb;

    [SyncVar] private bool isAnimated;
    [SyncVar] private string animationName;


    public override void OnStartLocalPlayer()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        CameraMover.singleton.activePlayer = transform; // on dit à la camera que c'est lui ici le player à suivre
        if (ColorManager.isGamePlaying) // s'il arrive dans un jeu en cours 
        {
            //print("GAME IS PLAYING");
            ColorManager.singleton.LaunchGameSolo(); //il désactive la GUI du lobby
            ColorManager.singleton.Kill(gameObject); // that was so assholes who come mid-game died but could still follow it; don't think it works though
        }
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
