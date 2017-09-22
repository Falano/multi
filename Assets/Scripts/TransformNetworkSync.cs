using UnityEngine;
using UnityEngine.Networking;

//only 2D Y rotation for now
// added a teleportDistance variable

public class TransformNetworkSync : NetworkBehaviour
{
    
    [SyncVar]
    private Vector3 syncPos;

    [SyncVar]
    private float syncYRot;

    private Vector3 lastPos;
    private Quaternion lastRot;
    private Transform myTransform;
    [SerializeField]
    private float lerpRate = 10; //higher is laggy, lower is too slow
    [SerializeField]
    private float posThreshold = 0.5f;
    [SerializeField]
    private float rotThreshold = 5;
	[SerializeField]
	private float teleportDistance = 5;

    // Use this for initialization
    void Start()
    {
        myTransform = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TransmitMotion();
        LerpMotion();
    }

    [Command]
    void Cmd_ProvidePositionToServer(Vector3 pos, float rot)
    {
        syncPos = pos;
        syncYRot = rot;
    }

    [ClientCallback]
    void TransmitMotion()
    {
        if (hasAuthority)
        {
            if (Vector3.Distance(myTransform.position, lastPos) > posThreshold || Quaternion.Angle(myTransform.rotation, lastRot) > rotThreshold)
            {
                Cmd_ProvidePositionToServer(myTransform.position, myTransform.localEulerAngles.y);

                lastPos = myTransform.position;
                lastRot = myTransform.rotation;
            }
        }
    }

    void LerpMotion()
    {
		if (!hasAuthority && Vector3.Distance(myTransform.transform.position, syncPos) < teleportDistance)
        {
            myTransform.position = Vector3.Lerp(myTransform.transform.position, syncPos, Time.deltaTime * lerpRate);

            Vector3 newRot = new Vector3(0, syncYRot, 0);
            myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.Euler(newRot), Time.deltaTime * lerpRate);
        }
    }
}

