using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day23 : MonoBehaviour
{
    public bool IsDebug = true;
    public bool IsTestInput = true;
    public bool Part1 = true;
    public bool Part2 = false;

    private string _input;

    void Start()
    {
        StartCoroutine(coDay());
    }

    IEnumerator coDay()
    {
        Debug.LogWarning("Day is : " + this.GetType().ToString().Substring(3));

        if (!IsTestInput)
        {
            yield return StartCoroutine(Tools.Instance.GetInput(this.GetType().ToString().Substring(3)));
            while (Tools.Instance.Input == "")
            {
                yield return new WaitForEndOfFrame();
            }
            _input = Tools.Instance.Input;
        }
        else
        {
            yield return StartCoroutine(Tools.Instance.GetTestInput("https://ollivier.iiens.net/AoC/2020/" + this.GetType().ToString().Substring(3) + ".txt"));
            while (Tools.Instance.Input == "")
            {
                yield return new WaitForEndOfFrame();
            }
            _input = Tools.Instance.Input;
        }
        float t0;



        if (Part1)
        {
            t0 = Time.realtimeSinceStartup;
            Debug.Log(Time.realtimeSinceStartup);

            day_1();

            Debug.Log(Time.realtimeSinceStartup);
            Debug.Log("Day 1 duration is : " + (Time.realtimeSinceStartup - t0).ToString());
        }

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        if (Part2)
        {
            t0 = Time.realtimeSinceStartup;
            Debug.Log(Time.realtimeSinceStartup);

            day_2();

            Debug.Log(Time.realtimeSinceStartup);
            Debug.Log("Day 2 duration is : " + (Time.realtimeSinceStartup - t0).ToString());
        }

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(5f);
        UnityEditor.EditorApplication.isPlaying = false;
    }


    void day_1()
    {
        string result = "";

        int length = _input.Length;
     

        List<int> cups = new List<int>();
        foreach (char c in _input) { cups.Add(int.Parse(c.ToString())); }
        int currentCup = cups[0];
        int movesCount = 10;
        for (int cnt = 1; cnt <= movesCount; cnt++)
        {
            string log = " -- move " + cnt + " --\n";
            if (IsDebug)
            {
                log += "cups: ";
                foreach (int c in cups) { log += (c == currentCup ? "(" + c + ")" : c.ToString()) + " "; }
            }
            int index = cups.FindIndex(x => x == currentCup); //_cups.IndexOf(currentCup);
            int pickA = cups[(index + 1) % length], pickB = cups[(index + 2) % length], pickC = cups[(index + 3) % length];
            cups.Remove(pickA);
            cups.Remove(pickB);
            cups.Remove(pickC);
            if (IsDebug)
            {
                log += System.Environment.NewLine + "pick up: " + pickA + ", " + pickB + ", " + pickC + System.Environment.NewLine;
            }

            int destinationIndex = -1, destinationCup = currentCup;
            while (destinationIndex < 0)
            {
                destinationCup = ((destinationCup - 1 - length) % length) + length;
                log += "destinationCup :" + destinationCup+",";
                destinationIndex = cups.FindIndex(x => x == destinationCup);                
            }
            log += System.Environment.NewLine + "destination: " + destinationCup;

            index = cups.FindIndex(x => x == currentCup);
            currentCup = cups[(index + 1) % cups.Count];

            cups.Insert(destinationIndex + 1, pickC);
            cups.Insert(destinationIndex + 1, pickB);
            cups.Insert(destinationIndex + 1, pickA);

            if (IsDebug)
                Debug.Log(log);
        }

        string finalLog = " -- final --\n";
        if (IsDebug)
        {
            finalLog += "cups: ";
            foreach (int c in cups) { finalLog += (c == currentCup ? "(" + c + ")" : c.ToString()) + " "; }
            Debug.Log(finalLog);
        }

        int index1 = cups.FindIndex(x => x == 1);
        int k = (index1 + 1) % cups.Count;
        result = "";
        while (k != index1)
        {
            result += cups[k];
            k = (k + 1) % cups.Count;
        }

        Debug.Log("Part 1 result is : " + result + "\n");
    }

    // Doesn't work on Unity, too much calls
    void day_2()
    {
        long result = 0;
        int length = _input.Length;

        long start = System.DateTime.Now.Ticks;
        long relativeStart = System.DateTime.Now.Ticks;

        int minValue = 1, maxValue = 1000000;

        List<int> cups = new List<int>();
        foreach (char c in _input) { cups.Add(int.Parse(c.ToString())); }
        for (int i = 10; i <= 1000000; i++) { cups.Add(i); }


        int movesCount = 10000000;
        for (int cnt = 1; cnt <= movesCount; cnt++)
        {
            int currentCup = cups[0];
            int pickA = cups[1], pickB = cups[2], pickC = cups[3];

            cups.RemoveRange(0, 4);

            int destinationCup = currentCup - 1;
            if (destinationCup < minValue)
                destinationCup = maxValue;
            while (pickA == destinationCup || pickB == destinationCup || pickC == destinationCup)
            {
                destinationCup--;
                if (destinationCup < minValue)
                    destinationCup = maxValue;
            }
            int destinationIndex = cups.FindIndex(x => x == destinationCup);

            cups.Insert(destinationIndex + 1, pickC);
            cups.Insert(destinationIndex + 1, pickB);
            cups.Insert(destinationIndex + 1, pickA);

            cups.Add(currentCup);

            if (cnt % 10000 == 0)
            {
                long end = System.DateTime.Now.Ticks;
                Debug.Log(cnt.ToString() + " -> " + ((end - start) / 10000).ToString() + "(" + ((end - relativeStart) / 10000).ToString() + ")");
                relativeStart = end;
            }
            if (cnt == 10000 || cnt == 100000 || cnt == 600000)
            {
                int _index1 = cups.FindIndex(x => x == 1);
                int _t1 = cups[(_index1 + 1) % cups.Count];
                int _t2 = cups[(_index1 + 2) % cups.Count];
                Debug.Log(_t1 + " * " + _t2);
                long _result = _t1 * _t2;
                Debug.Log("At turn " + cnt + " result is : " + _result);
            }
        }

        int index1 = cups.FindIndex(x => x == 1);
        long t1 = cups[(index1 + 1) % cups.Count];
        long t2 = cups[(index1 + 2) % cups.Count];
        Debug.Log(t1 + " * " + t2);

        result = t1 * t2;
        Debug.Log("Part2 result is " + result);
    }


}
