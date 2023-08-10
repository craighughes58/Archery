/*****************************************************************************
Author: Brett Hansen
Date: 6/15/2023

Description: This holds health & defense information for a parent creature.

*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    #region Delegate Events
    public delegate void OnHealthChange(float health); //delegate event called for health status change
    public event OnHealthChange onHealthChange; //CURRENTLY UNUSED
    #endregion
    
    #region Serialized Private Fields
    [Header("Health Fields")]
    [Tooltip("Maximum health of creature - cannot be exceeded")]
    [SerializeField, Min(1)] private float maximumHealth = 10;
    [Tooltip("Current health of creature")]
    [SerializeField, Min(1)] private float currentHealth = 10;
    [Tooltip("percentage of damage reduction - cannot reduce damage below 1")]
    [SerializeField, Range(0,1)] private float damageReduction = 0;
    [Tooltip("If this bool is true then it signifies that his class is for the player")]
    [SerializeField] private bool bIsPlayer;
    #endregion

    #region Methods
    /// <summary>
    /// function to process a healing request
    /// </summary>
    /// <param name="healAmount"></param>
    /// <returns></returns>
    public void heal(float healAmount)
    {
        this.currentHealth = Mathf.Min(currentHealth + healAmount, maximumHealth);
        this.onHealthChange?.Invoke(this.currentHealth);
    }

    /// <summary>
    /// function to process a damage request
    /// </summary>
    /// <param name="damageAmount"></param>
    /// I would rename this to TakeDamage, it's a lil ambiguous -MKE
    public void damage(float damageAmount) //might overload later to add "damage types"
    {
        float effectiveDamage = damageAmount;
        if (damageReduction > 0)
        { 
            effectiveDamage = damageAmount - Mathf.Round(damageAmount * this.damageReduction);
            effectiveDamage = Mathf.Min(effectiveDamage, damageAmount - 1); //has to do at least 1 point of reduction
            effectiveDamage = Mathf.Max(effectiveDamage, 0); //can't deal negative damage)
        }

        this.currentHealth = Mathf.Max(0, this.currentHealth - effectiveDamage); //updates health, bounded to zero
        this.onHealthChange?.Invoke(currentHealth); //notify relevant parties health has changed 

        if(bIsPlayer && this.currentHealth <= 0)
        {
            GetComponent<PlayerController>().NoteDeath();
        }

    }

    /// <summary>
    /// Will return true if health is already full relative to maximum
    /// </summary>
    /// <returns></returns>
    public bool isFull()
    {
        return ((currentHealth >= maximumHealth) ? true : false);
    }

    /// <summary>
    /// Returns true if health is above 0; otherwise false.
    /// </summary>
    /// <returns></returns>
    public bool isAlive()
    {
        if (this.currentHealth > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Getters & Setters
    //setters
    public void setMaximumHealth(float maximumHealth)
    {
        this.maximumHealth = Mathf.Max(maximumHealth, 1); //ensures you can't have empty or negative maxHealth
        if (this.maximumHealth < this.currentHealth)
        {
            this.currentHealth = this.maximumHealth; //clamps current health if it's more than maximum
        }
    }

    public void setCurrentHealth(int currentHealth)
    {
        this.currentHealth = Mathf.Clamp(currentHealth, 0, this.maximumHealth); //can't have a value above max
    }

    public void setDamageReduction(float damageReduction)
    {
        this.damageReduction = Mathf.Clamp(damageReduction, 0, 1);
    }

    //getters
    public float getMaximumHealth() { return this.maximumHealth; }
    public float getCurrentHealth() { return this.currentHealth; }
    public float getCurrentDamageReduction() { return this.damageReduction; }
    #endregion

}
