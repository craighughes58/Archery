using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponBehaviorBase : WeaponBehaviorBase
{
    #region Private Variables
    //
   internal EnemyAIBase _EnemyAI;


    #endregion


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    internal virtual void OnEnemyParenInitialized(bool EnemyStatus)
    {
        _EnemyAI = gameObject.transform.root.gameObject.GetComponent<EnemyAIBase>();

    }
}
