using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
 

    //private variables 

    //0 = normal, 1 = grapple
    private int arrowType = 0;
    //represents if the arrow has collided
    private bool hasCollided;
    //reference to the rigidbody
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        //GetComponent<Rigidbody>().centerOfMass = Vector3.back;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        hasCollided = true;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public void setArrowType(int type)
    {
        arrowType = type;
    }
    public bool GetCollided()
    {
        return hasCollided;
    }
}
