/*****************************************************************************
Author: Brett Hansen
Date: 06/08/23

Description: Pickup base class; not really used heavily for now, but may be useful later

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PickupBase : MonoBehaviour
{
    #region variable space
    //space for variables; currently just temporary float/oscillation info for transform
    [Header("Pickup Movement")]
    [Tooltip("how fast will the pickup cycle up and down")]
    [SerializeField] protected float oscillationSpeed = 1f;
    [Tooltip("how much will the pickup bob up and down")]
    [SerializeField] protected float oscillationMagnitude = .0025f;
    [Tooltip("how rapidly will the pickup rotate")]
    [SerializeField] protected float rotationSpeed = 1f;
    [Tooltip("Oscillation offset to ensure all pickups move out of sync w/ one another")]
    protected float oscillationOffset;
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    //basic idea is to set an oscillation & rotation offset so multiple pickups near eachother won't look like they're in sync with eachother
    //I know this is a placeholder, but it bugged me.
    private void Awake()
    {
        oscillationOffset = Random.Range(0, Time.time+180);
        transform.Rotate(0, oscillationOffset, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 Position = transform.position;
        float YMod = Mathf.Sin(Time.time * oscillationSpeed + oscillationOffset) * oscillationMagnitude + Position.y;
        transform.position = new Vector3(Position.x, YMod, Position.z);

        transform.Rotate(0, rotationSpeed, 0);
    }
}
