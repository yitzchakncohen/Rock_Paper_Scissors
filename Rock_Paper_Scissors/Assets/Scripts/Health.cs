using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public static event EventHandler<Unit> OnDeath;
    public event Action OnHealthChanged;
    private Unit unit;
    private int health;

    private void Awake() 
    {
        unit = GetComponent<Unit>();
        health = unit.GetMaximumHealth();
    }

    public void Damage(int damageAmount, Unit attacker)
    {
        // Debug.Log("Damage!");
        health -= damageAmount;
        OnHealthChanged?.Invoke();
        CheckForDeath(attacker);
    }

    public void CheckForDeath(Unit attacker)
    {
        if(health <= 0)
        {
            Destroy(gameObject);
            OnDeath?.Invoke(this, attacker);
        }
    }

    public int GetHealth()
    {
        return health;
    }

    public float GetNormalizedHealth()
    {
        return (float)health / (float)unit.GetMaximumHealth();
    }
}
