using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day2 : MonoBehaviour
{
    public const int Day = 2;

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
        yield return new WaitForSeconds(1f);
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void day_1()
    {
        int count = 0;
        foreach (string line in Tools.Instance.Input.Split('\n'))
        {
            string[] parses = line.Split(' ');
            if (parses.Length != 3)
            {
                Debug.LogError("[day_1] parses length is not 3 --> " + line);
                continue;
            }

            string[] bounds = parses[0].Split('-');
            if (bounds.Length != 2)
            {
                Debug.LogError("[day_1] bounds length is not 2 --> " + parses[0]);
                continue;
            }

            int minBound = -1, maxBound = -1;
            if (int.TryParse(bounds[0], out minBound) && int.TryParse(bounds[1], out maxBound))
            {
                char c = parses[1][0];

                int cCount = 0;
                for (int i = 0; i < parses[2].Length; i++) { if (parses[2][i] == c) { cCount++; } }

                if (cCount <= maxBound && cCount >= minBound)
                    count++;
            }
            else
            {
                Debug.LogError("[day_1] bounds not valid : " + parses[0]);
                continue;
            }
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + count.ToString());
    }

    void day_2()
    {
        int count = 0;
        foreach (string line in Tools.Instance.Input.Split('\n'))
        {
            string[] parses = line.Split(' ');
            if (parses.Length != 3)
            {
                Debug.LogError("[day_2] parses length is not 3 --> " + line);
                continue;
            }

            string[] bounds = parses[0].Split('-');
            if (bounds.Length != 2)
            {
                Debug.LogError("[day_2] bounds length is not 2 --> " + parses[0]);
                continue;
            }

            int minBound = -1, maxBound = -1;
            if (int.TryParse(bounds[0], out minBound) && int.TryParse(bounds[1], out maxBound))
            {
                if (minBound < 1 || minBound > parses[2].Length 
                    || maxBound < 1 || maxBound > parses[2].Length)
                {
                    Debug.LogError("[day_2] minBound or maxBound is not correctly set : " + minBound + " & " + maxBound + " | " + line);
                }
                minBound--;
                maxBound--;

                char c = parses[1][0];

                if (parses[2][minBound] == c && parses[2][maxBound] != c)
                    count++;
                else if (parses[2][minBound] != c && parses[2][maxBound] == c)
                    count++;
            }
            else
            {
                Debug.LogError("[day_2] bounds not valid : " + parses[0]);
                continue;
            }
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + count.ToString());
    }
}
