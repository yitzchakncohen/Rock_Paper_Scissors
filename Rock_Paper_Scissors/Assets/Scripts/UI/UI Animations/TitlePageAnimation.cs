using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TitlePageAnimation : MonoBehaviour
{
    [System.Serializable]
    private struct ObjectToAnimate
    {
        public RectTransform rectTransform;
        public Vector2 position;
    }
    
    [SerializeField] private LayoutGroup[] layoutGroups;
    [SerializeField] private Transform mainMenu;
    [SerializeField] private Transform titleBlock;
    [SerializeField] private CanvasGroup highscore;
    [SerializeField] private ObjectToAnimate unit;

    private Animator unitAnimator;

    private List<ObjectToAnimate> titleBlockObjects = new List<ObjectToAnimate>();
    private List<ObjectToAnimate> mainMenuObjects = new List<ObjectToAnimate>();

    private void Start() 
    {
        StartCoroutine(SetupAnimation());
    }

    private void OnDestroy() 
    {
        CancelInvoke();
    }

    public IEnumerator SetupAnimation()
    {
        yield return new WaitForEndOfFrame();
        foreach (LayoutGroup group in layoutGroups)
        {
            group.enabled = false;
        }
        
        // Create sort lists of objects to animate
        foreach (RectTransform child in titleBlock)
        {
            ObjectToAnimate objectToAnimate = new ObjectToAnimate
            {
                rectTransform = child,
                position = child.anchoredPosition
            };
            titleBlockObjects.Add(objectToAnimate);
            child.DOMoveY(Screen.height*2, 0.0f);
        }
        titleBlockObjects.OrderBy(item => item.position.y);
        titleBlockObjects.Reverse();

        foreach (RectTransform child in mainMenu)
        {
            ObjectToAnimate objectToAnimate = new ObjectToAnimate
            {
                rectTransform = child,
                position = child.anchoredPosition
            };
            mainMenuObjects.Add(objectToAnimate);
            child.DOMoveY(Screen.height*2, 0.0f);
        }
        mainMenuObjects.OrderBy(item => item.position.y);
        mainMenuObjects.Reverse();

        // Highscore
        highscore.alpha = 0;

        // Unit
        unit.position = unit.rectTransform.anchoredPosition;
        unit.rectTransform.DOMoveX(0, 0.0f);
        unitAnimator = unit.rectTransform.GetComponent<Animator>();
    }

    public IEnumerator AnimationRoutine()
    {
        float time = 0f;
        float titleMoveTime = 0.5f;
        Sequence sequence = DOTween.Sequence();
        foreach (ObjectToAnimate item in titleBlockObjects)
        {
            sequence.Append(item.rectTransform.DOAnchorPos(item.position, titleMoveTime));
            sequence.AppendCallback(() => {
                AudioManager.Instance.PlayTitleBlockSound();
            });
            time += titleMoveTime;
        }
        sequence.AppendInterval(titleMoveTime);
        time += titleMoveTime;
        float menuMoveTime = 0.4f;
        foreach (ObjectToAnimate item in mainMenuObjects)
        {
            sequence.Insert(time, item.rectTransform.DOAnchorPos(item.position, menuMoveTime));
            time += menuMoveTime / 2f;
            sequence.InsertCallback(time, () => {
                AudioManager.Instance.PlayMenuBlockSound();
            });
        }
        float fadeTime = 1f;
        sequence.Append(highscore.DOFade(1f, fadeTime));
        sequence.AppendCallback(() => { 
            unitAnimator.SetTrigger("Hop");
        });
        float unitMoveTime = 2.0f;
        sequence.AppendCallback(PlayMoveSoundCallback);
        sequence.Append(unit.rectTransform.DOAnchorPos(unit.position, unitMoveTime).SetEase(Ease.Linear));
        sequence.AppendInterval(3.5f);
        sequence.AppendCallback(PlayRandomUnitAnimation);

        sequence.PlayForward();
        yield return sequence;
    }

    private void PlayMoveSoundCallback()
    {
        StartCoroutine(PlayMoveSound());
    }

    private IEnumerator PlayMoveSound()
    {
        AudioManager.Instance.PlayUnitMovementSound();
        yield return new WaitForSeconds(2.0f/3.0f);
        AudioManager.Instance.PlayUnitMovementSound();
        yield return new WaitForSeconds(2.0f/3.0f);
        AudioManager.Instance.PlayUnitMovementSound();
    }

    private void PlayRandomUnitAnimation()
    {
        int randomAnimation = Random.Range(1, 6);

        switch (randomAnimation)
        {
            case 1:
                unitAnimator.SetTrigger("Hop");
                break;
            case 2:
                unitAnimator.SetTrigger("LookRight");
                break;
            case 3:
                unitAnimator.SetTrigger("LookLeft");
                break;
            case 4:
                unitAnimator.SetTrigger("LookUpRight");
                break;
            case 5:
                unitAnimator.SetTrigger("LookUpLeft");
                break;
        }

        float randomAnimationTime = Random.Range(3.5f, 6.0f);
        Invoke("PlayRandomUnitAnimation", randomAnimationTime);
    }
}
