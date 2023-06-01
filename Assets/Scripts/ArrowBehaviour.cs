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
    //
    private Transform finalTransform;

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
    private void FixedUpdate()
    {
        if(!hasCollided)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
        else
        {
            rb.velocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!collision.gameObject.tag.Equals("Arrow"))
        {
            hasCollided = true;
            rb.freezeRotation = true;
            rb.useGravity = false;
            //rb.isKinematic = true;

        }

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
