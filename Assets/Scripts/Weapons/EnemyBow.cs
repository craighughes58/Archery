using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyBow : EnemyWeaponBehaviorBase
{
    [SerializeField] protected GameObject _ArrowPrefab;
    protected GameObject _CurrentArrow;
    [SerializeField] protected float _ArrowForce;
    protected int _ArrowNum = 0;
    [Tooltip("Dictates the initial upward aim of the arrow.")]
    [SerializeField] protected float _YOffset = 0.75f;



    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Collider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void Fire()
    {
        _CurrentArrow = Instantiate(_ArrowPrefab, 
            _EnemyAI._ShootFromLocation.transform.position, 
            _EnemyAI._ShootFromLocation.transform.rotation
            );
        ArrowBehaviour ArrowBehaviour = _CurrentArrow.GetComponent<ArrowBehaviour>();
        ArrowBehaviour.SetArrowOriginClass("Enemy");
        ArrowBehaviour.setArrowType(_ArrowNum);
        _CurrentArrow.GetComponent<Rigidbody>().velocity = _EnemyAI._ShootFromLocation.transform.forward * (_ArrowForce);


    }
}
