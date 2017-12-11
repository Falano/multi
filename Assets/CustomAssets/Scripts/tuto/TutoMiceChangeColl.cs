using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoMiceChangeColl : MonoBehaviour {

    TutoChangeCol changeCol;

    void Start()
    {
        changeCol = transform.parent.GetComponent<TutoChangeCol>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("AttackChangeCol"))
        {
            changeCol.ChangeCol(other.gameObject);
        }   
    }
}
