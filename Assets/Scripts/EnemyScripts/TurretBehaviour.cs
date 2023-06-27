using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : EnemyAIBase
{

    #region Serialized Variables

    [Header("Object References:")]
    [Tooltip("The reference to the transform of the top piece of the turret")]
    [SerializeField] private Transform SkullTransform;

    [Tooltip("The reference to the transform of the bottom piece of the turret")]
    [SerializeField] private Transform JawTransform;

    [Tooltip("The object the turret will shoot")]
    [SerializeField] private GameObject Projectile;

    [Tooltip("The fireball that appears in the mouth of the skull before firing")]
    [SerializeField] private GameObject FakeFireball;
    [Header("Movement:")]

    [Tooltip("How fast the turret moves to look at the player")]
    [SerializeField] private float rotationSpeed;

    [Tooltip("How fast the tirret moves to shoot the player")]
    [SerializeField] private float firingSpeed;

    [Tooltip("The location where the skull will end up when firing")]
    [SerializeField] private Transform SkullUpPosition;

    [Tooltip("The time in between firing and reloading")]
    [SerializeField] private float stallTime;

    [Tooltip("The turret looks very low to the ground so this can make it look up higher")]
    [SerializeField] private float yOffset;


    #endregion

    #region Private variables

    //reference to the rigidbody
    private Rigidbody rb;
    //The starting rotation for the skull
    private Quaternion StartRotation;
    //The starting position for the skull
    private Vector3 startPosition;
    //true if the turret is looking at the player and false means it isnt
    private bool lockedOn;
    #endregion
    // Start is called before the first frame update
    internal override void Start()
    {
        base.Start();

        StartRotation = SkullTransform.localRotation;
        startPosition = SkullTransform.localPosition;
        rb = GetComponent<Rigidbody>();
    }

    internal override void Update()
    {
        base.Update();
    }
    internal override void Idle()
    {
        base.Idle();
    }
    internal override void Ranged()
    {
        base.Ranged();
        FacePlayer();
    }

    #region Movement

    /// <summary>
    /// This coroutine triggers when the player is found
    /// it will slowly rotate to face the player and then once they can see the player un blocked, it will trigger the fire coroutine 
    /// </summary>
    /// <returns></returns>
    public void FacePlayer()
    {
            //create new rotate closer to the player
            Quaternion TargetRotation = Quaternion.LookRotation(PlayerPosition + new Vector3(0,yOffset,0) - transform.position);
            //set current rotation to new rotation
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, TargetRotation, rotationSpeed));
            //lock us on!  
                    if (!lockedOn)
                    {
                        lockedOn = true;
                    //shoot the player
                     StartCoroutine(Fire());
                    }

    }

    #endregion

    #region Attacking 

    /// <summary>
    /// This script moves the turrets jaw to reveal a fireball then shoots a projectile and returns the jaw to its closed position
    /// </summary>
    /// <returns></returns>
    public IEnumerator Fire()
    {
        FakeFireball.gameObject.SetActive(true);
        float amount = 0;
        //open the jaw
        while (SkullTransform.rotation != SkullUpPosition.rotation)
                {
                    SkullTransform.position = Vector3.MoveTowards(SkullTransform.position, SkullUpPosition.position, firingSpeed * Time.deltaTime);
                    SkullTransform.rotation = Quaternion.RotateTowards(SkullTransform.rotation, SkullUpPosition.rotation, amount);
                    amount += firingSpeed * Time.deltaTime;
                    yield return new WaitForSeconds(Time.deltaTime);
                }
       //StartCoroutine(ChangeMouthPos(SkullUpPosition.rotation, SkullUpPosition.position)); doesn't work it gets out of sync
        yield return new WaitForSeconds(1f);
        FakeFireball.gameObject.SetActive(false);
        //fire the bullet
        Instantiate(Projectile, FakeFireball.transform.position, transform.rotation);
        yield return new WaitForSeconds(1f);
        //StartCoroutine(ChangeMouthPos(StartRotation * transform.rotation, startPosition + transform.position));
        //close the jaw
        while (SkullTransform.rotation != StartRotation * transform.rotation)
                {
                    SkullTransform.position = Vector3.MoveTowards(SkullTransform.position, startPosition + transform.position , firingSpeed * Time.deltaTime);
                    SkullTransform.rotation = Quaternion.RotateTowards(SkullTransform.rotation, StartRotation * transform.rotation, amount);
                    amount += firingSpeed * Time.deltaTime;
                    yield return new WaitForSeconds(Time.deltaTime);
                }
        yield return new WaitForSeconds(stallTime);
        lockedOn = false;//reset

    }
    #endregion

    #region Perception

    //Perception inherited and performed as a part of EnemyAIBase

    #endregion
}
