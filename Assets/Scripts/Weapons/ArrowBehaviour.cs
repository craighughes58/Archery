using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : WeaponBehaviorBase
{

    //inherited variables

    //represents if the weapon has collided
    //internal bool HasCollided;

    //reference to the rigidbody
    //internal Rigidbody ThisRigidBody;

    //end inherited variables

    //private variables 

    //0 = normal, 1 = grapple
    private int arrowType = 0;

    
    //
    private Transform finalTransform;

    // Start is called before the first frame update
    void Start()
    {
        ThisRigidBody = GetComponent<Rigidbody>();
        //GetComponent<Rigidbody>().centerOfMass = Vector3.back;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.LookRotation(ThisRigidBody.velocity);
    }
    private void FixedUpdate()
    {
        if(!HasCollided)
        {
            transform.rotation = Quaternion.LookRotation(ThisRigidBody.velocity);
        }
        else
        {
            ThisRigidBody.velocity = Vector3.zero;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        Debug.Log("In Arrow Behavior");

        if(!collision.gameObject.tag.Equals("Arrow"))
        {
            HasCollided = true;
            ThisRigidBody.freezeRotation = true;
            ThisRigidBody.useGravity = false;
            //ThisRigidBody.isKinematic = true;

            if (collision.gameObject.tag.Equals("Enemy"))
            {
               EnemyController HitController = collision.gameObject.GetComponent<EnemyController>();

                if (HitController != null) 
                {
                    HitController.TakeDamage(collision);
                
                }


            }

        }
        

    }

    public void setArrowType(int type)
    {
        arrowType = type;
    }
    public int GetArrowType()
    {
        return arrowType;
    }
    public bool GetCollided()
    {
        return HasCollided;
    }
}
