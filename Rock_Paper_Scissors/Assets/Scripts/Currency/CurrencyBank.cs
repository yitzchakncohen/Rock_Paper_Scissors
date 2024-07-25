using System;
using RockPaperScissors.SaveSystem;
using Unity.Mathematics;
using UnityEngine;

public class CurrencyBank : MonoBehaviour, ISaveInterface<SaveCurrencyBankData>
{
    public static CurrencyBank FriendlyCurrencyBank = null;
    public static CurrencyBank EnemyCurrencyBank = null;
    public event EventHandler<int> OnCurrencyChanged;
    [SerializeField] private bool isFriendly;
    [SerializeField] private ParticleSystem MarbleFXPrefab;
    private int currency = 0;

    private void Awake() 
    {
        if(isFriendly)
        {
            if(FriendlyCurrencyBank == null)
            {
                FriendlyCurrencyBank = this;
            }
            else
            {
                DestroyImmediate(this);
            }
        }
        else
        {
            if(EnemyCurrencyBank == null)
            {
                EnemyCurrencyBank = this;
            }
            else
            {
                DestroyImmediate(this);
            }
        }
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

    public void AddCurrencyToBank(int amount, Transform unitLocation)
    {
        currency += amount;
        OnCurrencyChanged?.Invoke(this, currency);
        if(isFriendly)
        {
            AudioManager.Instance.PlayCollectCurrencySound();
            if(unitLocation != null)
            {
                Instantiate(MarbleFXPrefab, unitLocation.position, quaternion.identity);
            }
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
            Currency = currency,
            IsFriendly = isFriendly
        };

        return bankData;
    }

    public void Load(SaveCurrencyBankData loadData)
    {
        currency = loadData.Currency;
        OnCurrencyChanged?.Invoke(this, currency);
    }
}   
