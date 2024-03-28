using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RockPaperScissors.UI
{
    public class CurrencyFX : MonoBehaviour
    {
        [SerializeField] private CurrencyUI currencyUI;
        private void Update() 
        {
            Vector2 currencyUIWorldPosition = Camera.main.ScreenToWorldPoint(currencyUI.GetMarbleLocation().position);
            transform.position = currencyUIWorldPosition;
        }        
    }    
}
