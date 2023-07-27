/*************************************************************************
 * Author: MaKayla Elder
 * Date: 07.27.23
 * 
 * Description:
 * Generic Moving Enemy Controller for later iteration.
 * 
 */
using UnityEngine;


public class GenericEnemyController : EnemyController
{


    [Header("Enemy Colliders")]

    [SerializeField] protected Collider Head;
    [SerializeField] protected Collider Body;


    internal override void TakeDamage(Collision collision)
    {

        base.TakeDamage(collision);

        if (Head != null && Body != null) 
        {
            if (collision.collider == Head)
            {


            }
            else if (collision.collider == Body)
            {


            }
        
        
        }

    }



}