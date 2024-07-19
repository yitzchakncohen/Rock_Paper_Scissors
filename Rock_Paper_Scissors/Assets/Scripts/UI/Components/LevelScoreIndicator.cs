using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelScoreIndicator : MonoBehaviour
{
    const string SCORE_REQUIREMENT_1 = "In ";
    const string SCORE_REQUIREMENT_2 = " Turns";
    const string NO_REQUIREMENT = "Victory";
    [SerializeField] private TextMeshProUGUI scoreRequirement;
    [SerializeField] private Color requirementMetColor;
    [SerializeField] private Color requirementNotMetColor;
    private float animationTime = 0.6f;

    private Image star;

    private void Awake() 
    {
        star = GetComponent<Image>();
        transform.localScale = Vector3.zero;
    }

    public void UpdateScore(bool requirementMet, int turns =-1)
    {
        if(turns == -1)
        {
            scoreRequirement.text = NO_REQUIREMENT;
        }
        else
        {
            scoreRequirement.text = SCORE_REQUIREMENT_1 + turns + SCORE_REQUIREMENT_2;
        }
        if(requirementMet)
        {
            star.color = requirementMetColor;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(1.0f, animationTime).SetEase(Ease.OutElastic));
            sequence.InsertCallback(animationTime/2f, AudioManager.Instance.PlayRequirementMetSound);
            sequence.PlayForward();
        }
        else
        {
            star.color = requirementNotMetColor;
            Sequence sequence = DOTween.Sequence();
            sequence.Append(transform.DOScale(1.0f, animationTime));
            sequence.InsertCallback(animationTime/2f, AudioManager.Instance.PlayRequirementNotMetSound);
            sequence.PlayForward();
        }
    }
}
