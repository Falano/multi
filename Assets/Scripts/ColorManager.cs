using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


/*

todo:

-DONE- have sheep die
die plouf graphical droplets
count lives on GUI
make real level design (several levels)
-DONE-implement enemies
-tocheck- anim enemies
options (how many enemies, how many lives, which start level, how many players in all, teamwork (brown feet), chrono, deathwave, / language, keys, sound volume, )
keys binding
start menu
checking if you're the same colour as the ground
players in a list when join
Esc -> sure wanna quit? Yes -> back to lobby

*/


// à mettre sur le color manager


[RequireComponent(typeof(NetworkIdentity))] //everything unchecked
public class ColorManager : NetworkBehaviour
{
	int i;
    public Vector3 LvlSize;


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