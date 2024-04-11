using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI.Components
{
    public class GravityWell : MonoBehaviour
    {
        [SerializeField] private Transform trackingTransform;

        private void Update() 
        {
            // Vector2 currencyUIWorldPosition = Camera.main.ScreenToWorldPoint(currencyUI.GetMarbleLocation().position);
            // Game hud canvas is now in world space, so I can access position directly. 
            transform.position = trackingTransform.position;
        }        
    }    
}
