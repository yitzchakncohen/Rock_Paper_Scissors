using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.Units;
using UnityEngine;

public class CurrencyBank : MonoBehaviour
{
    public event EventHandler<int> OnCurrencyChanged;
    private int currency = 0;

    private void Start() 
    {
        UnitHealth.OnDeath += Health_OnDeath;
    }

    public bool TrySpendCurrency(int amountToSpend)
    {
        if(amountToSpend <= currency)
        {
            currency -= amountToSpend;
            OnCurrencyChanged?.Invoke(this, currency);
            return true;
        }
        return false;
    }

    public int GetCurrencyRemaining()
    {
        return currency;
    }

    public void AddCurrencyToBank(int amount)
    {
        currency += amount;
        OnCurrencyChanged?.Invoke(this, currency);
    }

    private void Health_OnDeath(object sender, Unit e)
    {
        Unit unit = ((UnitHealth)sender).GetUnit();
        if(!unit.IsFriendly())
        {
            currency += unit.GetUnitDefeatedReward();
            OnCurrencyChanged?.Invoke(this, currency);
        }
    }
}   
