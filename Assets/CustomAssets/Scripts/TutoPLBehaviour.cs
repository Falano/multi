using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoPLBehaviour : MonoBehaviour {
    RaycastHit hit;
    public float hitDistance = 1;
    Vector3 offsetPos;
    Renderer rd;
    TutoChangeCol changeCol;

    public bool paintReady = true;
    public float cooldown = .5f;
    // mettre un son pour quand tu as essayé de changer la couleur de quelqu'un et tu as mal visé
    // mettre un son pour quand tu as essayé de changer la couleur de quelqu'un et le cooldown était encore en cours (un petit po-pow)

    // Use this for initialization
    void Start()
    {
        rd = GetComponentInChildren<Renderer>();
        offsetPos = new Vector3(0, .5f, 0);
        changeCol = GetComponent<TutoChangeCol>();
    }




    void Update()
    {
        // changing their own colour
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            changeCol.ChangeCol(gameObject);
        }

        Debug.DrawRay(transform.position + offsetPos, transform.forward * hitDistance, Color.green);
        // changing another's colour
        if (Physics.Raycast(transform.position + offsetPos, transform.forward * 5, out hit) && hit.transform.CompareTag("NPS"))
        {
           // TutoManager.singleton.speak(); // should I get closer?
            if (Input.GetKeyDown(KeyCode.Space) && paintReady && Vector3.Distance(hit.transform.position, transform.position) <= hitDistance)
            {
                hit.transform.gameObject.GetComponent<TutoChangeCol>().ChangeCol(gameObject);
             //   TutoManager.singleton.speak();// I totes should bully my neighbour
            }
        }

        if (rd.materials[1].color != Color.black)
        {
            rd.materials[1].color = Color.black;
        }
    }
}
