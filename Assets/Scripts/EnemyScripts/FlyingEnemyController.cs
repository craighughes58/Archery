using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyController : EnemyAIBase
{
    #region Serialized Variables

    #endregion
    #region Private Variables
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

    #endregion

    #region Attacking
    #endregion

    #region Collisions
    //if the enemy runs into a player switch states to attack mode

    //if the player leaves the perception sphere switch back to patrol mode
    #endregion
}
