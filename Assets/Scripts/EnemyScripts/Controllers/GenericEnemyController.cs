/*************************************************************************
 * Author: MaKayla Elder
 * Date: 07.27.23
 * 
 * Description:
 * Generic Moving Enemy Controller for later iteration.
 * 
 */
using System;
using UnityEngine;


public class GenericEnemyController : EnemyController
{


    [Header("Enemy Colliders")]

   [ReadOnly] [SerializeField] protected Collider _Head;
   [ReadOnly] [SerializeField] protected Collider _Body;
    protected Collider[] _Colliders; 

    

    protected Health _Health;

    private void Start()
    {
        _Health = GetComponent<Health>();
        
        SetupColliders();
      
    }
    /// <summary>
    /// Matches and assigns the colliders attached to this GameObject to collider variables.
    /// Sets up ignoring colliders that are on this GameObject as well as the floor Plane(of type MeshCollider).
    /// </summary>
    private void SetupColliders()
    {
        #region Collect the Colliders
        //we need two total arrays of our colliders so we can easily process the ignoring to itself
        _Colliders = GetComponentsInChildren<Collider>();
        Collider[] _TempCopy;

        _TempCopy = _Colliders.Clone() as Collider[];

        //a reference to the floor plane which also needs to be ignored
        MeshCollider Plane = GameObject.Find("Plane").GetComponent<MeshCollider>();
        #endregion

        //safety check for nulls
        if (_Colliders != null &&
            _TempCopy != null &&
            Plane != null)
        {
            #region Populate Bodily Colliders
            //quick run through for labeling
            foreach (Collider collider in _Colliders)
            {
                if (collider.name == "Head")
                {
                    _Head = collider;
                }
                else if (collider.name == "Body")
                {
                    _Body = collider;
                }
            }
            #endregion

            #region Ignore Collision Processing
            //ignore collision pass
            foreach (Collider c1 in _Colliders)
            {
                foreach (Collider c2 in _Colliders)
                {
                    Physics.IgnoreCollision(c1, c2);
                }
                Physics.IgnoreCollision(c1, Plane);
            }
            #endregion
        }

        #region Center Of Mass Adjustment
        //fix the center of mass so that the multiple colliders are not creating a conflicting vector
        Vector3 CenterOfMass = gameObject.transform.localPosition;
        CenterOfMass.y /= 2;
        GetComponent<Rigidbody>().centerOfMass = CenterOfMass;
        #endregion
    }

    /// <summary>
    /// Processes damage to the colliders attached to this GameObject.
    /// </summary>
    /// <param name="collision">The collision event causing damage.</param>
    /// <param name="attacker">The GameObject responsible for the collision.</param>
    internal override void TakeDamage(Collision collision, GameObject attacker)
    {
        #region Safety catch for no health component.
        if (_Health == null) 
        {
            return;
        }
        #endregion

        base.TakeDamage(collision, attacker);

        Debug.Log("attacker is " + attacker.transform.root.tag);

        if (attacker.transform.root.CompareTag("Enemy"))
        {
            return;
        }


        #region Apply Damage w/ Area of Damage Adjustments
        if (_Head != null && _Body != null) 
        {

            WeaponBehaviorBase weaponBehavior = attacker.GetComponent<WeaponBehaviorBase>();

            if (collision.collider == _Head)
            {
             if (weaponBehavior != null)
                {
                    _Health.damage(_DamageMultiplier * weaponBehavior.GetInflictingDamage());
                }                
            }
            else if (collision.collider == _Body)
            {
                if(weaponBehavior != null)
                {
                    _Health.damage(weaponBehavior.GetInflictingDamage());
                }
            }
        }
        #endregion

        #region Pass to OnDeath() if now Dead
        if (!_Health.isAlive())
        {
            OnDeath(collision,attacker);
        }
        #endregion

    }

    /// <summary>
    /// Processes all actions to take once an enemy has died
    /// </summary>
    /// <param name="collision">The collision that caused the death.</param>
    /// <param name="attacker">The GameObject responsible for the collision.</param>
    protected override void OnDeath(Collision collision, GameObject attacker)
    {
        base.OnDeath(collision, attacker);


        Destroy(this.gameObject);

        

    }



}