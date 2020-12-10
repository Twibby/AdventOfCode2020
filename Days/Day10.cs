using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class Day10 : MonoBehaviour
{
    public const int Day = 10;
    private bool _debug = false;

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

        List<int> adaptaters = new List<int>(Tools.Instance.Input.Split('\n').Select(int.Parse));

        adaptaters.Sort();

        int countDiff1 = 0, countDiff2 = 0, countDiff3 = 0;
        bool validAdaptater = true;

        if (adaptaters[0] == 1)
            countDiff1++;
        else if (adaptaters[0] == 2)
            countDiff2++;
        else if (adaptaters[0] == 3)
            countDiff3++;
        else
            validAdaptater = false;
            

        for (int i = 1; i < adaptaters.Count && validAdaptater; i++)
        {
            int diff = adaptaters[i] - adaptaters[i - 1];

            if (diff == 1)
                countDiff1++;
            else if (diff == 2)
                countDiff2++;
            else if (diff == 3)
                countDiff3++;
            else
                validAdaptater = false;
        }

        countDiff3++;
        result = countDiff1 * countDiff3;

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }


    void day_2()
    {
        long result = 0;

        List<int> adaptaters = new List<int>(Tools.Instance.Input.Split('\n').Select(int.Parse));
        List<int> testInput = new List<int>() { 16, 10, 15, 5, 1, 11, 7, 19, 6, 12, 4 };
        List<int> testInput2 = new List<int>() { 28, 33, 18, 42, 31, 14, 46, 20, 48, 47, 24, 23, 49, 45, 19, 38, 39, 11, 1, 32, 25, 35, 8, 17, 7, 9, 4, 2, 34, 10, 3 };
        testInput.Add(0);
        testInput.Sort();
        testInput2.Add(0);
        testInput2.Sort();
        adaptaters.Add(0);
        adaptaters.Sort();

        //_memory = new Dictionary<KeyValuePair<int, int>, long>();
        //result = validCombinationsCount(testInput);
        //Debug.Log("[Day10] Part 'testInput' result is : " + result.ToString());
        //if (result == 8)
        //{
        //    _memory = new Dictionary<KeyValuePair<int, int>, long>();
        //    result = validCombinationsCount(testInput2);
        //}

        _memory = new Dictionary<KeyValuePair<int, int>, long>();
        result = validCombinationsCount(adaptaters);

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }

    private static Dictionary<KeyValuePair<int, int>, long> _memory;

    // input has to be sorted
    private long validCombinationsCount(List<int> adaptaters)
    {
        if (_debug)
        {
            string log = "Current list (" + adaptaters.Count + ") is : ";
            adaptaters.ForEach(x => log += x + ",");
            Debug.Log(log);
        }

        if (adaptaters.Count == 0)
        {
            //Debug.Log("Empty List -> 0");
            return 0;
        }
        else if (adaptaters.Count == 1)
        {
            //Debug.Log("Last list Element -> 1");
            return 1;
        }
        else
        {
            KeyValuePair<int, int> keys = new KeyValuePair<int, int>(adaptaters[0], adaptaters[1]);
            if (_memory.ContainsKey(keys))
            {
                return _memory[keys];
            }

            int diff = adaptaters[1] - adaptaters[0];
            if (diff == 3)
            {
                long result3 = validCombinationsCount(adaptaters.GetRange(1, adaptaters.Count - 1));
                _memory.Add(new KeyValuePair<int, int>(adaptaters[0], adaptaters[1]), result3);
                writeMemory();
                return result3;

            }
            else if (diff == 2 || diff == 1)
            {
                if (adaptaters.Count > 2 && (adaptaters[2] - adaptaters[0]) <= 3)
                {
                    List<int> subset = new List<int>(adaptaters);
                    subset.RemoveAt(1);

                    long result12 = validCombinationsCount(subset);
                    //_memory.Add(new KeyValuePair<int, int>(subset[0], subset[1]), result12);
                    long result3 = validCombinationsCount(adaptaters.GetRange(1, adaptaters.Count - 1));
                    //_memory.Add(new KeyValuePair<int, int>(adaptaters[1], adaptaters[2]), result3);

                    _memory.Add(new KeyValuePair<int, int>(adaptaters[0], adaptaters[1]), result12 + result3);
                    writeMemory();

                    return result12 + result3;
                }
                else
                {
                    long result12Big = validCombinationsCount(adaptaters.GetRange(1, adaptaters.Count - 1));
                    _memory.Add(new KeyValuePair<int, int>(adaptaters[0], adaptaters[1]), result12Big);
                    writeMemory();

                    return result12Big;
                }
            }
            else
            {
                //Debug.Log("diff is too high : " + diff + " -> returning 0");
                return 0;
            }
        }
    }

    void writeMemory()
    {
        if (!_debug)
            return;

        Debug.Log("MEMORY STATE : ");
        foreach (var tmp in _memory)
        {
            Debug.Log("subset from ('" + tmp.Key.Key + "','" + tmp.Key.Value + "') count is : " + tmp.Value);
        }
    }
}
