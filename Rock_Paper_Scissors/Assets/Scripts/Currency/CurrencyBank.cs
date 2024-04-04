using System;
using RockPaperScissors.SaveSystem;
using Unity.Mathematics;
using UnityEngine;

public class CurrencyBank : MonoBehaviour, ISaveInterface<SaveCurrencyBankData>
{
    [SerializeField] private ParticleSystem MarbleFXPrefab;
    public event EventHandler<int> OnCurrencyChanged;
    private int currency = 0;

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

    public void AddCurrencyToBank(int amount, Transform unitLocation)
    {
        currency += amount;
        AudioManager.Instance.PlayCollectCurrencySound();
        OnCurrencyChanged?.Invoke(this, currency);
        if(unitLocation != null)
        {
            Instantiate(MarbleFXPrefab, unitLocation.position, quaternion.identity);
        }
    }

    [ContextMenu("More Monies")]
    public void AddSomeCurrency()
    {
        AddCurrencyToBank(1000, null);
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
