using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManagerPersistence : NetworkBehaviour {
    public static NetworkManager NetworkManager;

    private void Awake()
    {
        if (NetworkManager == null)
        {
            NetworkManager = gameObject.GetComponent<NetworkManager>();
        }
        else if (NetworkManager != gameObject.GetComponent<NetworkManager>()) {
            Destroy(gameObject);
        }
    }
}
