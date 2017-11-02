using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// à mettre sur le 2D billboarded obj death anim
// détruit tout le mouton quand l'anim est finie

public class DieWhenFinishedAnim : MonoBehaviour
{

    private Animator death;

    private void OnEnable()
    {
        death = GetComponent<Animator>();
    }

    void Update()
    {
        if (death.GetCurrentAnimatorStateInfo(0).IsName("empty"))
        {
            Destroy(transform.parent.gameObject);
        }
    }
}