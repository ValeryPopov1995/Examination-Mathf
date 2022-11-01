using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
/// <summary>
/// Формирует мат.выражение и варианты ответа, анимирует построение и действия игрока
/// </summary>
public class EquationBuilder : MonoBehaviour
{
    #region DECLARATIONS
    public static EquationBuilder Singlton { get; private set; }

    [SerializeField, Min(1)] int levelEquationCount = 10;
    [SerializeField] LevelMaths levelMaths;

    [Header("Equation")]
    [SerializeField] RectTransform equationRect;
    [SerializeField] Image backlight;
    [SerializeField] TextMeshProUGUI textLeft;
    [SerializeField] TextMeshProUGUI textX;
    [SerializeField] TextMeshProUGUI textRigt;
    [SerializeField, Min(1)] int pixelsPerChar = 50;

    [Header("Answers")]
    [SerializeField] AnswerButton answerButtonPrefab;
    [field: SerializeField] public RectTransform spawnPoint { get; private set; }
    [SerializeField, Min(2)] int answersCount = 4;
    [field: SerializeField, Min(1)] public int answerButtonSpacing { get; private set; } = 100;

    [field: SerializeField, Min(.1f)] public float animDuration { get; private set; } = .9f;
    [SerializeField] Vector2Int incorrectDisplacement = new Vector2Int(-3, 3);

    public Equation equation { get; private set; }
    public float startEquationWidth { get; private set; }
    public float firstButtonPositionX { get; private set; }
    public bool IsInput { get; private set; }



    private List<AnswerButton> answerButtons = new List<AnswerButton>();
    private int correctAnswerButtonId;
    private CanvasGroup canvasGroup;
    private TextMeshProUGUI textSlerp;
    #endregion



    private void Awake()
    {
        Singlton = this;
        canvasGroup = GetComponent<CanvasGroup>();
        startEquationWidth = equationRect.rect.width;

        textLeft.DOFade(0, 0);
        textX.DOFade(0, 0);
        textRigt.DOFade(0, 0);
        canvasGroup.alpha = 0;

        firstButtonPositionX = spawnPoint.position.x - answerButtonSpacing * (answersCount - 1) / 2;

        for (int i = 0; i < answersCount; i++)
        {
            var button = Instantiate(answerButtonPrefab, spawnPoint.position, Quaternion.identity, transform);
            answerButtons.Add(button);
            button.Init(i);
        }

        textSlerp = Instantiate(textX, transform);
        textSlerp.DOFade(0, 0);
    }

    private void Start()
    {
        StartCoroutine(LevelSiquanceCoroutine());
    }



    public void CheckAnswerClick(AnswerButton button)
    {
        if (button.IsCorrectAnswer)
        {
            IsInput = false;
        }
        else
        {

        }
    }

    private IEnumerator LevelSiquanceCoroutine()
    {
        // start appear
        yield return new WaitForSeconds(1);
        yield return canvasGroup.DOFade(1, animDuration).WaitForCompletion();

        for (int i = 0; i < levelEquationCount; i++)
        {
            UpdateEquation();

            // appear
            SetTextAnswers();
            
            for (int j = 0; j < answerButtons.Count; j++) answerButtons[j].MoveToPosition();
            yield return equationRect.DOSizeDelta(new Vector2(equation.length * pixelsPerChar, equationRect.rect.height), animDuration).WaitForCompletion();
            

            for (int j = 0; j < answerButtons.Count; j++) answerButtons[j].FadeIn();
            textLeft.DOFade(1, animDuration);
            textX.DOFade(1, animDuration);
            yield return textRigt.DOFade(1, animDuration).WaitForCompletion();

            IsInput = true;



            yield return new WaitWhile(() => IsInput);
            textX.DOFade(0, animDuration);
            yield return StartCoroutine(AnimateSlerpAnswer(answerButtons[correctAnswerButtonId].transform.position, animDuration, animDuration));



            // disappear
            for (int j = 0; j < answerButtons.Count; j++) answerButtons[j].FadeOut();
            textLeft.DOFade(0, animDuration);
            textSlerp.DOFade(0, animDuration);
            yield return textRigt.DOFade(0, animDuration).WaitForCompletion();

            for (int j = 0; j < answerButtons.Count; j++) answerButtons[j].MoveToStart();
            yield return equationRect.DOSizeDelta(new Vector2(startEquationWidth, equationRect.rect.height), animDuration).WaitForCompletion();
        }

        IsInput = false;
        // end animation
        for (int j = 0; j < answerButtons.Count; j++) answerButtons[j].FinishFadeIn();
        backlight.DOFade(1, animDuration);
    }

    private void SetTextAnswers()
    {
        correctAnswerButtonId = Random.Range(0, answersCount);
        answerButtons[correctAnswerButtonId].SetAnswer(equation.correctAnswer);

        int incorrectAnswerCount = Mathf.Min(answersCount - 1, equation.incorrectAnswers.Length); // чего меньше, пустых кнопок или вручную заданных неверных ответов
        int incorrectIndex = 0;
        int manualButtonInstertIndex = 0;
        for (int i = 0; i < answerButtons.Count; i++) // заполняем остальные кнопки неправильными ответами
        {
            if (i == correctAnswerButtonId) continue;

            if (manualButtonInstertIndex < incorrectAnswerCount)
            {
                // вносим вручную заданный неверный ответ
                answerButtons[i].SetAnswer(equation.incorrectAnswers[incorrectIndex]);
                incorrectIndex++;
                manualButtonInstertIndex++;
            }
            else
            {
                int incorrectPart = Random.Range(incorrectDisplacement.x, incorrectDisplacement.y);
                while(incorrectPart == 0) incorrectPart = Random.Range(-2, 2);
                // заданных неверных ответов не хватило на все кнопки, придумываем свои неверные ответы
                answerButtons[i].SetAnswer(equation.correctAnswer + incorrectPart);
            }
        }
    }

    private IEnumerator AnimateSlerpAnswer(Vector3 startPosition, float duration, float durationAfterMove = 0)
    {
        int slerpSteps = 50;
        float stepDuration = duration / slerpSteps;

        textSlerp.transform.position = startPosition;
        textSlerp.rectTransform.sizeDelta = textX.rectTransform.sizeDelta;
        textSlerp.text = equation.correctAnswer.ToString();
        textSlerp.DOFade(1, 0);

        for (int i = 0; i < slerpSteps; i++)
        {
            textSlerp.transform.position = Vector3.Slerp(startPosition, textX.transform.position, i / (float)slerpSteps);
            yield return new WaitForSeconds(stepDuration);
        }

        if (durationAfterMove > 0) yield return new WaitForSeconds(durationAfterMove);
    }

    private void UpdateEquation()
    {
        equation = levelMaths.GetEquation();
        textLeft.text = equation.expressionLeft;
        textRigt.text = equation.expressionRight;
    }
}