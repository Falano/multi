using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LvlSize : MonoBehaviour
{
    public Vector3 size;
    public static LvlSize singleton;




    void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else if (singleton != this)
        {
            Destroy(this);
        }
    }
}