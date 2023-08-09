using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBow : WeaponBehaviorBase
{
    [SerializeField] protected GameObject _ArrowPrefab;
    protected GameObject _CurrentArrow;
    [SerializeField] protected float _ArrowForce;
    protected EnemyAIBase _EnemyAI;
    protected int _ArrowNum;


    // Start is called before the first frame update
    void Start()
    {
        _EnemyAI = gameObject.transform.root.GetComponent<EnemyAIBase>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Fire()
    {
        _CurrentArrow = Instantiate(_ArrowPrefab, _EnemyAI._ShootFromLocation.transform.position, _EnemyAI._ShootFromLocation.transform.rotation);
        Debug.Log("new arrow fired");
        _CurrentArrow.GetComponent<Rigidbody>().velocity = _EnemyAI._ShootFromLocation.transform.forward * (_ArrowForce);
        _CurrentArrow.GetComponent<ArrowBehaviour>().setArrowType(_ArrowNum);


    }
}
