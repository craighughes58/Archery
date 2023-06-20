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

    [Tooltip("The reference to the transform of the top piece of the turret")]
    [SerializeField] private Transform SkullTransform;

    [Tooltip("The reference to the transform of the bottom piece of the turret")]
    [SerializeField] private Transform JawTransform;


    #endregion

    #region Private variables
    //active reference to the player
    private Transform Player; 
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        SphereCol.radius = perceptionRadius;
        Player = null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Movement

    private void FacePlayer()
    {

    }
    #endregion

    #region Attacking 

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
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    private bool confirmPlayer()
    {
        return true;
    }
    #endregion
}
