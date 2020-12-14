using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Day14 : MonoBehaviour
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
    string mask = "";

    void day_1()
    {
        long result = 0;

        int stringLength = 36;
        _memory = new Dictionary<long, long>();

        foreach (string instruction in Tools.Instance.Input.Split('\n'))
        {
            if (instruction.StartsWith("mask ="))
            {
                mask = instruction.Substring(7);
                if (mask.Length != stringLength)
                {
                    Debug.LogError("Mask malformed : " + instruction + "  || " + instruction.Length);
                    return;
                }
            }
            else if (instruction.StartsWith("mem"))
            {
                long index = long.Parse(instruction.Substring(4, instruction.IndexOf(']') - 4));

                string binary = Convert.ToString(long.Parse(instruction.Substring(instruction.IndexOf('=') + 2)), 2);
                if (binary.Length < stringLength)
                    binary = new string('0', stringLength - binary.Length) + binary;

                string val = "";
                for (int i = 0; i < stringLength; i++)
                {
                    val += (mask[i] == 'X' ? binary[i] : mask[i]);
                }

                if (!_memory.ContainsKey(index))
                    _memory.Add(index, 0);

                _memory[index] = Convert.ToInt64(val, 2);
            }
        }


        foreach (var mem in _memory)
        {
            result += mem.Value;
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }


    void day_2()
    {
        long result = 0;

         int stringLength = 36;
        _memory = new Dictionary<long, long>();

        foreach (string instruction in Tools.Instance.Input.Split('\n'))
        {
            if (instruction.StartsWith("mask ="))
            {
                mask = instruction.Substring(7);
                if (mask.Length != stringLength)
                {
                    Debug.LogError("Mask malformed : " + instruction + "  || " + instruction.Length);
                    return;
                }
            }
            else if (instruction.StartsWith("mem"))
            {
                int index = int.Parse(instruction.Substring(4, instruction.IndexOf(']') - 4));
                long value = long.Parse(instruction.Substring(instruction.IndexOf('=') + 2));

                string binary = Convert.ToString(index, 2);
                if (binary.Length < stringLength)
                    binary = new string('0', stringLength - binary.Length) + binary;

                string val = "";
                for (int i = 0; i < stringLength; i++)
                {
                    val += (mask[i] == '0' ? binary[i] : mask[i]);
                }

                foreach (string address in recAux(val))
                {
                    long addr = Convert.ToInt64(address, 2);
                    if (!_memory.ContainsKey(addr))
                        _memory.Add(addr, 0);

                    _memory[addr] = value;
                }               
            }
        }

        foreach (var mem in _memory)
        {
            result += mem.Value;
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }

    // returns all combinations of input where all char X are replaced by 0 or 1
    List<string> recAux(string input)
    {
        if (input.Length == 0)
            return new List<string>() { "" };
        else
        {
            if (input[0] == '1' || input[0] == '0')
            {
                return recAux(input.Substring(1)).Select(s => input[0] + s).ToList();
            }
            else if (input[0] == 'X')
            {

                return recAux(input.Substring(1)).Select(s => '0' + s).ToList().Union(recAux(input.Substring(1)).Select(s => '1' + s).ToList()).ToList();
            }
            else
            {
                Debug.LogError("input string malformed : " + input);
                return new List<string>();
            }
        }
    }
}
