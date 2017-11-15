using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// à mettre sur le player
// triggers the change col 

public class PlayerChangeCol : NetworkBehaviour
{
    [SyncVar] private Color currColor;
    private Color prevColor;
    Color[] colors = { Color.yellow, Color.cyan, Color.blue, Color.green, Color.white, Color.magenta };

    RaycastHit hit;
    public float hitDistance = 1;
    Vector3 offsetPos;
    Renderer rd;

    bool paintReady = true;
    [SerializeField]
    float cooldown = 3;

    void Start()
    {
        rd = GetComponentInChildren<Renderer>();
        currColor = Color.white;
        offsetPos = new Vector3(0, .5f, 0);
        Invoke("startWhite", .5f);
    }

    void startWhite()
    {
        ChangeCol(gameObject, Color.white/*, ColorManager.singleton.gameObject*/);
    }

    // mice make them change colour
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AttackChangeCol"))
        {
            ChangeCol(this.gameObject/*, other.gameObject*/);
        }
    }

    // changing colour
    // le ChangeCol qui est sur le mouton choisit une couleur, puis appelle CmdChangeCol (sur le mouton) qui (dit au serveur de) appelle RpcChangeCol (sur le color manager) qui dit à tous les clients que ce mouton a pris des dégâts et changé de couleur 
    void ChangeCol(GameObject obj/*, GameObject attacker*/)
    {
        if (obj.GetComponent<PlayerHealth>().Hp <= 0) // comme ça s'il est en train de jouer l'anim death, il ne remeurt pas.
        {
            return;
        }
        paintReady = false;
        prevColor = currColor;
        // so it doesn't "change" to the same colour:
        while (prevColor == currColor)
        {
            currColor = colors[Random.Range(0, colors.Length)];
        }
        CmdChangeCol(obj, currColor/*, attacker*/);
        StartCoroutine("paintCooldown", cooldown);
    }

    IEnumerator paintCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        paintReady = true;
    }




    // so I can choose to change to one specific colour
    void ChangeCol(GameObject obj, Color col/*, GameObject attacker*/)
    {
        if (obj.GetComponent<PlayerHealth>().Hp <= 0) // comme ça s'il est en train de jouer l'anim death, il ne remeurt pas.
        {
            return;
        }
        CmdChangeCol(obj, col/*, attacker*/);
    }

    [Command]
    void CmdChangeCol(GameObject obj, Color col /*, GameObject attacker*/)
    {
        ColorManager.singleton.RpcChangeCol(obj, col/*, attacker*/);
    }





    void Update()
    {
        if (isLocalPlayer)
        {
            // changing their own colour
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ChangeCol(this.gameObject/*, this.gameObject*/);
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
                            ChangeCol(hit.transform.gameObject/*, this.gameObject*/);
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
