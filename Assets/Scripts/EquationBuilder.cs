using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Формирует мат.выражение и варианты ответа, анимирует построение и действия игрока
/// </summary>
public class EquationBuilder : MonoBehaviour
{
    [SerializeField, Min(1)] int levelEquationCount = 10;

    [Space]
    [SerializeField] LevelMaths levelMaths;
    [SerializeField] RectTransform equationRect;
    [SerializeField] TextMeshProUGUI textLeft;
    [SerializeField] TextMeshProUGUI textX;
    [SerializeField] TextMeshProUGUI textRigt;

    [Space]
    [SerializeField] AnswerButton answerButtonPrefab;
    [SerializeField] RectTransform spawnPoint;
    [SerializeField, Min(2)] int answersCount = 4;
    [SerializeField, Min(1)] int answerButtonSpacing = 100;

    private List<AnswerButton> answerButtons = new List<AnswerButton>();
    private Equation equation;

    private void Awake()
    {
        for (int i = 0; i < answersCount; i++)
        {
            var button = Instantiate(answerButtonPrefab, spawnPoint.position, Quaternion.identity, transform);
            answerButtons.Add(button);
        }
    }

    private void Start()
    {
        SetEquation();
        //StartCoroutine(LevelSiquanceCoroutine());
    }

    private IEnumerator LevelSiquanceCoroutine()
    {
        for (int i = 0; i < levelEquationCount; i++)
        {
            yield return null;
            
            // fade in

            SetEquation();
            
            // fade out
        }

        // end animation
    }

    private void SetEquation()
    {
        equation = levelMaths.GetEquation();
        textLeft.text = equation.expressionLeft;
        textRigt.text = equation.expressionRight;
    }
}