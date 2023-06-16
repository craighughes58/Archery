/*************************************************************************
 * Author: MaKayla Elder
 * Date: 06.08.2023
 * 
 * Description:
 * Base enemy class with base attributes and functionality to be inherited to implementable classes.
 * 
 * 
 * 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//******Re-implement abstract keyword when base class complete!
public  class EnemyController : MonoBehaviour
{

    #region Public variables
    //public variables begin
   

    //end public variables
    #endregion

    #region Private variables

    //begin private variables
    
    [Header("Weapon & Attack Properties")]

    [SerializeField] private int Ammo;

    [Tooltip("The amount of damage this enemy will inflict")]
    [SerializeField] private int Damage;

    [Tooltip("The transform location for projectile instantiation.")]
    [SerializeField] private Transform ShootFromLocation;



    //end private variables
    #endregion

    #region Public Variables

    [Header("Movement")]

    [Tooltip("How fast the enemy can move")]
    public float speed = 1;

    [Tooltip("how strong the enemy jump is")]
    public float jumpForce = 1;

    [Tooltip("How strong the gravity that affects the enemy is")]
    public float gravityForce = 1;

    [Tooltip("The multiplier added when the enemy gains speeed")]
    public float speedMultiplier;


    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //need to check the proper way to apply damage for Unity, probably not using this method
   /* private void OnCollisionEnter(Collision collision)
    {
        if (collision == null) return;

        GameObject CollidedObject = collision.gameObject;

        
    }*/

    #endregion
}
