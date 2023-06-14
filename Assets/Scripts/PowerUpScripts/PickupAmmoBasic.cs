/*****************************************************************************
Author: Brett Hansen
Date: 06/08/23

Description: This is the basic ammo pack pickup on the map.
If we implement different ammo types, we'll likely inherit off this base class.

*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupAmmoBasic : PickupBase
{
    #region private variables
    //variable space - for now just containing pack amount
    [Header("Ammo Variables")]
    [Tooltip("How many arrows in arrow package")]
    [SerializeField] private int packSize = 5; 

    #endregion


    // Start is called before the first frame update
    void Start()
    {
        
    }

    //function checks if the player is touching it; if so, asks the player if it can add ammo
    //AddAmmo returns a boolean to indicate if any ammo was able to be added
    //if ammo was added, ammo pack is considered picked up & thus destroyed
    //NEEDED ADD: Some sort of VFX & sound queue to note pickup of ammo
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.GetComponent<PlayerController>();
            //Debug.Log("AmmoPack trigger called");
            if (player.AddAmmo(packSize)) Destroy(this.gameObject);
        }
    }

}
