using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ModalWindow : MonoBehaviour
{
    private float openModalAnimationTime = 0.3f;
    private float closeModalAnimationTime = 0.3f;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OpenModal()
    {
        AudioManager.Instance.PlayMenuNavigationSound();
        gameObject.SetActive(true);
        transform.localPosition = new Vector2(0, -Screen.height);
        Sequence openModalSequence = DOTween.Sequence();
        openModalSequence.Append(rectTransform.DOAnchorPos(Vector2.zero, openModalAnimationTime).SetEase(Ease.InOutQuint));
        openModalSequence.PlayForward();
    }

    public void CloseModal()
    {
        AudioManager.Instance.PlayMenuNavigationSound();
        Sequence openModalSequence = DOTween.Sequence();
        openModalSequence.Append(rectTransform.DOAnchorPos(new Vector2(0, -Screen.height), closeModalAnimationTime).SetEase(Ease.InOutQuint));
        openModalSequence.AppendCallback(() => {gameObject.SetActive(false);});
        openModalSequence.PlayForward();
    }
}
