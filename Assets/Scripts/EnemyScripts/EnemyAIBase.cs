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
using UnityEngine.Assertions;
using UnityEngine.InputSystem.HID;

public class EnemyAIBase : MonoBehaviour
{

    #region Private Variables
    [Header("Attack Settings")]

    [Tooltip("What kind of damage should the enemy inflict?")]
    [SerializeField] private AttackCategories AttackType;

    [Tooltip("This enemy's base damage to apply.")]
    [SerializeField] private int BaseDamage = 5;

    [Header("Patrol Settings")]

    [Tooltip("Should this enemy be assigned a patrol?")]
    [SerializeField] private bool ShouldPatrol;

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
    [SerializeField] private int AttackDistance = 3;

    [Tooltip("How long should the enemy pursue when the player escapes?")]
    [SerializeField] private float ChaseTime = 0;

    private NavMeshAgent NavAgent;
    private int PatrolIndex = 0;

    private Timer Timer;

    private bool bShouldPatrol;
    private bool bShouldAttack;
    private bool bPlayerVisible;
   

    private EnemyController EnemyController;

    private GameObject Player;
    private Vector3 PlayerPosition;

    private Vector3 EnemyPosition;
    private Vector3 StartPosition;

    private EnemyStates CurrentState;
    private enum EnemyStates
        {
            Idle,
            Patrolling,
            Chasing,
            Attacking
        }
    
    private enum AttackCategories
    {
        Melee,
        Ranged,
        SelfDestruct
    }

    #endregion

    EnemyAIBase(AttackCategories AttackType)
    {
        this.AttackType = AttackType;
    }
   

    // Start is called before the first frame update
    void Start()
    {

        EnemyController = GetComponent<EnemyController>();

        NavAgent = GetComponent<NavMeshAgent>();
        NavAgent.autoBraking = false;
        NavAgent.speed = EnemyController.speed;

        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerPosition = Player.transform.position;

        EnemyPosition = this.gameObject.transform.position;
        StartPosition = this.gameObject.transform.position;

        CurrentState = EnemyStates.Idle;

        Timer = gameObject.AddComponent<Timer>();
       
       
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(EnemyPosition, GeneralPerceptionRadius);
    }*/


    // Update is called once per frame
    void Update()
    {
        EnemyPosition = this.transform.position;
        PlayerPosition = Player.transform.position;
        bPlayerVisible = IsPlayerVisible();

        //Enemy AI State Machine
        DecideState(CurrentState);
   

    }
    /// <summary>
    /// function to handle what action an enemy should take next
    /// </summary>
    /// <param name="CState"></param>
    private void DecideState(EnemyStates CState)
    {
        //the logic of switching to a new state itself (ex. CState = EnemyStates.Patrolling)
        //occurs within other state functions as logic it becomes necessary.
        switch (CState)
        {
            case EnemyStates.Idle:
                {
                    //make an initial selection to action or idle if no actions possible
                    Idle();
                    break;
                }
            case EnemyStates.Patrolling:
                {
                    //all timer logic and switching out of this case exists therein
                    Patrolling();
                    break;
                }
            case EnemyStates.Chasing:
                {
                    ChasePlayer(AttackDistance);
                    break;
                }
            case EnemyStates.Attacking:
                {
                    Debug.Log("attacking");
                    AttackPlayer(AttackType);
                    break;
                }
            default:
                {
                    break;
                }


        }
    }

