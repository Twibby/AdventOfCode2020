using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day13 : MonoBehaviour
{
    public bool IsDebug = false;

    void Start()
    {
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

        day_2();

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(5f);
        UnityEditor.EditorApplication.isPlaying = false;
    }


    void day_1()
    {
        int result = 0;

        int departTime = int.Parse(Tools.Instance.Input.Split('\n')[0]);
        string[] busIDs = Tools.Instance.Input.Split('\n')[1].Split(',');

        int minID = int.MaxValue, minTime = int.MaxValue;
        foreach (string id in busIDs)
        {
            
            if (id == "x")
                continue;

            int val = -1;
            if (int.TryParse(id, out val))
            {
                int waitingTime = (val - (departTime % val)) %val;
                if (waitingTime < minTime)
                {
                    minID = val;
                    minTime = waitingTime;
                }
            }
            else
            {
                Debug.LogError("[Day13] Bus malformed : " + id);
                continue;
            }
        }
        Debug.LogWarning("minID is " + minID + " & minTime is : " + minTime);
        result = minID * minTime;

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }


    void day_2()
    {
        string test1 = "17,x,13,19";
        string test2 = "67,7,59,61";
        string test3 = "67,x,7,59,61";
        string test4 = "67,7,x,59,61";
        string test5 = "1789,37,47,1889";

        string input = Tools.Instance.Input.Split('\n')[1];

        bool valid = true;
        Debug.Log("Starting Exemple 1");
        long res = auxPart2(test1);
        valid = (res == 3417);
        if (valid)
            Debug.LogWarning("Example 1 successful, continue");
        else
        {
            Debug.LogError("Example failed");
            return;
        }

        Debug.Log("Starting Exemple 2");
        res = auxPart2(test2);
        valid = (res == 754018);
        if (valid)
            Debug.LogWarning("Example 2 successful, continue");
        else
        {
            Debug.LogError("Example failed");
            return;
        }

        Debug.Log("Starting Exemple 3");
        res = auxPart2(test3);
        valid = (res == 779210);
        if (valid)
            Debug.LogWarning("Example 3 successful, continue");
        else
        {
            Debug.LogError("Example failed");
            return;
        }

        Debug.Log("Starting Exemple 4");
        res = auxPart2(test4);
        valid = (res == 1261476);
        if (valid)
            Debug.LogWarning("Example 4 successful, continue");
        else
        {
            Debug.LogError("Example failed");
            return;
        }

        Debug.Log("Starting Exemple 5");
        res = auxPart2(test5);
        valid = (res == 1202161486);
        if (valid)
            Debug.LogWarning("Example 5 successful, continue");
        else
        {
            Debug.LogError("Example failed");
            return;
        }

        Debug.Log("Starting Real Input");
        res = auxPart2(input);
    }

    long auxPart2(string input)
    {
        long result = 0;

        List<KeyValuePair<int, int>> busIDs = new List<KeyValuePair<int, int>>();

        //Creating list
        int index = 0, max = 0, maxIndex = 0;
        long product = 1;
        foreach (string id in input.Split(','))
        {
            if (id == "x")
            {
                index++;
                continue;
            }

            int val = -1;
            if (int.TryParse(id, out val))
            {
                busIDs.Add(new KeyValuePair<int, int>(index, val));
                if (val > max)
                {
                    max = val;
                    maxIndex = index;
                }
                product *= val;
                index++;
            }
            else
            {
                index++;
                Debug.LogError("[Day13] Bus malformed : " + id);
                continue;
            }
        }

        // Idea n°2 : find solution for the 2 first bus ids A and B then add A*B until having a solution for third id C then add A*B*C etc...
        long A = busIDs[0].Value;
        long solution = A;
        for (int i = 1; i < busIDs.Count; i++)
        {
            int B = busIDs[i].Value, offset = busIDs[i].Key;
            while ((solution + offset) % B != 0)
            {
                solution += A;
            }

            A *= B;
        }
        result = solution;

        #region bad first idea
        // IDEA n°1 (bad) : """smart""" brutforce with max factor as increment
        //for (long k = product / max; k > 0; k--)
        //{
        //    bool valid = true;
        //    foreach (KeyValuePair<int, int> id in busIDs)
        //    {
        //        if (id.Value == max)
        //            continue;
        //
        //        long offsetTime = k * max + (id.Key - maxIndex);
        //        if (offsetTime % id.Value != 0)
        //        {
        //            valid = false;
        //            break;
        //        }
        //    }
        //
        //    if (valid)
        //    {
        //        result = k * max - maxIndex;
        //        break;
        //    }
        //}
        #endregion

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
        return result;
    }
}
