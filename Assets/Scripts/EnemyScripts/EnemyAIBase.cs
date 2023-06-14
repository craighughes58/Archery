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
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.HID;

public class EnemyAIBase : MonoBehaviour
{

    #region Private Variables

    [Header("Patrol Settings")]

    [Tooltip("Should this enemy be patrolling?")]
    [SerializeField] private bool Patrol = true;

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

    [Header("Perception Settings")]

    [Tooltip("The radial distance the enemy can see to.")]
    [SerializeField] private float GeneralPerceptionRadius;

    [Tooltip("The distance the enemy should stop at before reaching the player. This should change based on the enemy type.")]
    [SerializeField] private int AttackDistance;

    private NavMeshAgent NavAgent;
    private int PatrolIndex = 0;

    private float PatrolTimer;

    private bool bShouldPatrol;
    private bool bShouldAttack;
    private bool bPlayerVisible;
   

    private EnemyController EnemyController;

    private GameObject Player;
    private Vector3 PlayerPosition;

    private Vector3 EnemyPosition;

    private EnemyStates CurrentState;
    private enum EnemyStates
        {
            Idle,
            Patrolling,
            Chasing,
            Attacking
        }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        EnemyController = GetComponent<EnemyController>();

        NavAgent = GetComponent<NavMeshAgent>();
        NavAgent.autoBraking = false;
        NavAgent.speed = EnemyController.speed;

        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerPosition = Player.gameObject.transform.position;

        EnemyPosition = this.gameObject.transform.position;

        CurrentState = EnemyStates.Idle;


        DecideState(CurrentState);



    }


    // Update is called once per frame
    void Update()
    {
        EnemyPosition = this.gameObject.transform.position;

        //Enemy AI State Machine
        DecideState(CurrentState);




    }
    private void DecideState(EnemyStates CState)
    {
        //expression simplification bools
        bShouldPatrol = Patrol || PatrolPoints != null && CurrentState == EnemyStates.Idle;
        bShouldAttack = CurrentState == EnemyStates.Chasing && NavAgent.remainingDistance < .5;
        bPlayerVisible = IsPlayerVisible(GeneralPerceptionRadius);


        if (bShouldPatrol)
        {
            
            CurrentState = EnemyStates.Patrolling;
        }
        if (bPlayerVisible)
        {

            Patrol = false;
            CurrentState = EnemyStates.Chasing;
        }
       
        if (bShouldAttack)
        {
            CurrentState = EnemyStates.Attacking;
        }

        //return to patrol or idle if player is lost
        if(!bPlayerVisible)
        {

        }


        switch (CState)
        {
            case EnemyStates.Idle:
                {
                    break;
                }
            case EnemyStates.Patrolling:
                {
                    if (NavAgent.remainingDistance < .5)
                    {
                        if (PatrolTimer == 0 || PatrolTimer < 0)
                        {
                            NextPatrolPoint();
                        }
                        else
                        {
                            PatrolTimer -= 1 * Time.deltaTime;
                        }
                    }
                    break;
                }
            case EnemyStates.Chasing:
                {
                    ChasePlayer(AttackDistance);
                    break;
                }
            case EnemyStates.Attacking:
                {
                    break;
                }
            default:
                {
                    break;
                }


        }
    }
    private bool IsPlayerVisible(float Radius)
    {
        if (Player == null)
        {
            return false;
        }
        NavMeshHit Hit;

        NavAgent.Raycast(PlayerPosition, out Hit);
        if (Hit.hit && Hit.distance <= GeneralPerceptionRadius)
        {
            return Hit.hit;
        }
        return false;
    }
    private void NextPatrolPoint() 
    {   
        if(PatrolPoints == null)
        {
            return;
        }
       Debug.Log("Patrolling");

        if ( (PatrolIndex >  (PatrolPoints.Length - 1) )|| (PatrolIndex < 0) )
        {
            PatrolIndex = 0; 
        }

        if(RandomPointOrder == true)
        {
            PatrolIndex = Random.Range(0, PatrolPoints.Length - 1);
        }

        float Duration = PatrolDelay;

        if (RandomDelayTime == true) 
        {
            Duration = Random.Range(0, RandomDelayMaxTime);        
        }

       NavAgent.SetDestination(PatrolPoints[PatrolIndex].transform.position);
        NavAgent.stoppingDistance = 0;

        PatrolIndex++;

        PatrolTimer = Duration;
    }

    private void ChasePlayer(int BufferDistance)
    {
        

        Debug.Log("Chasing");
        NavAgent.stoppingDistance = BufferDistance;
        NavAgent.SetDestination(PlayerPosition);
    }
   
    private void AttackPlayer()
    {

    }


}
