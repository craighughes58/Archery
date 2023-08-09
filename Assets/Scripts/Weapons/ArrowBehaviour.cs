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
    }

    // Update is called once per frame
    void Update()
    {
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

    internal override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if(!collision.gameObject.tag.Equals("Arrow"))
        {
            HasCollided = true;
            ThisRigidBody.freezeRotation = true;
            ThisRigidBody.useGravity = false;

            if (collision.gameObject.tag.Equals("Enemy"))
            {
               EnemyController HitController = collision.gameObject.GetComponent<EnemyController>();

                if (HitController != null) 
                {
                    HitController.TakeDamage(collision, gameObject);
                
                }


            }
            else if (collision.gameObject.tag.Equals("Player"))
            {
                Health PlayerHealthSystem = collision.gameObject.GetComponent<Health>();

                PlayerHealthSystem.damage(GetInflictingDamage());

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
