using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class assignColorOnStart : MonoBehaviour
{
    void Start()
    {
        ChangeMatColor[] change = GetComponentsInChildren<ChangeMatColor>();
        for (int i = 0; i < MenuManager.singleton.colorsMats.Length; i++)
        {
            change[i].targetMat = MenuManager.singleton.colorsMats[i];
        }
    }
}
