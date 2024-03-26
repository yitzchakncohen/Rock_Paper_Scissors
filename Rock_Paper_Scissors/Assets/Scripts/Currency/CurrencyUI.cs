using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    private CurrencyBank currencyBank;

    private void Start() 
    {
        currencyBank = FindObjectOfType<CurrencyBank>();
        currencyBank.OnCurrencyChanged += CurrencyBank_OnCurrencyChanged;
    }

    private void OnDestroy() 
    {
        currencyBank.OnCurrencyChanged -= CurrencyBank_OnCurrencyChanged;
    }

    private void CurrencyBank_OnCurrencyChanged(object sender, int currency)
    {
        currencyText.text = currency.ToString();
    }
}
