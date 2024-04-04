using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransitionUI : MonoBehaviour
{
    [SerializeField] private Image transitionPanel;
    [SerializeField] private Image loadingPanel;
    [SerializeField] private float fadeOutTime = 0.5f;
    [SerializeField] private float fadeInTime = 0.5f;

    public IEnumerator TransitionOut()
    {
        transitionPanel.gameObject.SetActive(true);
        transitionPanel.DOFade(1.0f, fadeOutTime);
        yield return new WaitForSeconds(fadeOutTime);
    }

    public void TransitionIn()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(transitionPanel.DOFade(0.0f, fadeInTime));
        sequence.AppendCallback(()=>{
            transitionPanel.gameObject.SetActive(false);
        });
        sequence.PlayForward();
    }

    public void StartLoading()
    {
        loadingPanel.gameObject.SetActive(true);
        loadingPanel.DOFade(1.0f, 0.0f);
    }

    public void LoadingCompleted()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(loadingPanel.DOFade(0.0f, fadeInTime));
        sequence.AppendCallback(()=>{
            loadingPanel.gameObject.SetActive(false);
        });
        sequence.PlayForward();
    }
}
