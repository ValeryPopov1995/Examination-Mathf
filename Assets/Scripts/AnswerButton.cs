using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class AnswerButton : MonoBehaviour
{
    public bool IsCorrectAnswer;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {

    }

    public void Reset()
    {
        IsCorrectAnswer = false;
    }
}