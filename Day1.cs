using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Day1 : MonoBehaviour
{
    public const int Day = 1;

    void Start()
    {
        StartCoroutine(coDay1());
    }

    IEnumerator coDay1()
    {
        yield return StartCoroutine(Tools.Instance.GetInput(Day));
        while (Tools.Instance.Input == "")
        {
            yield return new WaitForEndOfFrame();
        }

        coDay1_1();

        yield return new WaitForEndOfFrame();

        coDay1_2();

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(1f);
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void coDay1_1()
    {
        string result = "Not found";

        int total = 2020, current = -1;
        List<int> waitedInt = new List<int>();
        foreach (string number in Tools.Instance.Input.Split('\n'))
        {
            if (int.TryParse(number, out current))
            {
                if (waitedInt.Contains(current))
                {
                    result = "Found numbers " + current + " & " + (total - current)
                        + " ! Product is : " + (current * (total - current)).ToString();
                    break;
                }
                else
                    waitedInt.Add(total - current);
            }
            else
                Debug.LogError("[Day1] can't parse int : " + number);
        }

        Debug.LogWarning(result);
    }

    void coDay1_2()
    {
        string result = "Not found";

        int total = 2020, current = -1;
        List<int> parsedInt = new List<int>();
        foreach (string number in Tools.Instance.Input.Split('\n'))
        {
            if (int.TryParse(number, out current))
            {
                for (int i = 0; i < parsedInt.Count; i++)
                {
                    for (int j=i+1; j < parsedInt.Count; j++)
                    {
                        if (parsedInt[i] + parsedInt[j] + current == total)
                        {
                            result = "Found numbers " + parsedInt[i] + " & " + parsedInt[j] + " & " + current
                        + " ! Product is : " + (parsedInt[i] * parsedInt[j] * current).ToString();

                            break;
                        }
                    }

                    if (result != "Not found")
                        break;
                }

                if (result != "Not found")
                    break;
                else
                    parsedInt.Add(current);
            }
            else
                Debug.LogError("[Day1-2] can't parse int : " + number);
        }

        Debug.LogWarning(result);
    }
}
