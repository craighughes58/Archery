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

using UnityEngine;
using UnityEngine.AI;


public class EnemyAIBase : MonoBehaviour
{

    #region Protected Variables

    #region Serialized Fields
    #region Attack Settings
    [Header("Attack Settings")]

    [Tooltip("What kind of damage should the enemy inflict?")]
    [SerializeField] protected _AttackCategories _AttackType;

    [Tooltip("This enemy's base damage to apply.")]
    [SerializeField] protected int _BaseDamage = 5;

    [Tooltip("The distance the enemy should stop at before reaching the player. This should change based on the enemy type.")]
    [SerializeField] protected int _AttackDistance = 3;
    #endregion

    #region Patrol Settings
    [Header("Patrol Settings")]

    [Tooltip("Set the patrol behavior type for this enemy")]
    [SerializeField] protected _PatrolCategories _PatrolType;

    [Tooltip("By default, the patrol will be begin at the first index and loop through each point in the array.")]
    [SerializeField]protected WaypointContainer _PatrolRoute;

    [Tooltip("Time in seconds until next movement.")]
    [SerializeField]protected float _PatrolDelay = 3;

   [SerializeField]
    protected float _RandomDelayMaxTime;

    #endregion

    #region Perception Settings
    [Header("Perception Settings")]

    [Tooltip("The forward distance the enemy can see to.")]
    [SerializeField] protected float _RangedPerceptionDistance;

    [Tooltip("The radial distance the enemy can see to.")]
    [SerializeField] protected float _GeneralPerceptionRadius;

    [Tooltip("How long should the enemy pursue when the player escapes sight?")]
    [SerializeField] protected float _ChaseTime = 0;
    #endregion
    #endregion

    #region Non-Serialized Fields

    #region Objects & Custom Classes
    protected NavMeshAgent _NavAgent;
    protected GameObject _Player;
    protected Timer _Timer;
    #endregion

    protected int _PatrolIndex = 0;

    #region Booleans
    protected bool _bShouldPatrol;
    protected bool _bShouldAttack;
    protected bool _bPlayerVisible;
    //must be made public so the custom inspector can read/write this value
    public bool _bRandomDelayTime;

    #endregion

    #region Vectors
    protected Vector3 _PlayerPosition;
    protected Vector3 _EnemyPosition;
    protected Vector3 _StartPosition;
    #endregion

   protected _EnemyStates _CurrentState;

    #region Enums
    protected enum _EnemyStates
        {
            Idle,
            Patrolling,
            Chasing,
            Attacking
        }
    
    protected enum _AttackCategories
    {
        Melee,
        Ranged,
        SelfDestruct
    }

    protected enum _PatrolCategories
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
    protected virtual void Start()
    {

        //initial setup for our movement component
        _NavAgent = GetComponent<NavMeshAgent>();
        _NavAgent.autoBraking = false;
        _NavAgent.speed = GetComponent<EnemyController>().speed;

        //grab the player
        _Player = GameObject.FindGameObjectWithTag("Player");
        _PlayerPosition = _Player.transform.position;

        //record initial values to compare
        _EnemyPosition = this.gameObject.transform.position;
        _StartPosition = this.gameObject.transform.position;

        //default state set
        _CurrentState = _EnemyStates.Idle;

        //REQUIRED for patrol and chase behaviors
        _Timer = gameObject.AddComponent<Timer>();
    }



