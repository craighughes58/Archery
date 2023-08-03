using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBow : WeaponBehaviorBase
{
    [SerializeField] protected GameObject _ArrowPrefab;
    protected ArrowBehaviour _ArrowBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        _ArrowBehaviour = _ArrowPrefab.GetComponent<ArrowBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
