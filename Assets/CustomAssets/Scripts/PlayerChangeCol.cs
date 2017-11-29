using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

// à mettre sur le player
// triggers the change col 

public class PlayerChangeCol : NetworkBehaviour
{
    [SyncVar] protected Color currColor;
    protected Color prevColor;
    protected Color[] colors;

    protected RaycastHit hit;
    public float hitDistance = 1;
    protected Vector3 offsetPos;
    protected Renderer rd;

    public bool paintReady = true;
    public float cooldown = 3;
    public float speedBoostDuration = 1;
    float speedBoostStrengthFactor = 3;
    public float speedBoostStrength;
    public int currBoost = 0;

    protected void Start()
    {
        speedBoostStrength = speedBoostStrengthFactor * GetComponent<PlayerMove>().BaseSpeed;
        colors = MenuManager.colors;
        rd = GetComponentInChildren<Renderer>();
        currColor = colors[0];
        offsetPos = new Vector3(0, .5f, 0);
    }

    public void startWhite()
    {
        ChangeCol(gameObject, colors[0], ColorManager.singleton.gameObject);
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
    protected void ChangeCol(GameObject obj, GameObject attacker)
    {
        if(!ColorManager.isGamePlaying)
        {
            return;
        }
        if (obj.GetComponent<PlayerHealth>().Hp <= 0) // comme ça s'il est en train de jouer l'anim death, il ne remeurt pas.
        {
            return;
        }

        prevColor = currColor;
        // so it doesn't "change" to the same colour:
        while (prevColor == currColor)
        {
            currColor = colors[Random.Range(0, colors.Length)];
        }
        CmdChangeCol(obj, currColor, attacker);
    }


    // so I can choose to change to one specific colour
    void ChangeCol(GameObject obj, Color col, GameObject attacker)
    {
        if (obj.GetComponent<PlayerHealth>().Hp <= 0) // comme ça s'il est en train de jouer l'anim death, il ne remeurt pas.
        {
            return;
        }
        CmdChangeCol(obj, col, attacker);
    }

    [Command]
    void CmdChangeCol(GameObject obj, Color col , GameObject attacker)
    {
        ColorManager.singleton.RpcChangeCol(obj, col, attacker);
    }





    void Update()
    {
        if (isLocalPlayer)
        {
            // changing their own colour
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ChangeCol(gameObject, gameObject);
            }

            Debug.DrawRay(transform.position + offsetPos, transform.forward * hitDistance, Color.green);
            if (Input.GetKeyDown(KeyCode.Space) && paintReady)
            {
                // changing another's colour
                if (Physics.Raycast(transform.position + offsetPos, transform.forward, out hit))
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
