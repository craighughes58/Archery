using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectileBehaviour : MonoBehaviour
{
    [Tooltip("How fast the object moves")]
    [SerializeField]
    private float speed;
    //reference to the rigidbody attached to the object
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;
    }

/*    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }*/
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag.Equals("Player"))
        {
            Destroy(gameObject);        }
    }
}
