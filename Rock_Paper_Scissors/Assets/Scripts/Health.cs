using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public static event EventHandler<Unit> OnDeath;
    public event Action OnHealthChanged;
    private Unit unit;
    private UnitProgression unitProgression;
    private int health;

    private void Awake() 
    {
        unit = GetComponent<Unit>();
    }
    
    private void Start() 
    {
        unitProgression = unit.GetUnitProgression();
        unitProgression.OnLevelUp += UnitProgression_OnLevelUp;        
        health = unit.GetMaximumHealth();
    }

    private void OnDestroy() 
    {
        unitProgression.OnLevelUp -= UnitProgression_OnLevelUp;
    }

    private void UnitProgression_OnLevelUp()
    {
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
