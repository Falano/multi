using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMatColor : MonoBehaviour {

    public Material targetMat;
    private Image selfCol;
    
    void Start()
    {
        selfCol = gameObject.GetComponent<Image>();
        selfCol.color = targetMat.color;
    }

    public void ChangeMatCol() {
        targetMat.color = selfCol.color;
        print("changing material color: "+selfCol.color.ToString());
    }
}
