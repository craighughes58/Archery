/*****************************************************************************
Author: Brett Hansen
Date: 06/21/23

Description: This is a script to be attached to any object that wants to be visible in the player's minimap
It generates a minimap exclusive "puck" that it attaches to parent

*/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MinimapVisibility : MonoBehaviour
{
    public enum minimapFlag { Neutral, Enemy, Player, HealthPickup, AmmoPickup };
    [SerializeField] minimapFlag mapFlag = minimapFlag.Neutral;

    //
    // Start is called before the first frame update
    void Start()
    {
        //variables placed here for ease of tweaking
        Vector3 localScale = new Vector3(1, .1f, 1); //absolute scale of pucks
        float playerScaleFactor = 1.5f; // how much to scale up player icon by

        GameObject minimapToken = GameObject.CreatePrimitive(PrimitiveType.Cylinder); //generate minimap puck object
        Destroy(minimapToken.GetComponent<CapsuleCollider>()); //remove capsule collider for physics purposes

        minimapToken.layer = LayerMask.NameToLayer("Minimap"); //sets the puck to be on the minimap layer


        //Chunk to set color of minimap puck based on selected minimap flag value
        Color minimapTokenColor; 
        if (mapFlag == minimapFlag.Enemy)
        {
            minimapTokenColor = Color.red;
        }
        else if (mapFlag == minimapFlag.Player)
        {
            minimapTokenColor = Color.blue;
            localScale.x = localScale.x * playerScaleFactor;
            localScale.z = localScale.z * playerScaleFactor;
        }
        else if (mapFlag == minimapFlag.HealthPickup)
        {
            minimapTokenColor = Color.green;
        }
        else if (mapFlag == minimapFlag.AmmoPickup)
        {
            minimapTokenColor = Color.yellow;
        }
        else
        {
            minimapTokenColor = Color.white;
        }
        minimapToken.GetComponent<Renderer>().material.color = minimapTokenColor;

        //attaches the minimaptoken to the parent gameobject
        minimapToken.transform.parent = this.transform;

        //sets position, rotation & scale of minimap puck appropriately 
        //note the local scale is a bit kludgy because you can't set the global scale directly nicely in unity.
        minimapToken.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity) ;
        minimapToken.transform.localScale = new Vector3(localScale.x / transform.lossyScale.x, localScale.y / transform.lossyScale.y, localScale.z / transform.lossyScale.z); 

    }
}
