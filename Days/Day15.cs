using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Day15 : MonoBehaviour
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

    Dictionary<long, long> _memory = new Dictionary<long, long>();

    void day_1()
    {
        long result = 0;

        result = auxDay15(2020);

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }

    void day_2()
    {
        long result = 0;

        result = auxDay15(30000000);

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }

    long auxDay15(int turnCount)
    {
        List<int> inputs = new List<int>() { 18, 11, 9, 0, 5, 1 };
        long lastValue = 0;

        for (int i = 1; i < turnCount; i++)
        {
            //Debug.LogWarning("TURN " + i);

            if (i < inputs.Count)
            {
                _memory.Add(inputs[i-1], i);
                lastValue = inputs[i];
                //Debug.Log("Adding in memory from input " + inputs[i - 1] + " -> " + i + " | last Value is " + lastValue);
                continue;
            }
            long newLastValue = -1;
            if (!_memory.ContainsKey(lastValue))
            {
                //Debug.Log("Last value '" + lastValue + "' not in memory");
                newLastValue = 0;
            }
            else
            {
                //Debug.Log("Last value '" + lastValue + "' in memory with last index " + _memory[lastValue]);
                newLastValue = i - _memory[lastValue];
                //Debug.Log("NEW Last value '" + newLastValue);
            }

            if (!_memory.ContainsKey(lastValue))
                _memory.Add(lastValue, 0);

            _memory[lastValue] = i;

            lastValue = newLastValue;
            if (IsDebug)
            {
                writeMemory();
                Debug.Log("Last Value is : " + lastValue);
            }

        }

        return lastValue;

    }

   void writeMemory()
    {
        string log = "";
        foreach (var x in _memory) { log += x.Key + " -> " + x.Value + "; "; }
        Debug.Log(log);
    }
}
