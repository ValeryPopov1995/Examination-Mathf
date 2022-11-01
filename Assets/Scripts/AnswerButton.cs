using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(Button), typeof(RectTransform))]
public class AnswerButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    public bool IsCorrectAnswer => text.text == builder.equation.correctAnswer.ToString();

    [SerializeField] Color correctColor;
    [SerializeField] Color incorrectColor;
    [SerializeField] Image backLight;

    private Color correctBacklightColor;
    private Color incorrectBacklightColor;

    private EquationBuilder builder => EquationBuilder.Singlton;

    private Image background;
    private Color defaultColor;
    private Color defaultBacklightColor;
    private int buttonId;

    private void Awake()
    {
        background = GetComponent<Image>();
        defaultColor = background.color;
        defaultBacklightColor = backLight.color;
        text.DOFade(0, 0);

        correctBacklightColor = new Color(correctColor.r, correctColor.g, correctColor.b, backLight.color.a);
        incorrectBacklightColor = new Color(incorrectColor.r, incorrectColor.g, incorrectColor.b, backLight.color.a);

        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    public void Init(int buttonId)
    {
        this.buttonId = buttonId;
    }

    public void SetAnswer(float answer)
    {
        text.text = answer.ToString();
    }

    private void OnButtonClick()
    {
        if (!builder.IsInput) return;

        builder.CheckAnswerClick(this);

        if (IsCorrectAnswer)
        {
            background.DOColor(correctColor, builder.animDuration);
            backLight.DOColor(correctBacklightColor, builder.animDuration);
        }
        else
        {
            background.DOColor(incorrectColor, builder.animDuration);
            backLight.DOColor(incorrectBacklightColor, builder.animDuration);
        }
    }

    public void MoveToPosition() => transform.DOMoveX(builder.firstButtonPositionX + buttonId * builder.answerButtonSpacing, builder.animDuration);
    public void MoveToStart() => transform.DOMoveX(builder.spawnPoint.position.x, builder.animDuration);
    public void FadeIn() => text.DOFade(1, builder.animDuration);
    public void FadeOut()
    {
        background.DOColor(defaultColor, builder.animDuration);
        backLight.DOColor(defaultBacklightColor, builder.animDuration);
        text.DOFade(0, builder.animDuration);
    }
    public void FinishFadeIn()
    {
        background.DOFade(1, builder.animDuration);
        backLight.DOFade(1, builder.animDuration);
    }
}