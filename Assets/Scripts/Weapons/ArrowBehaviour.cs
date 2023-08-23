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

    private string _ArrowOriginClass;

    [SerializeField] private float _TimeToLive = 2;


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

        HasCollided = true;
        ThisRigidBody.freezeRotation = true;

        //ThisRigidBody.useGravity = false;

        if (collision.gameObject.CompareTag("Enemy") 
                && 
               (!(_ArrowOriginClass == "Enemy")
               && !collision.gameObject.CompareTag("Arrow")) 
               )
            {

                EnemyController HitController = collision.gameObject.GetComponent<EnemyController>();

                if (HitController != null) 
                {
                    HitController.TakeDamage(collision, gameObject);
                
                }


            }
            else if (collision.gameObject.CompareTag("Player")
                     &&
                     (!!(_ArrowOriginClass == "Player")
                     && !collision.gameObject.CompareTag("Arrow")) 
                    )
            {
                Health PlayerHealthSystem = collision.gameObject.GetComponent<Health>();

                PlayerHealthSystem.damage(GetInflictingDamage());

            }
        if (arrowType == 0)
        {
            StartCoroutine(DestroyCountdown());

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

    internal void SetArrowOriginClass(string OriginClass)
    {
        _ArrowOriginClass = OriginClass;
    }

    internal string GetArrowOriginClass()
    {
        return _ArrowOriginClass;
    }

    internal IEnumerator DestroyCountdown()
    {
        while (_TimeToLive > 0)
        {
            if ((_TimeToLive -= Time.deltaTime) < 0)
            {
                _TimeToLive = 0;
            }
            else _TimeToLive -= Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);

        }
        Destroy(gameObject);
    }
}