    private void Idle()
    {
        #region Determine our factors
        //expression simplification bools
        bShouldPatrol = ShouldPatrol && PatrolPoints != null && CurrentState == EnemyStates.Idle;
        #endregion

        #region Set a state (Chase, Patrol, or continue Idling)
        if (bPlayerVisible)
        {
            CurrentState = EnemyStates.Chasing;
        }
        
       else if (bShouldPatrol)
        {
            CurrentState = EnemyStates.Patrolling;
        }
        #endregion

    }
    private bool IsPlayerVisible()
    {
        #region Check Player Exists
        //no player, no cast!
        if (Player == null)
        {
            return false;
        }
        #endregion

        #region Perform Raycast
        RaycastHit Hit;

        Vector3 Direction = PlayerPosition - EnemyPosition;

        Physics.Raycast(this.transform.position, Direction, out Hit, GeneralPerceptionRadius);
        #endregion

        #region Return if no hit
        if (Hit.collider == null)
        {
            return false;
        }
        #endregion

        #region Return true if hit is player
        if ( 
            (Hit.collider == Player.GetComponent<Collider>())

            && (Hit.collider != this.gameObject.GetComponent<Collider>())

            && (Hit.distance <= GeneralPerceptionRadius)

            )
        {
            return true;
        }
        #endregion

        #region Return false if not the player
        else
        {
            return false;
        }
        #endregion
    }
    private void Patrolling() 
    {
        #region Safety catch for no Patrol Points set
        if (PatrolPoints == null)
        {
            return;
        }
        #endregion

        #region Switch to chase if player visible
        if(bPlayerVisible)
        {
            //perform the reset so it is clean for next use!
            Timer.ResetTimer();
            CurrentState = EnemyStates.Chasing;
            return;
        }
        #endregion

        #region Setup Patrol Parameters
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

        Vector3 PatrolDestination = PatrolPoints[PatrolIndex].transform.position;
        #endregion

        #region Patrol
        
        //check if we have reached the destination
         if ((NavAgent.remainingDistance < .5))
        {        
            //check if a timer is running and if it is completed
            if (Timer.bHasTimerCompleted())
            {
                //Reset your timer or it will always return true for completed!!!
                Timer.ResetTimer();
                //move on if both destination is reached and timer is completed
                NavAgent.SetDestination(PatrolDestination);
                NavAgent.stoppingDistance = 0;
                PatrolIndex++;
            }
            else if (!Timer.bHasTimerStarted())
            {
                //initial kick-off w/out a delay since default NavAgent destination is itself
                //making a custom assertion because the average difference is outside of == assertion epsilon
                if ((EnemyPosition - StartPosition).magnitude < 1)
                {
                    NavAgent.SetDestination(PatrolDestination);
                    NavAgent.stoppingDistance = 0;
                    PatrolIndex++;
                    return;
                }

                Timer.NewTimer(Duration);
            }
        }
        #endregion
    }
    private void ChasePlayer(int BufferDistance)
    {
        #region Chase Timer
        //stop the countdown if player returns to view
        if (bPlayerVisible)
        {
            if (Timer.bHasTimerStarted())
            {
                Timer.ResetTimer();
            }
        }       
        //check that the player is still in range
        else if (!bPlayerVisible)
        {
            //abandon chasing if Chase Time has completed
            if (Timer.bHasTimerCompleted())
            {
                //do not set isStopped property to true, this will require an explicit call again to reverse to default state, isStopped=false
                NavAgent.destination = this.transform.position;
                CurrentState = EnemyStates.Idle;
                //reset the timer for clean use!
                Timer.ResetTimer();

                return;
            }
            //begin chase timer if none going
           else if (!Timer.bHasTimerStarted())
            {
                Timer.NewTimer(ChaseTime);
            }
        }
        #endregion

        bShouldAttack = (CurrentState == EnemyStates.Chasing) && NavAgent.remainingDistance <= BufferDistance;
        if (bShouldAttack)
        {
            CurrentState = EnemyStates.Attacking;
        }

        NavAgent.stoppingDistance = BufferDistance;
        NavAgent.SetDestination(PlayerPosition);
    }
   
    private void AttackPlayer(AttackCategories CurrentAttackType)
    {
       bool bShouldChase = (CurrentState == EnemyStates.Attacking) && Vector3.Distance(EnemyPosition, PlayerPosition) > AttackDistance;
        if (bShouldChase)
        {
            CurrentState = EnemyStates.Chasing;
        }

        switch (CurrentAttackType)
        {
            case AttackCategories.Melee:
                {

                    //Melee method to be added later
                    break;
                }

            case AttackCategories.Ranged:
                {
                    //Ranged method to be added later
                    break;
                }
            case AttackCategories.SelfDestruct:
                {
                    NavAgent.stoppingDistance = 0;
                    //OnCollisionEnter will handle the rest of inflicting damage in this case
                    break;
                }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        #region Safety Catch If No Collider
        if (collision.collider == null) return;
        #endregion

        if (collision.collider == Player.GetComponent<Collider>())
        {
            if (AttackType == AttackCategories.SelfDestruct)
            {
                Player.GetComponent<Health>().damage(BaseDamage);
                Destroy(gameObject);
            }
        }
        
        
      
    }
}
