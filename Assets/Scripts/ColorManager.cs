using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/*

todo:

have sheep die
count lives on GUI
make real level design (several levels)
implement enemies
options (how many enemies, lives, which start level)
keys binding
start menu
checking if you're the same colour as the ground

*/


// à mettre sur le color manager


[RequireComponent(typeof(NetworkIdentity))] //everything unchecked
public class ColorManager : NetworkBehaviour
{
	int i;

	[ClientRpc]
    public void RpcChangeCol(GameObject obj, Color col) {
		obj.GetComponent<PlayerHealth> ().TakeDamage();
		Renderer rd = obj.GetComponentInChildren<Renderer> ();
		foreach (Material mat in rd.materials)
        {
            mat.color = col;
        }
	}
    
	public void Kill(GameObject obj){
		Destroy (obj);
	}

}