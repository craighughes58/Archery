/*************************************************************************
 * Author: MaKayla Elder
 * Date: 07.27.23
 * 
 * Description:
 * Generic Moving Enemy AI for later iteration.
 * 
 */


using UnityEngine;


public class GenericMovingEnemyAI : EnemyAIBase
{
    [Tooltip("Dictates the initial upward aim of the arrow.")]
    [SerializeField] protected float _YOffset = 0.75f;
    [Tooltip("The rate at which the enemy should turn to face the player.")]
    [SerializeField] protected float _RotationSpeed = 2;

    protected Rigidbody _RigidBody;

    protected override void AttackPlayer(_AttackCategories CurrentAttackType)
    {
       
        FacePlayer();
       if((_PlayerCrossProduct.y > -0.8) && (_PlayerCrossProduct.y < .8) && (_PlayerCrossProduct.x > 0))
        {
            Debug.Log("moving to firing phase");
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