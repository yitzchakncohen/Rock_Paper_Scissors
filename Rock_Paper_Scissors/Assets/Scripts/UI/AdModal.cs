using System;
using RockPaperScissors.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI
{
    public class AdModal : MonoBehaviour
    {
        public static event EventHandler<GameplayManager.OnGameOverEventArgs> OnWatchButtonClick;
        public static event EventHandler<GameplayManager.OnGameOverEventArgs> OnSkipButtonClick;
        [SerializeField] private Button watchAdButton;
        [SerializeField] private Button skipAdButton;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private float watchAdTime = 5f;
        private ModalWindow modalWindow;
        private float timer = 0f;
        private GameplayManager.OnGameOverEventArgs onGameOverEventArgs = null;

        private void Awake() 
        {
            watchAdButton.onClick.AddListener(WatchAdButton_onClick);
            skipAdButton.onClick.AddListener(SkipAdButton_onClick);
            modalWindow = GetComponent<ModalWindow>();
        }

        private void Update() 
        {
            if( onGameOverEventArgs != null)
            {
                timer -= Time.deltaTime;
                UpdateTimer(timer);
                if(timer < 0)
                {
                    WatchAdButton_onClick();
                }
            }
        }

        private void SkipAdButton_onClick()
        {
            OnSkipButtonClick?.Invoke(this, onGameOverEventArgs);
            modalWindow.Close();
            onGameOverEventArgs = null;
        }

        private void WatchAdButton_onClick()
        {
            OnWatchButtonClick?.Invoke(this, onGameOverEventArgs);
            modalWindow.Close();
            onGameOverEventArgs = null;
        }

        public void PassGameOverEventArgs(GameplayManager.OnGameOverEventArgs onGameOverEventArgs)
        {
            this.onGameOverEventArgs = onGameOverEventArgs;
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
