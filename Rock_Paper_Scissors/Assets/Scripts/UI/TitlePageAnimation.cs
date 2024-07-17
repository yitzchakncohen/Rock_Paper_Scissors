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

    private List<ObjectToAnimate> titleBlockObjects = new List<ObjectToAnimate>();
    private List<ObjectToAnimate> mainMenuObjects = new List<ObjectToAnimate>();

    private void Start() 
    {
        StartCoroutine(SetupAnimation());
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

        highscore.alpha = 0;
    }

    public IEnumerator AnimationRoutine()
    {
        float time = 0f;
        float titleMoveTime = 0.5f;
        Sequence sequence = DOTween.Sequence();
        foreach (ObjectToAnimate item in titleBlockObjects)
        {
            sequence.Append(item.rectTransform.DOAnchorPos(item.position, titleMoveTime));
            time += titleMoveTime;
        }
        sequence.AppendInterval(titleMoveTime);
        time += titleMoveTime;
        float menuMoveTime = 0.4f;
        foreach (ObjectToAnimate item in mainMenuObjects)
        {
            sequence.Insert(time, item.rectTransform.DOAnchorPos(item.position, menuMoveTime));
            time += menuMoveTime / 2f;
        }
        float fadeTime = 1f;
        sequence.Append(highscore.DOFade(1f, fadeTime));
        sequence.PlayForward();
        yield return sequence;
    }
}
