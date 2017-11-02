using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// à mettre sur le color manager
// dit à tout le monde que le mouton a changé de couleur (rpc)
// contient la fonction finale Kill qui désactive le renderer du mouton et active death anim; the object destroy itself is on a script on the child

[RequireComponent(typeof(NetworkIdentity))] //everything unchecked
public class ColorManager : NetworkBehaviour
{
	int i;
    public Vector3 LvlSize;
    public static ColorManager singleton;

    void Awake()
    {
        if (singleton == null) {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }
    }

    [ClientRpc]
    public void RpcChangeCol(GameObject obj, Color col) {
		obj.GetComponent<PlayerHealth> ().TakeDamage();
        if (obj.GetComponent<PlayerHealth>().Hp > 0) { // pour que la flaque de peinture soit de la dernière couleur vue et pas d'une nouvelle couleur random (cf Kill() ci-dessous)
            Renderer rd = obj.GetComponentInChildren<Renderer>();
            foreach (Material mat in rd.materials)
            {
                mat.color = col;
            }
        }
		
	}
    
	public void Kill(GameObject obj){
        GameObject mesh = obj.transform.GetChild(0).gameObject;
        mesh.SetActive(false);
        GameObject death = obj.transform.GetChild(2).gameObject;
        death.SetActive(true);
        death.GetComponent<SpriteRenderer>().color = mesh.GetComponent<Renderer>().material.color;
        print("deathcol = " + death.GetComponent<SpriteRenderer>().color);
        print("meshcol = " + mesh.GetComponent<Renderer>().material.color);
        obj.GetComponent<BoxCollider>().enabled = false; //careful il y a deux box colliders, l'un trigger; ne pas changer leur place
        //the object destroy itself is on a script on the child
	}
}