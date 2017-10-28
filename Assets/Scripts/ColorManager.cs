using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
        obj.transform.GetChild(0).gameObject.SetActive(false);
        obj.transform.GetChild(2).gameObject.SetActive(true);
        obj.GetComponent<BoxCollider>().enabled = false; //careful il y a deux box colliders, l'un trigger; ne pas changer leur place
        //the object destroy itself is on a script on the child
	}
}