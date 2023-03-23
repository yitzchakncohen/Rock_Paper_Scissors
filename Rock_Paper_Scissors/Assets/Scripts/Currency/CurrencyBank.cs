using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyBank : MonoBehaviour
{
    public event EventHandler<int> OnCurrencyChanged;
    private int currency = 100;

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
    }
}   
