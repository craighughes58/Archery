/*************************************************************************
 * Author: MaKayla Elder
 * Date: 07.27.23
 * 
 * Description:
 * Generic Moving Enemy AI for later iteration.
 * 
 */


using UnityEngine;
using UnityEngine.AI;

public class GenericMovingEnemyAI : EnemyAIBase
{
    [Tooltip("Dictates the initial upward aim of the arrow.")]
    [SerializeField] protected float _YOffset = 0.75f;
    [Tooltip("The rate at which the enemy should turn to face the player.")]
    [SerializeField] protected float _RotationSpeed = 2;

    [SerializeField] protected Rigidbody _RigidBody;

    protected override void AttackPlayer(_AttackCategories CurrentAttackType)
    {
       
        FacePlayer();

        Vector3 PlayerDirection = _EnemyPosition - _PlayerPosition;
        float AngleToPlayer = Vector3.Angle(PlayerDirection, gameObject.transform.forward);


       if(AngleToPlayer >= 175)
        {
            base.AttackPlayer(CurrentAttackType);
        }


    }

    protected override void Start()
    {
        base.Start();

        _RigidBody = GetComponent<Rigidbody>();
    }
    protected override void Ranged()
    {
        base.Ranged();

        if (_Weapon != null)
        {
            EnemyBow Bow = _Weapon.GetComponentInChildren<EnemyBow>();

            if (Bow != null)
            {
                Bow.Fire();
            }
        }
        
        }

    protected void FacePlayer()
    {
        //create new rotate closer to the player
        Quaternion TargetRotation = Quaternion.LookRotation(_PlayerPosition + new Vector3(0, _YOffset, 0) - transform.position);
        //set current rotation to new rotation
        _RigidBody.MoveRotation(Quaternion.RotateTowards(transform.rotation, TargetRotation, _RotationSpeed));

    }

   

}