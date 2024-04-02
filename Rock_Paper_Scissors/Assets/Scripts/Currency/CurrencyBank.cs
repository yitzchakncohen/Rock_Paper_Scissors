using System;
using System.Collections;
using System.Collections.Generic;
using RockPaperScissors.SaveSystem;
using RockPaperScissors.Units;
using Unity.Mathematics;
using UnityEngine;

public class CurrencyBank : MonoBehaviour, ISaveInterface<SaveCurrencyBankData>
{
    [SerializeField] private ParticleSystem MarbleFXPrefab;
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

    public void AddCurrencyToBank(int amount, Unit unit)
    {
        currency += amount;
        AudioManager.Instance.PlayCollectCurrencySound();
        OnCurrencyChanged?.Invoke(this, currency);
        if(unit != null)
        {
            Instantiate(MarbleFXPrefab, unit.transform.position, quaternion.identity);
        }
    }

    [ContextMenu("More Monies")]
    public void AddSomeCurrency()
    {
        AddCurrencyToBank(1000, null);
    }

    private void Health_OnDeath(object sender, Unit e)
    {
        Unit unit = ((UnitHealth)sender).GetUnit();
        if(!unit.IsFriendly())
        {
            OnCurrencyChanged?.Invoke(this, currency);
            AddCurrencyToBank(unit.GetUnitDefeatedReward(), e);
        }

    }

    public SaveCurrencyBankData Save()
    {
        SaveCurrencyBankData bankData = new SaveCurrencyBankData
        {
            Currency = currency
        };

        return bankData;
    }

    public void Load(SaveCurrencyBankData loadData)
    {
        currency = loadData.Currency;
        OnCurrencyChanged?.Invoke(this, currency);
    }
}   
