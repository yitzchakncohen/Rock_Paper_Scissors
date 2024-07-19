using System;
using RockPaperScissors.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI
{
    public class AdModal : MonoBehaviour
    {
        public static event Action OnWatchButtonClick;
        public static event Action OnSkipButtonClick;
        [SerializeField] private Button watchAdButton;
        [SerializeField] private Button skipAdButton;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private float watchAdTime = 5f;
        private ModalWindow modalWindow;
        private float timer = 0f;

        private void Awake() 
        {
            watchAdButton.onClick.AddListener(WatchAdButton_onClick);
            skipAdButton.onClick.AddListener(SkipAdButton_onClick);
            modalWindow = GetComponent<ModalWindow>();
        }

        private void Update() 
        {
            timer -= Time.deltaTime;
            UpdateTimer(timer);
            if(timer < 0)
            {
                WatchAdButton_onClick();
            }
        }

        private void SkipAdButton_onClick()
        {
            OnSkipButtonClick?.Invoke();
            modalWindow.Close();
        }

        private void WatchAdButton_onClick()
        {
            OnWatchButtonClick?.Invoke();
            modalWindow.Close();
        }

        public void SetupTimer()
        {
            timer = watchAdTime;
            UpdateTimer(watchAdTime);
        }

        private void UpdateTimer(float time)
        {
            timerText.text = "Ad: " + time.ToString("0");
        }

        internal void Open()
        {
            modalWindow.Open();
        }
    }
}
