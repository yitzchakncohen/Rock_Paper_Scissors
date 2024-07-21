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
        private TextMeshProUGUI textComponent;

        private void Awake() 
        {
            textComponent = GetComponent<TextMeshProUGUI>();
            textComponent.text = "";
        }

        public void Play(string textString, float animationTime)
        {
            if(textComponent == null)
            {
                textComponent = GetComponent<TextMeshProUGUI>();
            }
            textComponent.text = "";
            textComponent.DOText(textString, animationTime).SetUpdate(true);
        }
    }    
}
