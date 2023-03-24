using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public static event EventHandler OnDeath;
    public event Action OnHealthChanged;
    private Unit unit;
    private int health;

    private void Awake() 
    {
        unit = GetComponent<Unit>();
        health = unit.GetMaximumHealth();
    }

    public void Damage(int damageAmount)
    {
        // Debug.Log("Damage!");
        health -= damageAmount;
        OnHealthChanged?.Invoke();
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
        return (float)health / (float)unit.GetMaximumHealth();
    }
}
