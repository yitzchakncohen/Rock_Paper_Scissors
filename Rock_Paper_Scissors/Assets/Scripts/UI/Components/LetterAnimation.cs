using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace RockPaperScissors.UI.Components
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LetterAnimation : MonoBehaviour
    {
        [SerializeField] private float animationTime = 0.3f;
        private TextMeshProUGUI textComponent;
        private string textString = "";

        private void Awake() 
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            textString = textComponent.text;
            textComponent.text = "";
        }

        public void Play()
        {
            textComponent.DOText(textString, animationTime);
        }
    }    
}
