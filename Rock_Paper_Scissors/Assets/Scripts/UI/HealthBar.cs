using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarImage; 
    private Health health;

    private void Start() 
    {
        health = GetComponentInParent<Health>();
        health.OnHealthChanged += Health_OnHealthChanged;
        healthBarImage.fillAmount = 1f;
    }

    private void Health_OnHealthChanged()
    {
        healthBarImage.fillAmount = health.GetNormalizedHealth();
    }
}
