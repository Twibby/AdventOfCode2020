using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day18 : MonoBehaviour
{
    public bool IsDebug = false;

    private Dictionary<string, long> testInputs = new Dictionary<string, long>();

    void Start()
    {
        testInputs.Add("1 + (2 * 3) + (4 * (5 + 6))", 51);
        testInputs.Add("2 * 3 + (4 * 5)", 26);
        testInputs.Add("5 + (8 * 3 + 9 + 3 * 4 * 3)", 437);
        testInputs.Add("5 * 9 * (7 * 3 * 3 + 9 * 3 + (8 + 6 * 4))", 12240);
        testInputs.Add("((2 + 4 * 9) * (6 + 9 * 8 + 6) + 6) + 2 + 4 * 2", 13632);

        StartCoroutine(coDay());
    }

    IEnumerator coDay()
    {
        Debug.LogWarning("Day is : " + this.GetType().ToString().Substring(3));

        yield return StartCoroutine(Tools.Instance.GetInput(this.GetType().ToString().Substring(3)));
        while (Tools.Instance.Input == "")
        {
            yield return new WaitForEndOfFrame();
        }

        day_1();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        day_2();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        alt_Day_2();


        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(5f);
        UnityEditor.EditorApplication.isPlaying = false;
    }


    void day_1()
    {
        long result = 0;

        List<string> inputs =  new List<string>(Tools.Instance.Input.Split('\n'));
        
        foreach (string calcul in inputs)
        {
            //Debug.Log(calcul + "  ==>  " + evaluateExpression(calcul));
            result += evaluateExpression(calcul);
        }


        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }


    void day_2()
    {
        long result = 0;

        List<string> inputs = new List<string>(Tools.Instance.Input.Split('\n')); // new List<string>(".#.\n..#\n###".Split('\n'));

        foreach (string calcul in inputs)
        {
            if (IsDebug)
                Debug.Log(calcul + "  ==>  " + evaluateExpressionWithPrevalence(calcul));

            result += evaluateExpressionWithPrevalence(calcul);
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }

    void alt_Day_2()
    {
        long result = 0;

        List<string> inputs = new List<string>(Tools.Instance.Input.Split('\n')); // new List<string>(".#.\n..#\n###".Split('\n'));

        foreach (string calcul in inputs)
        {
            if (IsDebug)
                Debug.Log(calcul + "  ==>  " + altEvaluateExpressionWithPrevalence(calcul));

            result += altEvaluateExpressionWithPrevalence(calcul);
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Alt Part 2 result is : " + result.ToString());
    }

    long evaluateExpression(string expression)
    {
        int val = 0;    // final case
        if (int.TryParse(expression.Trim(), out val))
            return val;

        if (expression[expression.Length-1] == ')')
        {
            //recursive call to evaluate expression between parenthesis. Loof for equivalent opening parenthesis
            int parenthesisCount = 0, correspondingOpenParenthsisIndex = -1;
            for (int i = expression.Length-1; i >= 0; i--)
            {
                if (expression[i] == ')')
                    parenthesisCount++;
                else if (expression[i] == '(')
                {
                    parenthesisCount--;
                    if (parenthesisCount < 0)
                    {
                        Debug.LogError(") expression malformed : " + expression);
                        return 0;
                    }
                    else if (parenthesisCount == 0)
                    {
                        correspondingOpenParenthsisIndex = i;
                        break;
                    }
                }
            }

            if (correspondingOpenParenthsisIndex < 0 || correspondingOpenParenthsisIndex == 1 || correspondingOpenParenthsisIndex == 2 || correspondingOpenParenthsisIndex == 3)
            {
                Debug.LogError(") expression malformed : " + expression);
                return 0;
            }
            else if (correspondingOpenParenthsisIndex == 0)
            {
                return evaluateExpression(expression.Substring(1, expression.Length - 2));
            }
            else
            {
                char operande = expression[correspondingOpenParenthsisIndex - 2];
                if (operande == '+')
                    return evaluateExpression(expression.Substring(correspondingOpenParenthsisIndex+1, expression.Length - correspondingOpenParenthsisIndex -2)) + evaluateExpression(expression.Substring(0, correspondingOpenParenthsisIndex - 3));
                else if (operande == '*')
                    return evaluateExpression(expression.Substring(correspondingOpenParenthsisIndex + 1, expression.Length - correspondingOpenParenthsisIndex - 2)) * evaluateExpression(expression.Substring(0, correspondingOpenParenthsisIndex - 3));
                else
                {
                    Debug.LogError(") operande is '" + operande + "' in expression : " + expression);
                    return 0;
                }
            }
        }
        else
        {
            val = 0;
            int lastSpaceIndex = expression.LastIndexOf(' ');
            if (lastSpaceIndex > 0 && lastSpaceIndex < expression.Length - 1 && int.TryParse(expression.Substring(lastSpaceIndex + 1, expression.Length - lastSpaceIndex - 1), out val))
            {   // parse ok, should always be there
                char operande = expression[lastSpaceIndex - 1];
                if (operande == '+')
                    return val + evaluateExpression(expression.Substring(0, lastSpaceIndex - 2));
                else if (operande == '*')
                    return val * evaluateExpression(expression.Substring(0, lastSpaceIndex - 2));
                else
                {
                    Debug.LogError("operande is '" + operande + "' in expression : " + expression);
                    return 0;
                }
            }
            else
            {
                Debug.LogError("expression malformed : " + expression);
                return 0;
            }
        }
    }

    long evaluateExpressionWithPrevalence(string expression)
    {
        long val = 0;    // final case
        if (long.TryParse(expression.Trim(), out val))
            return val;

        Dictionary<char, int> prevalence = new Dictionary<char, int>();
        prevalence.Add('+', 3);
        prevalence.Add('*', 2);
        prevalence.Add('(', 1);

        Stack operandeStack = new Stack();
        List<string> orderedValues = new List<string>();
        bool isNumber = false;
        string tmpNumber = "";

        // First step sort expression depending on prevalence of operandes
        foreach (char c in expression)
        {
            if ((int)c >= (int)'0' && (int)c <= (int)'9')
            {
                isNumber = true;
                tmpNumber += c;
            }
            else
            {
                switch (c)
                {
                    case ' ':
                        if (isNumber)
                        {
                            isNumber = false;
                            orderedValues.Add(tmpNumber);
                            tmpNumber = "";
                        }
                        break;

                    case '+':
                    case '*':
                        if (operandeStack.Count > 0)
                        {
                            char top = (char)operandeStack.Pop();
                            if (top != '(' && prevalence[top] >= prevalence[c])
                                orderedValues.Add(top + "");
                            else
                            {
                                operandeStack.Push(top);
                            }
                        }                        
                        operandeStack.Push(c);
                        break;

                    case '(':
                        operandeStack.Push(c);
                        break;
                    case ')':
                        if (isNumber)
                        {
                            isNumber = false;
                            orderedValues.Add(tmpNumber);
                            tmpNumber = "";
                        }
                        char stackTop;
                        while (operandeStack.Count > 0 && (stackTop = (char)operandeStack.Pop()) != '(')
                        {
                            orderedValues.Add(stackTop + "");
                        }
                        break;

                    default:
                        Debug.Log("Error in Parse, char is : " + c + " in expression " + expression);
                        break;
                }
            }
        }

        if (isNumber)
            orderedValues.Add(tmpNumber);

        while (operandeStack.Count > 0)
        {
            orderedValues.Add((char)operandeStack.Pop() + "");
        }

        //Debug string to be sure all formula is here in the right order
        if (IsDebug)
        {
            string s = "";
            foreach (string c in orderedValues)
            {
                s += c + ", ";
            }
            Debug.Log(s);
        }

        // go through ordered values to compute everything
        Stack<long> intStack = new Stack<long>();
        foreach (string elmt in orderedValues)
        {
            val = 0;
            if (long.TryParse(elmt, out val))
            {
                intStack.Push(val);
            }
            else
            {
                switch (elmt)
                {
                    case "+":
                        if (intStack.Count < 2)
                            Debug.LogError("error in formula");
                        else
                        {
                            long b = intStack.Pop();
                            long a = intStack.Pop();
                            intStack.Push(a + b);
                        }
                        break;

                    case "*":
                        if (intStack.Count < 2)
                            Debug.Log("error in formula");
                        else
                        {
                            long b = intStack.Pop();
                            long a = intStack.Pop();
                            intStack.Push(a * b);
                        }
                        break;
                }
            }
        }

        if (intStack.Count != 1)
        {
            Debug.Log("error, intStack count = " + intStack.Count);
            return 0;
        }
        else
        {
            return intStack.Pop();
        }
    }

    long altEvaluateExpressionWithPrevalence(string expression)
    {
        long val = 0;    // final case
        if (long.TryParse(expression.Trim(), out val))
            return val;

        int indexOf = expression.IndexOf('(');
        if (indexOf >= 0)    // there is some parenthesis, evaluate it and replace it in full expression
        {
            int parenthesisCount = 0, correspondingOpenParenthsisIndex = -1;
            for (int i = indexOf; i < expression.Length; i++)
            {
                if (expression[i] == '(')
                    parenthesisCount++;
                else if (expression[i] == ')')
                {
                    parenthesisCount--;
                    if (parenthesisCount < 0)
                    {
                        Debug.LogError(") expression malformed : " + expression);
                        return 0;
                    }
                    else if (parenthesisCount == 0)
                    {
                        correspondingOpenParenthsisIndex = i;
                        break;
                    }
                }
            }


            if (correspondingOpenParenthsisIndex < 0 || correspondingOpenParenthsisIndex == 1 || correspondingOpenParenthsisIndex == 2 || correspondingOpenParenthsisIndex == 3)
            {
                Debug.LogError(") expression malformed : " + expression);
                return 0;
            }

            if (IsDebug)
            {
                Debug.Log("string is " + (indexOf > 0 ? expression.Substring(0, indexOf) : "")
                    + altEvaluateExpressionWithPrevalence(expression.Substring(indexOf + 1, correspondingOpenParenthsisIndex - indexOf - 1)).ToString()
                    + (correspondingOpenParenthsisIndex == expression.Length - 1 ? "" : expression.Substring(correspondingOpenParenthsisIndex + 1))
                    );
            }

            return altEvaluateExpressionWithPrevalence
                ( (indexOf > 0 ? expression.Substring(0, indexOf) : "")
                + altEvaluateExpressionWithPrevalence(expression.Substring(indexOf + 1, correspondingOpenParenthsisIndex - indexOf - 1)).ToString()
                + (correspondingOpenParenthsisIndex == expression.Length - 1 ? "" : expression.Substring(correspondingOpenParenthsisIndex + 1))
                );
        }
        else
        {
            indexOf = expression.IndexOf('*');
            if (indexOf > 0)
            {
                return altEvaluateExpressionWithPrevalence(expression.Substring(0, indexOf - 1))
                    * altEvaluateExpressionWithPrevalence(expression.Substring(indexOf + 2));
            }
            else
            {
                indexOf = expression.IndexOf('+');
                if (indexOf > 0)
                {
                    return altEvaluateExpressionWithPrevalence(expression.Substring(0, indexOf - 1))
                        + altEvaluateExpressionWithPrevalence(expression.Substring(indexOf + 2));
                }
                else
                {
                    Debug.LogError("we should not be here because if no ()+* that means it's only int and it's the early return : " + expression);
                    return 0;
                }
            }
        }
    }
}
