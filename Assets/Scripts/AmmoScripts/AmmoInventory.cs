/*****************************************************************************
Author: Brett Hansen
Date: 06/16/2023

Description: Class to hold Arrow inventory management for anything holding arrows.

*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AmmoInventory : MonoBehaviour
{
    #region Delegate Events
    public delegate void OnAmmoChange(int ammoPack); //delegate event called for ammo status change
    public event OnAmmoChange onAmmoChange; //CURRENTLY UNUSED
    #endregion

    #region Serialized Private Fields
    [Header("Ammo Fields")]
    [Tooltip("Whether this being has unlimited ammo")]
    [SerializeField] private bool hasInfininiteAmmo = false;
    [Tooltip("Maximum basic ammunition of creature - cannot be exceeded")]
    [SerializeField, Min(1)] private int quiverSize = 10;
    [Tooltip("Current basic ammunition of creature")]
    [SerializeField, Min(0)] private int quiverArrowCount = 10;
    #endregion

    #region Methods
    /// <summary>
    /// adds arrows to quiver, returns true if able to add, false if unable
    /// </summary>
    public bool AddToQuiver(int arrowPack)
    {
        if (quiverSize > quiverArrowCount)
        {
            quiverArrowCount = Mathf.Min(quiverSize, quiverArrowCount + arrowPack);
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// tries to pull an arrow from quiver, returns true if able to do so, false if unable
    /// </summary>
    /// <returns></returns>
    public bool PullFromQuiver()
    {
        if (hasInfininiteAmmo)
        {
            return true;
        }
        else if (quiverArrowCount > 0)
        {
            quiverArrowCount--;
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool QuiverIsEmpty()
    {
        if (quiverArrowCount > 0 || hasInfininiteAmmo)
        { return false; }
        else 
        { return true; }
    }
    #endregion

    #region Getters & Setters
    public int GetQuiverSize() { return quiverSize; }
    public int GetArrowCount() { return quiverArrowCount; }
    public bool GetInfiniteAmmoStatus() { return hasInfininiteAmmo; }

    /// <summary>
    /// sets size of quiver for arrows; input floored at zero
    /// </summary>
    /// <param name="size"></param>
    public void SetQuiverSize(int size) 
    {
        this.quiverSize = Mathf.Max(size, 0);
        if (this.quiverSize < this.quiverArrowCount)
        {
            this.quiverArrowCount = this.quiverSize;
        }
        
    }

    /// <summary>
    /// sets count of arrows in quiver; input clamped between 0 & size of quiver
    /// </summary>
    /// <param name="ammoPack"></param>
    public void SetArrowCount(int ammoPack) 
    {
        this.quiverArrowCount = Mathf.Clamp(ammoPack, 0,quiverSize); 
    }
    #endregion
}
