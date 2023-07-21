using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyController : EnemyAIBase
{
    #region Serialized Variables

    #endregion
    #region Private Variables
    //The point where the enemy finds the player and departs from the patrol route
    private Vector3 _FinalPatrolPoint;
    #endregion

    #region Start and Update
    protected override void Start()
    {
        base.Start();
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
    }
    //The enemy will chase the player when in range 
    protected override void ChasePlayer(int BufferDistance)
    {
        //base.ChasePlayer(BufferDistance);
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