    // Update is called once per frame
    protected virtual void Update()
    {
        //Makes sure our data is accurate for later checks
        UpdateVariables();

        //Enemy AI State Machine
        DecideState(_CurrentState);
  
    }
    /// <summary>
    /// A switch machine bringing us to the correct behavior based on the state information.
    /// </summary>
    /// <param name="CState"></param>
    protected void DecideState(_EnemyStates CState)
    {
        //the logic of switching to a new state itself (ex. CState = _EnemyStates.Patrolling)
        //occurs within other state functions as logic it becomes necessary.
        switch (CState)
        {
            case _EnemyStates.Idle:
                {
                    //make an initial selection to action or idle if no actions possible
                    Idle();
                    break;
                }
            case _EnemyStates.Patrolling:
                {
                    //all timer logic to perform the patrol and switching out of this case exists therein
                    Patrolling();
                    break;
                }
            case _EnemyStates.Chasing:
                {
                    ChasePlayer(_AttackDistance);
                    break;
                }
            case _EnemyStates.Attacking:
                {
                    AttackPlayer(_AttackType);
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
    protected virtual void Idle()
    {
        #region Determine our factors
        //expression simplification bools
        _bShouldPatrol = (_PatrolType == _PatrolCategories.Patrol) || (_PatrolType == _PatrolCategories.Roam) && _PatrolRoute != null && _CurrentState == _EnemyStates.Idle;
        #endregion

        #region Set a state (Chase, Patrol, or continue Idling)
        if (_bPlayerVisible && _PatrolType != _PatrolCategories.StaticEnemy)
        {
            _CurrentState = _EnemyStates.Chasing;
        }
        else if (_bPlayerVisible && _PatrolType == _PatrolCategories.StaticEnemy)
        {
            _CurrentState = _EnemyStates.Attacking;
        }
        
       else if (_bShouldPatrol)
        {
            _CurrentState = _EnemyStates.Patrolling;
        }
        #endregion

    }

    /// <summary>
    /// Checks if the player is visible within long or close range.
    /// Long range detection TBD.
    /// </summary>
    /// <returns></returns>
    protected virtual bool IsPlayerVisible()
    {
        #region Check Player Exists
        //no player, no cast!
        if (_Player == null)
        {
            return false;
        }
        #endregion

        #region Perform Raycast
        RaycastHit Hit;
        Vector3 Direction = (_PlayerPosition - _EnemyPosition).normalized;
        Physics.Raycast(this.transform.position, Direction, out Hit, _RangedPerceptionDistance);
        #endregion

        #region Return if no hit
        if (Hit.collider == null)
        {
            return false;
        }
        #endregion

        #region Process hit on player

        //first process for Static Enemies since it is less restrictive
        if(_PatrolType == _PatrolCategories.StaticEnemy
            && (Hit.collider == _Player.GetComponent<Collider>())

            && (Hit.collider != this.gameObject.GetComponent<Collider>())

            && (Hit.distance <= _RangedPerceptionDistance))
                 {
                      return true;
                 }
        //process hit for all other enemy types
       else if ( 
            (Hit.collider == _Player.GetComponent<Collider>())

            && (Hit.collider != this.gameObject.GetComponent<Collider>())

            && (Hit.distance <= _RangedPerceptionDistance)

            )
        {
            //we want the cross product because we can use other information in this vector to affect our rotation later on
            Vector3 CrossProduct = Vector3.Cross(_EnemyPosition.normalized, _PlayerPosition.normalized);

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
            else if (Hit.distance <= _GeneralPerceptionRadius)
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
    protected virtual void Patrolling() 
    {
        #region Safety catch for no Patrol Points set
        if (_PatrolRoute == null)
        {
            return;
        }
        #endregion

        #region Switch to chase if player visible
        if(_bPlayerVisible)
        {
            //perform the reset so it is clean for next use!
            _Timer.ResetTimer();
            _CurrentState = _EnemyStates.Chasing;
            return;
        }
        #endregion

        #region Setup Patrol Parameters
        if ( (_PatrolIndex >  (_PatrolRoute._Waypoints.Count - 1) )|| (_PatrolIndex < 0) )
        {
            _PatrolIndex = 0; 
        }

        if(_PatrolType == _PatrolCategories.Roam)
        {
            _PatrolIndex = Random.Range(0, _PatrolRoute._Waypoints.Count - 1);
        }

        float Duration = _PatrolDelay;

        if (_bRandomDelayTime == true) 
        {
            Debug.Log("using random delay");
            Duration = Random.Range(0, _RandomDelayMaxTime);        
        }

        Vector3 PatrolDestination = _PatrolRoute._Waypoints[_PatrolIndex].transform.position;
        #endregion

        #region Patrol
        
        //check if we have reached the destination
         if ((_NavAgent.remainingDistance < .5))
        {        
            //check if a timer is running and if it is completed
            if (_Timer.bHasTimerCompleted())
            {
                //Reset your timer or it will always return true for completed!!!
                _Timer.ResetTimer();
                //move on if both destination is reached and timer is completed
                _NavAgent.SetDestination(PatrolDestination);
                _NavAgent.stoppingDistance = 0;
                _PatrolIndex++;
            }
            //get a timer going when needed
            else if (!_Timer.bHasTimerStarted())
            {
                //initial kick-off w/out a delay since default _NavAgent destination is itself
                //making a custom assertion because the average difference is outside of == assertion epsilon
                if ((_EnemyPosition - _StartPosition).magnitude < 1)
                {
                    _NavAgent.SetDestination(PatrolDestination);
                    _NavAgent.stoppingDistance = 0;
                    _PatrolIndex++;
                    return;
                }

                _Timer.NewTimer(Duration);
            }
        }
        #endregion
    }

    /// <summary>
    /// Handles chase behavior for the enemy.
    /// </summary>
    /// <param name="BufferDistance"></param>
    protected virtual void ChasePlayer(int BufferDistance)
    {
        #region Chase Timer & visibility check
        //stop the countdown if player returns to view
        if (_bPlayerVisible)
        {
            if (_Timer.bHasTimerStarted())
            {
                _Timer.ResetTimer();
            }
        }       
        //check that the player is still in range
        else if (!_bPlayerVisible)
        {
            //abandon chasing if Chase Time has completed
            if (_Timer.bHasTimerCompleted())
            {
                //do not set isStopped property to true, this will require an explicit call again to reverse to default state, isStopped=false
                _NavAgent.destination = this.transform.position;
                _CurrentState = _EnemyStates.Idle;
                //reset the timer for clean use!
                _Timer.ResetTimer();

                return;
            }
            //begin chase timer if none going
           else if (!_Timer.bHasTimerStarted())
            {
                _Timer.NewTimer(_ChaseTime);
            }
        }
        #endregion

        #region Check if we are in range to be Attacking
        _bShouldAttack = (_CurrentState == _EnemyStates.Chasing) && _NavAgent.remainingDistance <= BufferDistance;
        if (_bShouldAttack)
        {
            _CurrentState = _EnemyStates.Attacking;
        }
        #endregion

        #region CHASE that player!
        _NavAgent.stoppingDistance = BufferDistance;
        _NavAgent.SetDestination(_PlayerPosition);
        #endregion
    }

    /// <summary>
    /// Handles attack behavior for the enemy.
    /// </summary>
    /// <param name="CurrentAttackType"></param>
    protected virtual void AttackPlayer(_AttackCategories CurrentAttackType)
    {
    
        #region Check Player Can Be Attacked
        bool bShouldChase = 
            (_CurrentState == _EnemyStates.Attacking) 
            && Vector3.Distance(_EnemyPosition, _PlayerPosition) > _AttackDistance
            && _PatrolType != _PatrolCategories.StaticEnemy;
        if (bShouldChase)
        {
            //let the chasing state revert us to idle if the player is lost
            _CurrentState = _EnemyStates.Chasing;
        }
        //make another check in the case of a static enemy
        bool bStaticEnemyIdle = 
            (_CurrentState == _EnemyStates.Attacking) 
            && Vector3.Distance(_EnemyPosition, _PlayerPosition) > _RangedPerceptionDistance
            && _PatrolType == _PatrolCategories.StaticEnemy;
        if (bStaticEnemyIdle)
        {
            _CurrentState = _EnemyStates.Idle;
        }

            #endregion

            #region Process the Enemy's Attack Type
            switch (CurrentAttackType)
        {
            case _AttackCategories.Melee:
                {
                    Melee();
                    break;
                }

            case _AttackCategories.Ranged:
                {
                    Ranged();
                    break;
                }
            case _AttackCategories.SelfDestruct:
                {
                    SelfDestruct();
                    break;
                }
        }
        #endregion
    }

    //Override me in your creature!
    protected virtual void Melee()
    {


    }
    //Override me in your creature!
    protected virtual void Ranged()
    {

    }
    //Override me in your creature!
    protected virtual void SelfDestruct()
    {
        _NavAgent.stoppingDistance = 0;
        //OnCollisionEnter will handle the rest of inflicting damage in this case
    }

    /// <summary>
    /// Handles the collision processing for the Enemy. Handles final Self-Destruct processes.
    /// </summary>
    /// <param name="collision"></param>
    protected virtual void OnCollisionEnter(Collision collision)
    {
        #region Safety Catch If No Collider
        if (collision.collider == null) return;
        #endregion

        #region Self Destruct Processing
        if (collision.collider == _Player.GetComponent<Collider>())
        {
            if (_AttackType == _AttackCategories.SelfDestruct)
            {
                _Player.GetComponent<Health>().damage(_BaseDamage);
                Destroy(gameObject);
            }
        }
        #endregion
    }

    /// <summary>
    /// Place any class scope variables w/ their checks here for per frame updating.
    /// </summary>
    protected virtual void UpdateVariables()
    {
        _EnemyPosition = this.transform.position;
        _PlayerPosition = _Player.transform.position;
        _bPlayerVisible = IsPlayerVisible();
    }
}
