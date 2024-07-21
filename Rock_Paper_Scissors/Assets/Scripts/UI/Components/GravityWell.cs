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
            Vector2 trackingWorldPosition = Camera.main.ScreenToWorldPoint(trackingTransform.position);
            transform.position = trackingWorldPosition;
        }        
    }    
}
