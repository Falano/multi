using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// à mettre sur le player
// triggers the change col
// aussi la fonction d'attaque des moutons est ici

public class PlayerChangeCol : NetworkBehaviour
{

	[SyncVar] private Color currColor;
	private Color prevColor;
	Color[] colors = { Color.yellow, Color.cyan, Color.blue, Color.green, Color.white, Color.magenta };
    private ColorManager cm;

	RaycastHit hit;
    public float hitDistance = 1;
    Vector3 offsetPos;
	Renderer rd;

    bool paintReady = true;
    [SerializeField]
    float cooldown = 3;

    void Start()
    {
		rd = GetComponentInChildren<Renderer> ();
		currColor = Color.black;
        offsetPos = new Vector3(0, .5f, 0);
        cm = GameObject.FindGameObjectWithTag("ColorManager").GetComponent<ColorManager>();
        ChangeCol(gameObject, Color.white);
    }


	// mice make them change colour
	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("AttackChangeCol")) {
			ChangeCol(this.gameObject);
		}
	}

	// changing colour
    // le ChangeCol qui est sur le mouton choisit une couleur, puis appelle CmdChangeCol (sur le mouton) qui (dit au serveur de) appelle RpcChangeCol (sur le color manager) qui dit à tous les clients que ce mouton a pris des dégâts et changé de couleur
	void ChangeCol(GameObject obj){
        paintReady = false;
		prevColor = currColor;
		// so it doesn't "change" to the same colour:
		while (prevColor == currColor) { 
			currColor = colors[Random.Range(0, colors.Length)];
		}
		CmdChangeCol (obj, currColor);
        StartCoroutine("paintCooldown", cooldown);
	}

    IEnumerator paintCooldown(float cooldown) {
        yield return new WaitForSeconds(cooldown);
        paintReady = true;
    }




	// so I can choose to change to one specific colour
	void ChangeCol(GameObject obj, Color col){ 
		CmdChangeCol (obj, col);
	}

	[Command]
	void CmdChangeCol(GameObject obj, Color col){
		cm.RpcChangeCol (obj, col);
	}





    void Update()
    {
		if(isLocalPlayer){
			// changing their own colour
			if (Input.GetKeyDown (KeyCode.LeftControl)) { 
				ChangeCol (this.gameObject);
			}

            // changing another's colour
            /*
            for (float i = -.2f; i < .2f; i += .05f)
            {
                Debug.DrawRay(transform.position + offsetPos, (transform.forward + new Vector3(0, 0, i)) * hitDistance, Color.green);
            */
            Debug.DrawRay(transform.position + offsetPos, transform.forward * hitDistance, Color.green);

            if (Input.GetKeyDown(KeyCode.Space) && paintReady)
                {

                    if (Physics.Raycast(transform.position + offsetPos, transform.forward /*+ new Vector3(0,0,i)*/, out hit))
                    {
                        if (Vector3.Distance(hit.transform.position, transform.position) <= hitDistance)
                        {
                            if (hit.transform.CompareTag("Player"))
                            {
                                ChangeCol(hit.transform.gameObject);
                            }
                        }
                    }
                }
                if (rd.materials[1].color != Color.black)
                {
                    rd.materials[1].color = Color.black;
                }


            //}
        }
	}
}
