using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RockPaperScissors.UI.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RockPaperScissors.UI.Menus
{
    public class GameOverMenu : MonoBehaviour
    {
        public static event Action OnStartEndlessGameButtonPress;
        public static event Action OnRestartLevelButtonPress;
        public static event Action OnNextLevelButtonPress;
        private const string GAME_OVER_STRING = "Game Over";
        private const string LEVEL_COMPLETE_STRING = "Success!";
        [SerializeField] private Button newGameButton;
        [SerializeField] private Button restartLevelButton;
        [SerializeField] private Button nextLevelButton;
        [SerializeField] private TextMeshProUGUI scoreValueText;
        [SerializeField] private TextMeshProUGUI highScoreValueText;
        [SerializeField] private LetterAnimation textAnimation;
        [SerializeField] private float gameOverAnimationTime = 0.5f;
        [SerializeField] private GameObject endlessModeScore;
        [SerializeField] private GameObject endlessModeHighScore;
        [SerializeField] private GameObject levelModeScore;
        [SerializeField] private GameObject levelModeBestScore;
        [SerializeField] private LevelScoreIndicator[] scoreIndicators = new LevelScoreIndicator[3];
        [SerializeField] private LevelScoreIndicator[] bestScoreIndicators = new LevelScoreIndicator[3];
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        private void Start() 
        {
            newGameButton.onClick.AddListener(StartGame);
            restartLevelButton.onClick.AddListener(RestartLevel);
            nextLevelButton.onClick.AddListener(NextLevel);
        }

        private void OnDestroy() 
        {
            newGameButton.onClick.RemoveAllListeners();
            restartLevelButton.onClick.RemoveAllListeners();
            nextLevelButton.onClick.RemoveAllListeners();
        }

        public void Open(int score, int highscore, GameMode gameMode, LevelData levelData, bool winCondition = false)
        {
            gameObject.SetActive(true);
            switch (gameMode)
            {
                case GameMode.Level:
                    SetupForLevelMode(winCondition, score, highscore, levelData);
                    break;
                case GameMode.Endless:
                default:
                    SetupForEndlessMode();
                    break;
            }
            scoreValueText.text = score.ToString();
            highScoreValueText.text = highscore.ToString();
            rectTransform.transform.localPosition = new Vector2(0, -Screen.height);
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x * Camera.main.aspect/2, rectTransform.sizeDelta.y);
            Sequence gameOverSequence = DOTween.Sequence();
            gameOverSequence.Append(rectTransform.DOAnchorPos(Vector2.zero, gameOverAnimationTime).SetEase(Ease.InOutQuint).SetUpdate(true));
            if(winCondition)
            {
                gameOverSequence.AppendCallback(() => {
                    textAnimation.Play(LEVEL_COMPLETE_STRING);
                }).SetUpdate(true);
            }
            else
            {
                gameOverSequence.AppendCallback(() => {
                    textAnimation.Play(GAME_OVER_STRING);
                }).SetUpdate(true);
            }
            gameOverSequence.PlayForward();
        }

        private void SetupForEndlessMode()
        {
            newGameButton.gameObject.SetActive(true);
            endlessModeScore.SetActive(true);
            endlessModeHighScore.SetActive(true);
            restartLevelButton.gameObject.SetActive(false);
            nextLevelButton.gameObject.SetActive(false);
            levelModeScore.SetActive(false);
            levelModeBestScore.SetActive(false);
        }

        private void SetupForLevelMode(bool winCondition, int score, int highscore, LevelData levelData)
        {
            newGameButton.gameObject.SetActive(false);
            endlessModeScore.SetActive(false);
            endlessModeHighScore.SetActive(false);
            restartLevelButton.gameObject.SetActive(true);
            if(winCondition)
            {
                nextLevelButton.gameObject.SetActive(true);
            }
            else
            {
                nextLevelButton.gameObject.SetActive(false);
            }
            levelModeScore.SetActive(true);
            levelModeBestScore.SetActive(true);
            scoreIndicators[0].UpdateScore(score >= 1);
            scoreIndicators[1].UpdateScore(score >= 2, levelData.twoStarRequirement);
            scoreIndicators[2].UpdateScore(score >= 3, levelData.threeStarRequirement);
            bestScoreIndicators[0].UpdateScore(highscore >= 1);
            bestScoreIndicators[1].UpdateScore(highscore >= 2, levelData.twoStarRequirement);
            bestScoreIndicators[2].UpdateScore(highscore >= 3, levelData.threeStarRequirement);
        }

        private void StartGame()
        {
            OnStartEndlessGameButtonPress?.Invoke();
            AudioManager.Instance.PlayMenuNavigationSound();
        }

        private void RestartLevel()
        {
            OnRestartLevelButtonPress?.Invoke();
            AudioManager.Instance.PlayMenuNavigationSound();
        }

        private void NextLevel()
        {
            OnNextLevelButtonPress?.Invoke();
            AudioManager.Instance.PlayMenuNavigationSound();
        }
    }
}
