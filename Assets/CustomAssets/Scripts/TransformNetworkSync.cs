using UnityEngine;
using UnityEngine.Networking;

// script found on the internet 
// because stupid unity network transform doesn't work with kinematic rigidbodies - or transforms? - with whatever I use
// only 2D Y rotation for now
// added a teleportDistance variable

public class TransformNetworkSync : NetworkBehaviour
{
    
    [SyncVar]
    private Vector3 syncPos;
    [SyncVar]
    private float syncYRot;
    [SyncVar]
    private float syncXRot;

    private Vector3 lastPos;
    private Quaternion lastRot;
    private Transform myTransform;
    [SerializeField]
    private float lerpRate = 10; //higher is laggy, lower is too slow
    [SerializeField]
    private float posThreshold = 0.5f; // distance à partir de laquelle on considère qu'il a bougé et on dit au server d'updater sa pos
    [SerializeField]
    private float rotThreshold = 5; // distance à partir de laquelle on considère qu'il a bougé et on dit au server d'updater sa rot
    [SerializeField]
	private float teleportDistance = 5;

    void Start()
    {
        myTransform = transform;
    }

    void FixedUpdate()
    {
        TransmitMotion();
        LerpMotion();
    }

    [Command]
    void Cmd_ProvidePositionToServer(Vector3 pos, float rotX, float rotY)
    {
        syncPos = pos;
        syncYRot = rotY;
        syncXRot = rotX;
    }

    [ClientCallback]
    void TransmitMotion()
    {
        if (hasAuthority)
        {
            if (Vector3.Distance(myTransform.position, lastPos) > posThreshold || Quaternion.Angle(myTransform.rotation, lastRot) > rotThreshold)
            {
                Cmd_ProvidePositionToServer(myTransform.position, myTransform.localEulerAngles.x, myTransform.localEulerAngles.y);
                lastPos = myTransform.position;
                lastRot = myTransform.rotation;
            }
        }
    }

    void LerpMotion()
    {
		if (!hasAuthority && Vector3.Distance(myTransform.position, syncPos) < teleportDistance)
        {
            myTransform.position = Vector3.Lerp(myTransform.transform.position, syncPos, Time.deltaTime * lerpRate);

            Vector3 newRot = new Vector3(0, syncYRot, 0);
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.Euler(newRot), Time.deltaTime * lerpRate);
        }
    }
}

