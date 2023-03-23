using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public static event EventHandler OnDeath;
    [SerializeField] private int maxHealth = 10;
    private int health;

    private void Awake() 
    {
        health = maxHealth;
    }

    public void Damage(int damageAmount)
    {
        // Debug.Log("Damage!");
        health -= damageAmount;
        CheckForDeath();
    }

    public void CheckForDeath()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
            OnDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    public float GetNormalizedHealth()
    {
        return (float)health / (float)maxHealth;
    }
}
