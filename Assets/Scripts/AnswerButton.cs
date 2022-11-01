using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(Button), typeof(RectTransform))]
public class AnswerButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    public bool IsCorrectAnswer => text.text == builder.equation.correctAnswer.ToString();

    [SerializeField] Color incorrectColor;
    [SerializeField] Color correctColor;

    private EquationBuilder builder => EquationBuilder.Singlton;

    private Image image;
    private Color defaultColor;
    private int buttonId;

    private void Awake()
    {
        image = GetComponent<Image>();
        defaultColor = image.color;
        text.DOFade(0, 0);

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
            image.DOColor(correctColor, builder.animDuration);
        else
            image.DOColor(incorrectColor, builder.animDuration);
    }

    public async void Appear()
    {
        await transform.DOMoveX(builder.firstButtonPositionX + buttonId * builder.answerButtonSpacing, builder.animDuration).AsyncWaitForCompletion();
        await text.DOFade(1, builder.animDuration).AsyncWaitForCompletion();
    }
    public async void Disappear()
    {
        image.DOColor(defaultColor, builder.animDuration);
        await text.DOFade(0, builder.animDuration).AsyncWaitForCompletion();
        await transform.DOMoveX(builder.spawnPoint.position.x, builder.animDuration).AsyncWaitForCompletion();
    }
}