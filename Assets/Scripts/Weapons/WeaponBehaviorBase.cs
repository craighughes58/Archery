using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehaviorBase : MonoBehaviour
{
    #region Private Variables


    //represents if the weapon has collided
    internal bool HasCollided;

    //reference to the rigidbody
    internal Rigidbody ThisRigidBody;
    //


    #endregion


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal virtual void OnCollisionEnter(Collision collision) 
    {
        Debug.Log("Weapon Behavior base");
    }
}
