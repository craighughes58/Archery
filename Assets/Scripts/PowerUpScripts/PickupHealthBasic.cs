/*****************************************************************************
Author: Brett Hansen
Date: 06/08/23

Description: This is the basic health pack pickup script

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupHealthBasic : PickupBase
{
    #region private variables
    //variable space - for now just containing pack amount
    [Header("Health Variables")]
    [Tooltip("How much health in this health pack")]
    [SerializeField] private int healthAmount = 10;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //function checks if the player is touching it; if so, asks the player if it can add ammo
    //if health was added, health pack is considered picked up & thus destroyed
    //NEEDED ADD: Some sort of VFX & sound queue to note pickup of ammo
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = GameObject.FindObjectOfType<PlayerController>();
            //Debug.Log("HealthPack trigger called");
            if (player.AddHealth(healthAmount)) Destroy(this.gameObject);
        }
    }
}
