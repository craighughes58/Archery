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
    // Start is called before the first frame update
    void Start()
    {
        //GetComponent<Rigidbody>().centerOfMass = Vector3.back;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        hasCollided = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
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
