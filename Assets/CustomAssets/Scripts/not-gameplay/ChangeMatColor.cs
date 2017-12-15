using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMatColor : MonoBehaviour {

    public Material targetMat;
    
    public void ChangeMatCol(Color col) {
        targetMat.color = col;
    }
}
