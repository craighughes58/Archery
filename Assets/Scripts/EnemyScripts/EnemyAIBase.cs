/*************************************************************************
 * Author: MaKayla Elder
 * Date: 06.11.2023
 * 
 * Description:
 * Base enemy AI class with base attributes and functionality to be iterated on in further classes.
 * 
 * 
 * 
 */

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIBase : MonoBehaviour
{

    #region Private Variables

    [Header("Patrol Settings")]

    [Tooltip("By default, the patrol will be begin at the first index and loop through each point in the array.")]
    [SerializeField]private Transform[] PatrolPoints;

    [Tooltip("Time in seconds until next movement.")]
    [SerializeField]private float PatrolDelay = 3;

    [Tooltip("Randomize patrol order?")]
    [SerializeField] private bool RandomPointOrder;

    [Tooltip("Randomize delay time?")]
    [SerializeField] private bool RandomDelayTime;

    [Tooltip("Check that Randomize Delay Time is set to true. Maximum idle time between patrol points.")]
    [SerializeField] private float RandomDelayMaxTime;

    private NavMeshAgent NavAgent;
    private int PatrolIndex = 0;

    private float NextTime;
    private Transform Destination;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        NavAgent.autoBraking = false;

        Destination = this.gameObject.transform;
        NextPatrolPoint();
    }

    // Update is called once per frame
    void Update()
    {
        //are we at the destination? have we completed our delay time?
        if (Time.time > NextTime && this.gameObject.transform.position == Destination.position)
        {
            NextPatrolPoint();

        }

    }

    void NextPatrolPoint() 
    {   

        if(PatrolPoints == null)
        {
            return;
        }

        
        if ( (PatrolIndex >  (PatrolPoints.Length - 1) )|| (PatrolIndex < 0) )
        {
            PatrolIndex = 0; 
        }

        if(RandomPointOrder == true)
        {
            PatrolIndex = Random.Range(0, PatrolPoints.Length);
        }

        float Duration = PatrolDelay;

        if (RandomDelayTime == true) 
        {
            Duration = Random.Range(0, RandomDelayMaxTime);        
        }

        NavAgent.SetDestination(PatrolPoints[PatrolIndex].transform.position);
         Destination.position = NavAgent.destination;
        PatrolIndex++;


        NextTime = Time.time + Duration;

        

    
    }
}
