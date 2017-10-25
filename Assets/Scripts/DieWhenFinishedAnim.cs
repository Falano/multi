using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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