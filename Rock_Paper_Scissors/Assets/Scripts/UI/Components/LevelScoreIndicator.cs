using System.Collections;
using System.Collections.Generic;
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

    private Image star;

    private void Awake() 
    {
        star = GetComponent<Image>();
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
        }
        else
        {
            star.color = requirementNotMetColor;
        }
    }
}
