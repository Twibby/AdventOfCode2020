using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day5 : MonoBehaviour
{
    public const int Day = 5;

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
        int result = 0;

        foreach (string line in Tools.Instance.Input.Split('\n'))
        {
            if (line == "")
                continue;

            if (line.Length != 10)
            {
                Debug.LogError("[" + this.GetType().ToString() + "] day_1, line is malformed : " + line);
                continue;
            }

            int row = aux(line[0]) * 64 + aux(line[1]) * 32 + aux(line[2]) * 16 + aux(line[3]) * 8 + aux(line[4]) * 4 + aux(line[5]) * 2 + aux(line[6]) * 1;
            int column = aux(line[7]) * 4 + aux(line[8]) * 2 + aux(line[9]) * 1;

            int seatID = row * 8 + column;

            result = Mathf.Max(seatID, result);
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }

    void day_2()
    {
        int result = 0;

        List<int> takenSeats = new List<int>();
        
        foreach (string line in Tools.Instance.Input.Split('\n'))
        {
            if (line == "")
                continue;

            if (line.Length != 10)
            {
                Debug.LogError("[" + this.GetType().ToString() + "] day_1, line is malformed : " + line);
                continue;
            }

            int row = aux(line[0]) * 64 + aux(line[1]) * 32 + aux(line[2]) * 16 + aux(line[3]) * 8 + aux(line[4]) * 4 + aux(line[5]) * 2 + aux(line[6]) * 1;
            int column = aux(line[7]) * 4 + aux(line[8]) * 2 + aux(line[9]) * 1;

            int seatID = row * 8 + column;
            takenSeats.Add(seatID);
        }

        //takenSeats.Sort();
        //for (int i=0; i < takenSeats.Count-1; i++)
        //{
        //    Debug.Log(takenSeats[i]);
        //    if (takenSeats[i + 1] == takenSeats[i] + 2)
        //    {
        //        result = takenSeats[i] + 1;
        //        break;
        //    }
        //}

        foreach (int seat in takenSeats)
        {
            if (takenSeats.Contains(seat + 2) && !takenSeats.Contains(seat + 1))
            {
                result = seat + 1;
                break;
            }
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }

    int aux(char c) { return (c == 'B' || c == 'R' ? 1 : 0) ; }
}
