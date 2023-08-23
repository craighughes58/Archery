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

    

   

}