using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyController : EnemyAIBase
{
    #region Serialized Variables
    [Header("Movement")]
    [Tooltip("How fast the enemy moves when they are going to patrol points")]
    [SerializeField] private float _PatrollingSpeed;

    [Tooltip("How fast the enemy moves when they are going to the player")]
    [SerializeField] private float _ChasingSpeed;


    #endregion
    #region Private Variables
    //The point where the enemy finds the player and departs from the patrol route
    private Vector3 _FinalPatrolPoint;
    #endregion

    #region Start and Update
    protected override void Start()
    {
        //base.Start();
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

    protected override void Update()
    {
        base.Update();
    }

    #endregion


    #region Movement
    //The enemy will have a list of points that it will patrol
    protected override void Patrolling()
    {
        //base.Patrolling();
        #region Safety catch for no Patrol Points set
        if (_PatrolRoute == null)
        {
            return;
        }
        #endregion

        #region Switch to chase if player visible
        if (_bPlayerVisible)
        {
            //perform the reset so it is clean for next use!
            _Timer.ResetTimer();
            _CurrentState = _EnemyStates.Chasing;
            return;
        }
        #endregion

        #region Setup Patrol Parameters
        if ((_PatrolIndex > (_PatrolRoute._Waypoints.Count - 1)) || (_PatrolIndex < 0))
        {
            _PatrolIndex = 0;
        }

        if (_PatrolType == _PatrolCategories.Roam)
        {
            _PatrolIndex = Random.Range(0, _PatrolRoute._Waypoints.Count - 1);
        }

        float Duration = _PatrolDelay;

        if (_bRandomDelayTime == true)
        {
            Duration = Random.Range(0, _RandomDelayMaxTime);
        }

        Vector3 PatrolDestination = _PatrolRoute._Waypoints[_PatrolIndex].transform.position;
        #endregion

        #region Patrol

        //check if we have reached the destination
        if (((_EnemyPosition - PatrolDestination).magnitude < .5))
        {
            //check if a timer is running and if it is completed
            if (_Timer.bHasTimerCompleted())
            {
                //Reset your timer or it will always return true for completed!!!
                _Timer.ResetTimer();//CHANGE ALL BELOW
                //move on if both destination is reached and timer is completed
                _PatrolIndex++;
                transform.position = Vector3.MoveTowards(_EnemyPosition,PatrolDestination,_PatrollingSpeed);
                transform.LookAt(PatrolDestination);
                /*                
                                _NavAgent.SetDestination(PatrolDestination);
                                _NavAgent.stoppingDistance = 0;*/
            }
            //get a timer going when needed
            else if (!_Timer.bHasTimerStarted())
            {
                //initial kick-off w/out a delay since default _NavAgent destination is itself
                //making a custom assertion because the average difference is outside of == assertion epsilon
                if ((_EnemyPosition - _StartPosition).magnitude < 1)//CHANGE
                {
                    // _NavAgent.SetDestination(PatrolDestination);
                    //_NavAgent.stoppingDistance = 0;
                    transform.position = Vector3.MoveTowards(_EnemyPosition, PatrolDestination, _PatrollingSpeed);
                    transform.LookAt(PatrolDestination);
                    _PatrolIndex++;
                    return;
                }

                _Timer.NewTimer(Duration);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(_EnemyPosition, PatrolDestination, _PatrollingSpeed);
            transform.LookAt(PatrolDestination);
        }
        #endregion
    }
    //The enemy will chase the player when in range 
    protected override void ChasePlayer(int BufferDistance)
    {
        //base.ChasePlayer(BufferDistance);
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
        //fix all below
        _NavAgent.stoppingDistance = BufferDistance;
        _NavAgent.SetDestination(_PlayerPosition);
        #endregion

    }
    #endregion

    #region Attacking
    #endregion

    #region Collisions and Triggers
    //if the enemy triggers from a player switch states to attack mode

    //if the player leaves the perception sphere switch back to patrol mode

    //if the enemy collides with the player then it self destructs 

    #endregion
}
