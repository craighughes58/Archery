using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretBehaviour : MonoBehaviour
{

    #region Serialized Variables

    [Header("Zone of Perception:")]
    [Tooltip("The zone where the player is seen by the enemy")]
    [SerializeField] private SphereCollider SphereCol;

    [Tooltip("The radius of the sphere collider")]
    [SerializeField] private float perceptionRadius;

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
    //active reference to the player
    private Transform Player;
    //reference to the rigidbody
    private Rigidbody rb;
    //
    private Rigidbody SkullRb;
    //
    private Quaternion StartRotation;
    //
    private Vector3 startPosition;
    //
    private bool lockedOn;
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        SphereCol.radius = perceptionRadius;
        StartRotation = SkullTransform.localRotation;
        startPosition = SkullTransform.localPosition;
        rb = GetComponent<Rigidbody>();
        SkullRb = SkullTransform.gameObject.GetComponent<Rigidbody>();
        Player = null;
    }


    #region Movement

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator FacePlayer()
    {
        RaycastHit Hit;
        while(Player != null)
        {
            Quaternion TargetRotation = Quaternion.LookRotation(Player.position + new Vector3(0,yOffset,0) - transform.position);//create new rotate closer to the player
            rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, TargetRotation, rotationSpeed));//set current rotation to new rotation
            if (Physics.Raycast(transform.position, transform.forward, out Hit))//this section checks if the raycast hits a collider and if the collider is the player
            {
                if (Hit.collider)//hit a collider
                {
                    if (Hit.collider.tag.Equals("Player") && !lockedOn)//collider is player
                    {
                        lockedOn = true;
                        StartCoroutine(Fire());//shoot the player
                    }

                }
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator ReturnToStationary()
    {
        yield return null;
    }
    #endregion

    #region Attacking 

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator Fire()
    {
        FakeFireball.gameObject.SetActive(true);
        float amount = 0;

                while (SkullTransform.rotation != SkullUpPosition.rotation)//open the jaw
                {
                    SkullTransform.position = Vector3.MoveTowards(SkullTransform.position, SkullUpPosition.position, firingSpeed * Time.deltaTime);
                    SkullTransform.rotation = Quaternion.RotateTowards(SkullTransform.rotation, SkullUpPosition.rotation, amount);
                    amount += firingSpeed * Time.deltaTime;
                    yield return new WaitForSeconds(Time.deltaTime);
                }
       //StartCoroutine(ChangeMouthPos(SkullUpPosition.rotation, SkullUpPosition.position)); doesn't work it gets out of sync
        yield return new WaitForSeconds(1f);
        FakeFireball.gameObject.SetActive(false);
        Instantiate(Projectile, FakeFireball.transform.position, transform.rotation);//fire the bullet
        yield return new WaitForSeconds(1f);
        //StartCoroutine(ChangeMouthPos(StartRotation * transform.rotation, startPosition + transform.position));
                while (SkullTransform.rotation != StartRotation * transform.rotation)//close the jaw
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            Player = other.transform;
            StartCoroutine(FacePlayer());
            
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            Player = null;
            StartCoroutine(ReturnToStationary());
        }
    }


    #endregion
}
