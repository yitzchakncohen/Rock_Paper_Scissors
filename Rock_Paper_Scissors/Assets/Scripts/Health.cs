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
    private UnitAnimator unitAnimator;
    private int health;
    private float deathAnimationTime = 0.6f;

    private void Awake() 
    {
        unit = GetComponent<Unit>();
        unitAnimator = GetComponentInChildren<UnitAnimator>();
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
        OnHealthChanged?.Invoke();
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
            StartCoroutine(OnDeathRoutine(attacker));
        }
    }

    private IEnumerator OnDeathRoutine(Unit attacker)
    {
        yield return unitAnimator.StartCoroutine(unitAnimator.DeathAnimationRoutine(deathAnimationTime));
        Destroy(gameObject);
        OnDeath?.Invoke(this, attacker);
    }

    public int GetHealth()
    {
        return health;
    }

    public float GetNormalizedHealth()
    {
        return (float)health / (float)unit.GetMaximumHealth();
    }

    public Unit GetUnit()
    {
        return unit;
    }
}
