using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// à mettre sur le player

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
	void ChangeCol(GameObject obj){
		prevColor = currColor;
		// so it doesn't "change" to the same colour:
		while (prevColor == currColor) { 
			currColor = colors[Random.Range(0, colors.Length)];
		}
		CmdChangeCol (obj, currColor);
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

			Debug.DrawRay(transform.position + offsetPos, transform.forward * hitDistance, Color.green);
			if (Input.GetKeyDown (KeyCode.Space)) {
				// changing another's colour
				if (Physics.Raycast (transform.position + offsetPos, transform.forward, out hit)) {
					if (Vector3.Distance (hit.transform.position, transform.position) <= hitDistance) {
						if (hit.transform.CompareTag ("Player")) {
							ChangeCol (hit.transform.gameObject);
						}
					}
				}
			}
			if (rd.materials [1].color != Color.black) {
				rd.materials [1].color = Color.black;
			}
        }
	}
}
