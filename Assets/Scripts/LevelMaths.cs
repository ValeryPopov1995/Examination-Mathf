using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Equation Base", menuName = "Scriptable/Equation Base")]
public class LevelMaths : ScriptableObject
{
    [field: SerializeField]
    private List<EquationPrefab> equations;

    public Equation GetEquation()
    {
        return equations[Random.Range(0, equations.Count)].GetEquation();
    }
}

[Serializable]
public class EquationPrefab
{
    [Tooltip("Впиши мат.выражение без ответа и символа '='")]
    [OnValueChanged("updateAnswer"), AllowNesting]
    public string equation = "(4+5)*2";

    [Tooltip("Через запятую номера чисел, которые можно заменять на неизвестное Х")]
    public string numbersIDforX = "1,2";

    [ReadOnly, AllowNesting]
    public float resultInfo;
    public float? result;

    public bool manualIncorrectAnswers = false;
    [ShowIf("manualIncorrectAnswers"), AllowNesting]
    public float[] incorrectAnswers;

    public bool hasAnswer => result != null;
    private void updateAnswer()
    {
        try
        {
            var dataTable = new DataTable();
            result = float.Parse(dataTable.Compute(equation, null).ToString());
            resultInfo = result.Value;
        }
        catch
        {
            result = null;
            resultInfo = 0;
        }
    }


    public Equation GetEquation()
    {
        var ids = numbersIDforX
            .Split(',', options: StringSplitOptions.RemoveEmptyEntries)
            .Select(x => int.Parse(x)); // starts from 1

        int targetID = ids.ElementAt(Random.Range(0, ids.Count()));

        var numbers = equation.Split(
            new char[] { '(', ')', '+', '-', '*', '/' },
            options: StringSplitOptions.RemoveEmptyEntries)
            .Select(x => float.Parse(x));

        int charXIndex = GetXIndex(targetID);

        string afterXEquation = equation.Substring(charXIndex).TrimStart('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
        string equationXResult = equation.Substring(0, charXIndex) + "X" + afterXEquation + "=" + resultInfo;

        var newEquation = new Equation();
        newEquation.expressionXResult = equationXResult;
        newEquation.correctAnswer = numbers.ElementAt(targetID - 1);
        if (manualIncorrectAnswers)
            newEquation.incorrectAnswers = incorrectAnswers;
        else
            newEquation.incorrectAnswers = new float[0];

        return newEquation;
    }

    /// <summary>
    /// Находит Х в мат.выражении
    /// </summary>
    /// <param name="targetID"></param>
    /// <returns>Номер символа в строке, который является первой цифрой искомого числа</returns>
    private int GetXIndex(int targetID)
    {
        bool isFirstDigit = true;
        int currentID = 0;
        int charXIndex = 0; // номер символа в строке, с которого начинается число, заменяемое на Х
        for (int i = 0; i < equation.Length; i++)
        {
            if (char.IsDigit(equation[i]))
            {
                if (isFirstDigit) // нашли первую цифру в следующем попавшемся числе в мат.выражении
                {
                    currentID++;
                    isFirstDigit = false;
                }

                if (currentID == targetID) // нашли то самое число, которое нужно заменить на Х
                {
                    charXIndex = i;
                    break;
                }
            }
            else
            {
                isFirstDigit = true;
            }
        }

        return charXIndex;
    }
}

public class Equation
{
    public string expressionLeft => expressionXResult.Split('X')[0];
    public string expressionRight => expressionXResult.Split('X')[1];


    public int length => expressionXResult.Length;

    /// <summary>
    /// Строка мат.выражения с 'X' вместо одного из чисел и равенством
    /// </summary>
    public string expressionXResult;

    public float correctAnswer;

    public float[] incorrectAnswers;
}