using System;
using DG.Tweening;
using RockPaperScissors;
using RockPaperScissors.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameMenu : MonoBehaviour
{
    public static event Action OnStartGameButtonPress;

    [SerializeField] private GameObject HUDPanel;
    [SerializeField] private ModalWindow gameMenuPanel;
    [SerializeField] private GameObject gameOverMenuPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreValueText;
    [SerializeField] private TextMeshProUGUI gameOverHighScoreValueText;
    [SerializeField] private Button[] mainMenuButtons;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button closeMenuButton;
    [SerializeField] private LetterAnimation gameOverTextAnimation;
    [SerializeField] private float gameOverAnimationTime = 0.5f;
    [SerializeField] private AdModal adModal;
    [SerializeField] private ModalWindow settingsModal;
    private RectTransform gameOverMenuRectTransform;

    private void Start() 
    {
        gameOverMenuRectTransform = gameOverMenuPanel.GetComponent<RectTransform>();
        GameplayManager.OnGameOver += GameplayManager_OnGameOver;
        AdModal.OnSkipButtonClick += AdModal_OnSkipButtonClick;
        AdModal.OnWatchButtonClick += AdModal_OnWatchButtonClick;
        foreach (Button button in mainMenuButtons)
        {
            button.onClick.AddListener(GoToMainMenu);
        }
        newGameButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(OpenSettingsMenu);
        closeMenuButton.onClick.AddListener(CloseGameMenu);

        gameOverMenuPanel.SetActive(false);
        gameMenuPanel.gameObject.SetActive(false);
        HUDPanel.SetActive(true);
        adModal.gameObject.SetActive(false);
        settingsModal.gameObject.SetActive(false);  
    }

    private void OnDestroy() 
    {
        foreach (Button button in mainMenuButtons)
        {
            button.onClick.RemoveAllListeners();
        }
        newGameButton.onClick.RemoveAllListeners();
        settingsButton.onClick.RemoveAllListeners();
        closeMenuButton.onClick.RemoveAllListeners();   
        GameplayManager.OnGameOver -= GameplayManager_OnGameOver;    
        AdModal.OnSkipButtonClick -= AdModal_OnSkipButtonClick;
        AdModal.OnWatchButtonClick -= AdModal_OnWatchButtonClick;    
    }

    public void OpenGameMenu()
    {
        if(!gameMenuPanel.gameObject.activeSelf)
        {
            gameMenuPanel.OpenModal();
        }
    }

    public void CloseGameMenu()
    {
        if(gameMenuPanel.gameObject.activeSelf)
        {
            gameMenuPanel.CloseModal();
        }
    }

    public void OpenSettingsMenu()
    {
        if(!settingsModal.gameObject.activeSelf)
        {
            settingsModal.OpenModal();
        }
    }

    public void CloseSettingsMenu()
    {
        if(settingsModal.gameObject.activeSelf)
        {
            settingsModal.CloseModal();
        }
    }


    public void OpenGameOverMenu(int score, int highscore)
    {
        HUDPanel.SetActive(false);
        gameOverMenuPanel.SetActive(true);
        gameOverScoreValueText.text = score.ToString();
        gameOverHighScoreValueText.text = highscore.ToString();
        gameOverMenuRectTransform.transform.localPosition = new Vector2(0, -Screen.height);
        gameOverMenuRectTransform.sizeDelta = new Vector2(gameOverMenuRectTransform.sizeDelta.x * Camera.main.aspect/2, gameOverMenuRectTransform.sizeDelta.y);
        Sequence gameOverSequence = DOTween.Sequence();
        gameOverSequence.Append(gameOverMenuRectTransform.DOAnchorPos(Vector2.zero, gameOverAnimationTime).SetEase(Ease.InOutQuint));
        gameOverSequence.AppendCallback(() => {
            gameOverTextAnimation.Play();
        });
        gameOverSequence.PlayForward();
    }

    private void CloseGameOverMenu()
    {
        gameOverMenuPanel.SetActive(false);
    }

    private void GoToMainMenu()
    {
        ApplicationManager.Instance.ReturnToMenu();
        AudioManager.Instance.PlayMenuNavigationSound();
    }

    private void StartGame()
    {
        OnStartGameButtonPress?.Invoke();
        AudioManager.Instance.PlayMenuNavigationSound();
    }

    private void GameplayManager_OnGameOver(object sender, GameplayManager.OnGameOverEventArgs e)
    {
        adModal.gameObject.SetActive(true);
        adModal.PassGameOverEventArgs(e);
    }

    private void AdModal_OnWatchButtonClick(object sender, GameplayManager.OnGameOverEventArgs e)
    {
        OpenGameOverMenu(e.Score, e.Highscore);
    }

    private void AdModal_OnSkipButtonClick(object sender, GameplayManager.OnGameOverEventArgs e)
    {
        OpenGameOverMenu(e.Score, e.Highscore);
    }
}
