using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// à mettre sur l'anim death 2D (pour qu'elle regarde la camera)
public class Billboard : MonoBehaviour {

    public Camera m_Camera;

    // Use this for initialization
    void Start()
    {
        m_Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    void Update()
    {
        transform.LookAt(transform.position + m_Camera.transform.rotation * Vector3.forward,
            m_Camera.transform.rotation * Vector3.up);
    }
}
