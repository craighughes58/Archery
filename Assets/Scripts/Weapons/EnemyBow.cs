using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBow : EnemyWeaponBehaviorBase
{
    [SerializeField] protected GameObject _ArrowPrefab;
    protected GameObject _CurrentArrow;
    [SerializeField] protected float _ArrowForce;
    protected int _ArrowNum;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Fire()
    {
        _CurrentArrow = Instantiate(_ArrowPrefab, _EnemyAI._ShootFromLocation.transform.position, _EnemyAI._ShootFromLocation.transform.rotation);
        _CurrentArrow.transform.SetParent(_EnemyAI.transform, true);
        _CurrentArrow.GetComponent<Rigidbody>().velocity = _EnemyAI._ShootFromLocation.transform.forward * (_ArrowForce);
        _CurrentArrow.GetComponent<ArrowBehaviour>().setArrowType(_ArrowNum);


    }
}
