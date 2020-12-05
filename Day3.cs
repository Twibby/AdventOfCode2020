using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day3 : MonoBehaviour
{
    public const int Day = 3;

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
        int index = 0;
        foreach (string line in Tools.Instance.Input.Split('\n'))
        {
            if (line == "")
                continue;

            if (line[(index * 3) % line.Length] == '#')
                count++;

            index++;
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + count.ToString());
    }

    void day_2()
    {
        int count1_1 = 0, count3_1 = 0, count5_1 = 0, count7_1 = 0, count1_2 = 0;
        int index = 0;
        foreach (string line in Tools.Instance.Input.Split('\n'))
        {
            if (line == "")
                continue;

            if (line[(index * 1) % line.Length] == '#')
                count1_1++;

            if (line[(index * 3) % line.Length] == '#')
                count3_1++;

            if (line[(index * 5) % line.Length] == '#')
                count5_1++;

            if (line[(index * 7) % line.Length] == '#')
                count7_1++;

            if (index % 2 == 0 && line[(index / 2) % line.Length] == '#')
                count1_2++;

            index++;
        }

        long product = count1_1 * count3_1 * count5_1 * count7_1 * count1_2;
        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + count1_1 + " & " + count3_1 + " & " + count5_1 + " & " + count7_1 + " & " + count1_2 + " --> Product is " + product);
    }
}
