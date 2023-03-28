using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitProgression : MonoBehaviour
{
    public event Action OnLevelUp;
    private int level = 1;
    private int xp = 0;

    private void Start() 
    {
        Health.OnDeath += Health_OnDeath;
    }

    private void Health_OnDeath(object sender, Unit attacker)
    {
        if(attacker.GetUnitProgression() == this)
        {
            GainXP(100);
        }
    }

    private void GainXP(int amount)
    {
        xp += amount;
        CheckForLevelUp();
    }

    private void CheckForLevelUp()
    {
        if(xp % 100 == 0)
        {
            level = Math.Clamp(level + 1, 1, 3);
            OnLevelUp?.Invoke();
        }
    }

    public int GetLevel()
    {
        return level;
    }
}
