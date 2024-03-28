using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private Transform marblesIcon;
    private CurrencyBank currencyBank;
    private float currencyUpdateTime = 0.3f;

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
        // currencyText.text = currency.ToString();
        StartCoroutine(CurrencyUpdateRoutine(currency));
    }

    private IEnumerator CurrencyUpdateRoutine(int currency)
    {
        float startingCurrency = int.Parse(currencyText.text);

        while(startingCurrency <= currency-1)
        {
            startingCurrency = Mathf.Lerp(startingCurrency, currency, Time.deltaTime/currencyUpdateTime);
            currencyText.text = startingCurrency.ToString("0000");
            // Debug.Log(startingCurrency);
            yield return null;
        }
        currencyText.text = currency.ToString("0000");
    }

    public Transform GetMarbleLocation()
    {
        return marblesIcon;
    }
}
