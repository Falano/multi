using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class TutoNPCBehaviour : PlayerBehaviour
{
    public static int nameIndex;
    void Start()
    {
        localName = "NPS-" + nameIndex;
        nameIndex++;
    }



}
