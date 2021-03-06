﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// à mettre sur le player
// triggers the change col 

public class PlayerChangeCol : NetworkBehaviour
{
    [SyncVar] private int currColorIndex;
    private int prevColorIndex;
    Color[] colors;


    RaycastHit hit;
    RaycastHit ground;
    public float hitDistance = 1.5f;
    Vector3 offsetPos;
    Renderer rd;

    public bool paintReady = true;
    public float cooldown = 3;
    public float speedBoostDuration = 1;
    [SerializeField]
    float speedBoostStrengthFactor = 2;
    public float speedBoostStrength;
    public int currBoost = 0;
    //public Vector3 offsetTarget;
    private int prevGroundColorIndex;
    private int currGroundColorIndex;
    [SerializeField]
    float stillTime;
    IEnumerator divineRetribution;

    void Start()
    {
        speedBoostStrength = speedBoostStrengthFactor * GetComponent<PlayerMove>().BaseSpeed;
        colors = MenuManager.colors;
        rd = GetComponentInChildren<Renderer>();
        currColorIndex = 0;
        offsetPos = new Vector3(0, .5f, 0);
        currGroundColorIndex = 0;
        InvokeRepeating("CheckGroundColor", 1, 1);

    }

    public void startWhite()
    {
        ChangeCol(gameObject, 0, ColorManager.singleton.gameObject);
    }

    // mice make them change colour
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AttackChangeCol"))
        {
            ChangeCol(gameObject, other.gameObject);
        }
    }

    // changing colour
    // le ChangeCol qui est sur le mouton choisit une couleur, puis appelle CmdChangeCol (sur le mouton) qui (dit au serveur de) appelle RpcChangeCol (sur le color manager) qui dit à tous les clients que ce mouton a pris des dégâts et changé de couleur 
    void ChangeCol(GameObject obj, GameObject attacker)
    {
        if(ColorManager.singleton.CurrState != ColorManager.gameState.playing)
        {
            return;
        }
        if (obj.GetComponent<PlayerHealth>().Hp <= 0) // comme ça s'il est en train de jouer l'anim death, il ne remeurt pas.
        {
            return;
        }

        prevColorIndex = currColorIndex;
        // so it doesn't "change" to the same colour:
        while (prevColorIndex == currColorIndex)
        {
            currColorIndex = Random.Range(0, colors.Length);
        }
        if (isLocalPlayer)
        {
            CmdChangeCol(obj, currColorIndex, attacker);
        }
    }


    // so I can choose to change to one specific colour
    void ChangeCol(GameObject obj, int colIndex, GameObject attacker)
    {
        print("changeCol");
        if (obj.GetComponent<PlayerHealth>().Hp <= 0) // comme ça s'il est en train de jouer l'anim death, il ne remeurt pas.
        {
            return;
        }
        if (isLocalPlayer)
        {
            CmdChangeCol(obj, colIndex, attacker);
        }
    }

    [Command]
    void CmdChangeCol(GameObject obj, int colIndex , GameObject attacker)
    {
        ColorManager.singleton.RpcChangeCol(obj, colIndex, attacker);
    }

    void CheckGroundColor() // so people don't stay too long in the same place
    {
        if (Physics.Raycast(transform.position+offsetPos, -transform.up, out ground))
        {
            prevGroundColorIndex = currGroundColorIndex;
            currGroundColorIndex =  System.Array.IndexOf(MenuManager.colors, ground.transform.GetComponent<Renderer>().material.color) ;
            if (prevGroundColorIndex != currGroundColorIndex && ColorManager.singleton.CurrState == ColorManager.gameState.playing)
            { // if you just moved ground colors, we launch a new countdown
                StopAllCoroutines();
                divineRetribution = autoChangeCol(stillTime);
                StartCoroutine(divineRetribution);
            }
        }
    }

    IEnumerator autoChangeCol(float time)
    {
        divineRetribution = autoChangeCol(stillTime);
        yield return new WaitForSeconds(time);
        
        ChangeCol(gameObject, ColorManager.singleton.gameObject);
        StartCoroutine(divineRetribution);
    }

    void Update()
    {
        if (isLocalPlayer)
        {
            if(ColorManager.singleton.CurrState != ColorManager.gameState.playing)
            {
                return;
            }
            // changing their own colour
            if (Input.GetKeyDown(MenuManager.selfChange))
            {
                ChangeCol(gameObject, gameObject);
            }

            // changing another's colour
            Debug.DrawRay(transform.position + offsetPos, transform.forward * hitDistance, Color.green);
            Debug.DrawRay(transform.position + offsetPos, (transform.forward + transform.right/6).normalized * hitDistance, Color.green);
            Debug.DrawRay(transform.position + offsetPos, (transform.forward - transform.right/6).normalized * hitDistance, Color.green);
            Debug.DrawRay(transform.position + offsetPos, (transform.forward + transform.up/4).normalized * hitDistance, Color.green);
            Debug.DrawRay(transform.position + offsetPos, (transform.forward - transform.up / 4).normalized * hitDistance, Color.green);

            if (Input.GetKeyDown(MenuManager.interact) && paintReady)
            {
                if (Physics.Raycast(transform.position + offsetPos, transform.forward, out hit) ||
                    Physics.Raycast(transform.position + offsetPos, transform.forward + transform.right / 6, out hit) ||
                    Physics.Raycast(transform.position + offsetPos, transform.forward - transform.right / 6, out hit) ||
                    Physics.Raycast(transform.position + offsetPos, transform.forward + transform.up / 4, out hit) ||
                    Physics.Raycast(transform.position + offsetPos, transform.forward - transform.up / 4, out hit)
                    )
                {
                    if (Vector3.Distance(hit.transform.position, transform.position) <= hitDistance)
                    {
                        if (hit.transform.CompareTag("Player"))
                        {
                            ChangeCol(hit.transform.gameObject, gameObject);
                        }
                    }
                }
            }
            if (rd.materials[1].color != Color.black)
            {
                rd.materials[1].color = Color.black;
            }
        }
    }
}
