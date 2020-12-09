using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day9 : MonoBehaviour
{
    public const int Day = 9;

    void Start()
    {
        StartCoroutine(coDay());
    }

    IEnumerator coDay()
    {
        yield return StartCoroutine(Tools.Instance.GetInput(Day));
        while (Tools.Instance.Input == "")
        {
            yield return new WaitForEndOfFrame();
        }

        day_1();

        yield return new WaitForEndOfFrame();

        day_2();

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(5f);
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void day_1()
    {
        long result = 0;


        int index = findFirstInvalidNumberIndex();
        if (index >= 0)
            result = Tools.Instance.Input.Split('\n').Select(long.Parse).ToArray()[index];


        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }


    void day_2()
    {
        long result = 0;

        int index = findFirstInvalidNumberIndex();
        long[] numbers = Tools.Instance.Input.Split('\n').Select(long.Parse).ToArray();

        if (index < 0 || index >= numbers.Length)
        {
            Debug.LogError("[Day9] Invalid number not found or wrong");
            return;
        }

        for (int i = 0; i < index; i++)
        {
            int validK = -1;
            long currentSum = 0, min = long.MaxValue, max = 0;
            for (int k = i; k < index; k++)
            {
                if (numbers[i + k] < min)
                    min = numbers[i + k];
                if (numbers[i + k] > max)
                    max = numbers[i + k];

                currentSum += numbers[i + k];
                if (currentSum == numbers[index])
                {
                    validK = k;
                    break;
                }
                else if (currentSum > numbers[index])
                    break;
            }

            if (validK > 0)
            {
                result = min + max;
                break;
            }

        }
        
        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }

    // returns index of first invalid number (is not sum of 2 number in the 25 previous numbers in the list)
    int findFirstInvalidNumberIndex()
    {
        int result = -1;

        long[] numbers = Tools.Instance.Input.Split('\n').Select(long.Parse).ToArray();

        HashSet<long> waitedNumbers = new HashSet<long>();

        string log = "";
        for (int i = 25; i < numbers.Length; i++)
        {
            bool debug = false;
            log = "index i is '" + i + "' and value is '" + numbers[i] + "'" + System.Environment.NewLine;

            bool valid = false;
            waitedNumbers = new HashSet<long>();
            for (int k = -25; k < 0; k++)
            {
                log += "   -> k=" + k + " value is " + numbers[i + k];
                if (waitedNumbers.Contains(numbers[i + k]))
                {
                    valid = true;
                    break;
                }
                else
                {
                    log += "\t--> not in waitedNumbers so adding " + (numbers[i] - numbers[i + k]).ToString();
                    waitedNumbers.Add(numbers[i] - numbers[i + k]);
                }
                log += System.Environment.NewLine;
            }

            if (!valid)
            {
                log += System.Environment.NewLine + "NOT VALID";
                result = i;
            }

            if (debug || !valid)
                Debug.Log(log);


            if (result >= 0)
                break;
        }

        return result;
    }

}
