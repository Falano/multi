using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// à mettre sur tous les moutons dans le tuto
// change la couleur des gens

public class TutoChangeCol : MonoBehaviour
{
    Renderer rd;
    GameObject deathAnim;

    void Start()
    {
        rd = GetComponentInChildren<Renderer>();
        deathAnim = GetComponentInChildren<DieWhenFinishedAnim>().gameObject;
    }


    void ChangeCol()
    {
        //rd.Material =  ////////////////////////////////////////////////////////////

    }

}
