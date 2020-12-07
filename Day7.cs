using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day7 : MonoBehaviour
{
    public const int Day = 7;

    private Dictionary<string, List<string>> _rules = new Dictionary<string, List<string>>();
    private Dictionary<string, List<KeyValuePair<string, int>>> _rules2 = new Dictionary<string, List<KeyValuePair<string, int>>>();
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
        yield return new WaitForSeconds(20f);
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void day_1()
    {
        int result = 0;

        List<string> colorsOK = new List<string>();
        _rules = new Dictionary<string, List<string>>();

        //initialization
        foreach (string rule in Tools.Instance.Input.Split('\n'))
        {
            if (rule == "")
                continue;

            string searchPattern = " bags contain ";
            int index = rule.IndexOf(searchPattern);
            if (index < 0)
            {
                Debug.LogWarning("[Day7] Rule is malformed : " + rule);
                continue;
            }

            string container = rule.Substring(0, index);

            if (rule.Substring(index + searchPattern.Length).Trim().StartsWith("no other bag"))
            {
                continue;
            }

            List<string> containings = new List<string>();
            foreach (string content in rule.Substring(index + searchPattern.Length).Split(','))
            {
                string containing = content.Trim();
                int startIndex = containing.IndexOf(' ');
                int endIndex = containing.IndexOf(" bag");

                containing = containing.Substring(startIndex + 1, endIndex - startIndex).Trim();
                if (!_rules.ContainsKey(containing))
                {
                    _rules.Add(containing, new List<string>());
                }

                _rules[containing].Add(container);
            }
        }

        string startPattern = "shiny gold";
        Debug.Log(startPattern + " count is " + (_rules.ContainsKey(startPattern) ? _rules[startPattern].Count : -1).ToString());
        colorsOK = recFindMatchingRules(startPattern);

        result = colorsOK.Count -1; // exclude startPattern
        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }

    List<string> recFindMatchingRules(string colouredBag)
    {
        if (!_rules.ContainsKey(colouredBag))
            return new List<string>() { colouredBag };
        else
        {
            List<string> result = new List<string>() { colouredBag };
            foreach (string subBag in _rules[colouredBag])
            {
                result = result.Union(recFindMatchingRules(subBag)).ToList();
            }
            return result;
        }
    }


    void day_2()
    {
        int result = 0;

        _rules2 = new Dictionary<string, List<KeyValuePair<string, int>>>();

        //initialization
        foreach (string rule in Tools.Instance.Input.Split('\n'))
        {
            if (rule == "")
                continue;

            string searchPattern = " bags contain ";
            int index = rule.IndexOf(searchPattern);
            if (index < 0)
            {
                Debug.LogWarning("[Day7] Rule is malformed : " + rule);
                continue;
            }

            string container = rule.Substring(0, index);

            if (rule.Substring(index + searchPattern.Length).Trim().StartsWith("no other bag"))
            {
                _rules2.Add(container, new List<KeyValuePair<string, int>>());
                continue;
            }

            List<KeyValuePair<string, int>> containings = new List<KeyValuePair<string, int>>();
            foreach (string content in rule.Substring(index + searchPattern.Length).Split(','))
            {
                string containing = content.Trim();
                int startIndex = containing.IndexOf(' ');
                int endIndex = containing.IndexOf(" bag");

                int number = int.Parse(containing.Substring(0, startIndex));

                containings.Add(new KeyValuePair<string, int>(containing.Substring(startIndex + 1, endIndex - startIndex).Trim(), number));
            }
            _rules2.Add(container, containings);
        }

        string startPattern = "shiny gold";
        result = recNumberOfBagsNeeded(startPattern);
        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }

    int recNumberOfBagsNeeded(string bagColour)
    {
        if (!_rules2.ContainsKey(bagColour))
        {
            Debug.LogError("bag colour '" + bagColour + "' is not in rules");
            return 0;
        }
        else if (_rules2[bagColour].Count == 0)
            return 1;
        else
        {
            int total = 1;
            foreach (KeyValuePair<string, int> subBag in _rules2[bagColour])
            {
                total += subBag.Value * recNumberOfBagsNeeded(subBag.Key);
            }
            return total;
        }
    }

    /*
    void day_1_old()
    {
        int result = 0;

        List<string> colorsOK = new List<string>();
        _rules = new Dictionary<string, List<string>>();

        //initialization
        foreach (string rule in Tools.Instance.Input.Split('\n'))
        {
            if (rule == "")
                continue;

            string searchPattern = " bags contain ";
            int index = rule.IndexOf(searchPattern);
            if (index < 0)
            {
                Debug.LogWarning("[Day7] Rule is malformed : " + rule);
                continue;
            }

            string container = rule.Substring(0, index);

            if (rule.Substring(index + searchPattern.Length).Trim().StartsWith("no other bag"))
            {
                _rules.Add(container, new List<string>());
                continue;
            }

            List<string> containings = new List<string>();
            foreach (string content in rule.Substring(index + searchPattern.Length).Split(','))
            {
                string containing = content.Trim();
                int startIndex = containing.IndexOf(' ');
                int endIndex = containing.IndexOf(" bag");
                containings.Add(containing.Substring(startIndex + 1, endIndex - startIndex).Trim());
            }
            _rules.Add(container, containings);
        }

        string startPattern = "shiny gold";
        colorsOK = recFindMatchingRulesOld(startPattern, new List<string>());

        colorsOK.Sort();
        colorsOK.ForEach(x => Debug.Log(x));

        result = colorsOK.Count;
        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }

    List<string> recFindMatchingRulesOld(string input, List<string> tmpResult)
    {
        List<string> result = new List<string>(tmpResult);
        string resultLog = "";
        result.ForEach(x => resultLog += x + ",");

        string log = "";
        List<string> containers = new List<string>();
        foreach (KeyValuePair<string, List<string>> rule in _rules)
        {
            if (rule.Value.Contains(input))
            {
                containers.Add(rule.Key);
                log += rule.Key + ", ";
            }
        }

        Debug.Log("result count is " + result.Count + " -> " + resultLog);
        Debug.Log("  => input = '" + input + "' and containers (" + containers.Count + ") are " + log);

        foreach (string container in containers)
        {
            if (result.Contains(container))
                continue;

            result.Add(container);
            result = recFindMatchingRulesOld(container, result);
        }

        return result;
    }
    */
}
