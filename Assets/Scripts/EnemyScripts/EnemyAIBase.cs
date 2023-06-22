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

    #region Serialized Fields
    #region Attack Settings
    [Header("Attack Settings")]

    [Tooltip("What kind of damage should the enemy inflict?")]
    [SerializeField] private AttackCategories AttackType;

    [Tooltip("This enemy's base damage to apply.")]
    [SerializeField] private int BaseDamage = 5;

    [Tooltip("The distance the enemy should stop at before reaching the player. This should change based on the enemy type.")]
    [SerializeField] private int AttackDistance = 3;
    #endregion

    #region Patrol Settings
    [Header("Patrol Settings")]

    [Tooltip("Set the patrol behavior type for this enemy")]
    [SerializeField] private PatrolCategories PatrolType;

    [Tooltip("By default, the patrol will be begin at the first index and loop through each point in the array.")]
    [SerializeField]private Transform[] PatrolPoints;

    [Tooltip("Time in seconds until next movement.")]
    [SerializeField]private float PatrolDelay = 3;

    //[Tooltip("Randomize delay time?")]
    [SerializeField] 
    private bool RandomDelayTime;
    // [Tooltip("Check that Randomize Delay Time is set to true. Maximum idle time between patrol points.")]
    [SerializeField]
    private float RandomDelayMaxTime;

    #endregion

    #region Perception Settings
    [Header("Perception Settings")]

    [Tooltip("The forward distance the enemy can see to.")]
    [SerializeField] private float RangedPerceptionDistance;

    [Tooltip("The radial distance the enemy can see to.")]
    [SerializeField] private float GeneralPerceptionRadius;

    [Tooltip("How long should the enemy pursue when the player escapes?")]
    [SerializeField] private float ChaseTime = 0;
    #endregion
    #endregion

    #region Non-Serialized Fields

    #region Objects & Custom Classes
    private NavMeshAgent NavAgent;
    private GameObject Player;
    private Timer Timer;
    #endregion

    private int PatrolIndex = 0;

    #region Booleans
    private bool bShouldPatrol;
    private bool bShouldAttack;
    private bool bPlayerVisible;
    #endregion

    #region Vectors
    private Vector3 PlayerPosition;
    private Vector3 EnemyPosition;
    private Vector3 StartPosition;
    #endregion

    private EnemyStates CurrentState;

    #region Enums
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

    private enum PatrolCategories
    {
        StaticEnemy,
        None,
        Patrol,
        Roam
    }
    #endregion

    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        //initial setup for our movement component
        NavAgent = GetComponent<NavMeshAgent>();
        NavAgent.autoBraking = false;
        NavAgent.speed = GetComponent<EnemyController>().speed;

        //grab the player
        Player = GameObject.FindGameObjectWithTag("Player");
        PlayerPosition = Player.transform.position;

        //record initial values to compare
        EnemyPosition = this.gameObject.transform.position;
        StartPosition = this.gameObject.transform.position;

        //default state set
        CurrentState = EnemyStates.Idle;

        //REQUIRED for patrol and chase behaviors
        Timer = gameObject.AddComponent<Timer>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
       
        Gizmos.DrawRay(EnemyPosition, this.transform.forward * RangedPerceptionDistance);
    }


    // Update is called once per frame
    void Update()
    {
        //Makes sure our data is accurate for later checks
        UpdateVariables();

        //Enemy AI State Machine
        DecideState(CurrentState);
  
    }
    /// <summary>
    /// A switch machine bringing us to the correct behavior based on the state information.
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
                    //all timer logic to perform the patrol and switching out of this case exists therein
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
                    AttackPlayer(AttackType);
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    /// <summary>
    /// Handles initial state changes when enemy become idle.
    /// Changes pending to reflect roaming system
    /// </summary>
    private void Idle()
    {
        #region Determine our factors
        //expression simplification bools
        bShouldPatrol = (PatrolType == PatrolCategories.Patrol) || (PatrolType == PatrolCategories.Roam) && PatrolPoints != null && CurrentState == EnemyStates.Idle;
        #endregion

        #region Set a state (Chase, Patrol, or continue Idling)
        if (bPlayerVisible && PatrolType != PatrolCategories.StaticEnemy)
        {
            CurrentState = EnemyStates.Chasing;
        }
        else if (bPlayerVisible && PatrolType == PatrolCategories.StaticEnemy)
        {
            CurrentState = EnemyStates.Attacking;
        }
        
       else if (bShouldPatrol)
        {
            CurrentState = EnemyStates.Patrolling;
        }
        #endregion

    }

    /// <summary>
    /// Checks if the player is visible within long or close range.
    /// Long range detection TBD.
    /// </summary>
    /// <returns></returns>
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
        Vector3 Direction = (PlayerPosition - EnemyPosition).normalized;
        Physics.Raycast(this.transform.position, Direction, out Hit, RangedPerceptionDistance);
        #endregion

        #region Return if no hit
        if (Hit.collider == null)
        {
            return false;
        }
        #endregion

        #region Process hit on player

        //first process for Static Enemies since it is less restrictive
        if(PatrolType == PatrolCategories.StaticEnemy
            && (Hit.collider == Player.GetComponent<Collider>())

            && (Hit.collider != this.gameObject.GetComponent<Collider>())

            && (Hit.distance <= RangedPerceptionDistance))
                 {
                      return true;
                 }
        //process hit for all other enemy types
       else if ( 
            (Hit.collider == Player.GetComponent<Collider>())

            && (Hit.collider != this.gameObject.GetComponent<Collider>())

            && (Hit.distance <= RangedPerceptionDistance)

            )
        {
            //we want the cross product because we can use other information in this vector to affect our rotation later on
            Vector3 CrossProduct = Vector3.Cross(EnemyPosition.normalized, PlayerPosition.normalized);

            #region Rotational Checks (Currently COMMENTED and UNUSED)
            //future rotational checks if desired
            //in this case a negative Y indicates an object to the right of the requesting object
            /* if (CrossProduct.y > 0.2)
             {
                 Debug.Log("Player is to the left of the enemy!");
             }
             else if (CrossProduct.y < -0.2)
             {
                 Debug.Log("Player is to the right of the enemy!");

             }
             else
             {
                 Debug.Log("Player is in line with the enemy!");

             }*/
            #endregion

            #region Visibility Range Check
            //if the x vector is positive, we are in front of the enemy
            if (CrossProduct.x > 0)
            {
                return true;
            }
            //if we are behind the enemy, we should still be able to detect the player if close enough
            else if (Hit.distance <= GeneralPerceptionRadius)
            {
                return true;
            }
            else { return false; }

            #endregion

        }
        #endregion

        #region Return false if not in range
        else
        {
            return false;
        }
        #endregion
    }

    /// <summary>
    /// Handles patrol behavior for the enemy.
    /// </summary>
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

        if(PatrolType == PatrolCategories.Roam)
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
            //get a timer going when needed
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

    /// <summary>
    /// Handles chase behavior for the enemy.
    /// </summary>
    /// <param name="BufferDistance"></param>
    private void ChasePlayer(int BufferDistance)
    {
        #region Chase Timer & visibility check
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

        #region Check if we are in range to be Attacking
        bShouldAttack = (CurrentState == EnemyStates.Chasing) && NavAgent.remainingDistance <= BufferDistance;
        if (bShouldAttack)
        {
            CurrentState = EnemyStates.Attacking;
        }
        #endregion

        #region CHASE that player!
        NavAgent.stoppingDistance = BufferDistance;
        NavAgent.SetDestination(PlayerPosition);
        #endregion
    }

    /// <summary>
    /// Handles attack behavior for the enemy.
    /// </summary>
    /// <param name="CurrentAttackType"></param>
    private void AttackPlayer(AttackCategories CurrentAttackType)
    {
    
        #region Check Player Can Be Attacked
        bool bShouldChase = 
            (CurrentState == EnemyStates.Attacking) 
            && Vector3.Distance(EnemyPosition, PlayerPosition) > AttackDistance
            && PatrolType != PatrolCategories.StaticEnemy;
        if (bShouldChase)
        {
            //let the chasing state revert us to idle if the player is lost
            CurrentState = EnemyStates.Chasing;
        }
        //make another check in the case of a static enemy
        bool bStaticEnemyIdle = 
            (CurrentState == EnemyStates.Attacking) 
            && Vector3.Distance(EnemyPosition, PlayerPosition) > RangedPerceptionDistance
            && PatrolType == PatrolCategories.StaticEnemy;
        if (bStaticEnemyIdle)
        {
            CurrentState = EnemyStates.Idle;
        }

            #endregion

            #region Process the Enemy's Attack Type
            switch (CurrentAttackType)
        {
            case AttackCategories.Melee:
                {
                    Melee();
                    break;
                }

            case AttackCategories.Ranged:
                {
                    Ranged();
                    break;
                }
            case AttackCategories.SelfDestruct:
                {
                    SelfDestruct();
                    break;
                }
        }
        #endregion
    }

    //Override me in your creature!
    internal virtual void Melee()
    {


    }
    //Override me in your creature!
    internal virtual void Ranged()
    {

    }
    //Override me in your creature!
    internal virtual void SelfDestruct()
    {
        NavAgent.stoppingDistance = 0;
        //OnCollisionEnter will handle the rest of inflicting damage in this case
    }

    /// <summary>
    /// Handles the collision processing for the Enemy. Handles final Self-Destruct processes.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        #region Safety Catch If No Collider
        if (collision.collider == null) return;
        #endregion

        #region Self Destruct Processing
        if (collision.collider == Player.GetComponent<Collider>())
        {
            if (AttackType == AttackCategories.SelfDestruct)
            {
                Player.GetComponent<Health>().damage(BaseDamage);
                Destroy(gameObject);
            }
        }
        #endregion
    }

    /// <summary>
    /// Place any class scope variables w/ their checks here for per frame updating.
    /// </summary>
    private void UpdateVariables()
    {
        EnemyPosition = this.transform.position;
        PlayerPosition = Player.transform.position;
        bPlayerVisible = IsPlayerVisible();
    }

    //this is WIP. Idea is to make random max delay time float field only available if random delay time bool has been set to true
    //
    #region CustomEditor
#if UNITY_EDITOR
    [CustomEditor(typeof(EnemyAIBase))]
    [CanEditMultipleObjects]
    public class EnemyAIBaseEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EnemyAIBase enemyAIBase = (EnemyAIBase)target;

            if (enemyAIBase.RandomDelayTime)
            {
               // EditorGUILayout.FloatField("Maximum random delay time", enemyAIBase.RandomDelayMaxTime);
            }

            /*using (new EditorGUI.DisabledScope(enemyAIBase.RandomDelayTime == false))
            {
                enemyAIBase.RandomDelayMaxTime = EditorGUILayout.FloatField("Maximum random delay time", enemyAIBase.RandomDelayMaxTime);
            }*/
        }
    }
#endif
    #endregion
}
