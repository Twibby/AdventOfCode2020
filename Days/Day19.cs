using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day19 : MonoBehaviour
{
    public bool IsDebug = false;
    public bool IsTestInput = false;
    public bool Part1 = true;

    private string _testInput = "";
    private string _input;

    void Start()
    {
        _testInput = "0: 4 1\n1: 2 3 | 3 2\n2: 4 4 | 5 5\n3: 4 5 | 5 4\n4: \"a\"\n5: \"b\"\n\nababb\nbabab\nabbba\naaabb\naaaab";

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
            _input = _testInput;
        }


        float t0 = Time.realtimeSinceStartup;
        Debug.Log(Time.realtimeSinceStartup);

        Part1 = true;
        day_1();

        Debug.Log(Time.realtimeSinceStartup);
        Debug.Log("Day 1 duration is : " + (Time.realtimeSinceStartup - t0).ToString());

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        t0 = Time.realtimeSinceStartup;
        Debug.Log(Time.realtimeSinceStartup);

        Part1 = false;
        day_1();

        Debug.Log(Time.realtimeSinceStartup);
        Debug.Log("Day 2 duration is : " + (Time.realtimeSinceStartup - t0).ToString());

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(5f);
        UnityEditor.EditorApplication.isPlaying = false;
    }

    class Rule
    {
        public List<List<int>> rules;
        public char c;

        public HashSet<string> validMessages = new HashSet<string>();

        public override string ToString()
        {
            if (rules == null || rules.Count == 0)
                return "'" + c.ToString() + "'";

            string log = "";
            foreach (List<int> subrule in rules)
            {
                subrule.ForEach(x => log += x + " ");
                log += " | ";
            }
            return log.TrimEnd(new char[] { ' ', '|' });
        }
    }

    Dictionary<int, Rule> _rules = new Dictionary<int, Rule>();

    void day_1()
    {
        long result = 0;

        string[] inputRules = _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[0].Split('\n');
        string[] messages = _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[1].Split('\n');

        _rules = new Dictionary<int, Rule>();

        foreach (string inputRule in inputRules)
        {
            int indexRule = int.Parse(inputRule.Substring(0, inputRule.IndexOf(':')));

            Rule rule = new Rule();
            if (inputRule.IndexOf('"') > 0)
            {
                rule.c = inputRule[inputRule.IndexOf('"') + 1];
            }
            else
            {
                rule.rules = new List<List<int>>();
                foreach (string subrule in inputRule.Substring(inputRule.IndexOf(':') + 1).Split('|'))
                {
                    rule.rules.Add(new List<int>(subrule.Trim().Split(new string[] { " " }, System.StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)));
                }
            }

            _rules.Add(indexRule, rule);
        }

        computeValidMessages(8);
        computeValidMessages(11);
        //computeValidMessages(0,0,true);   AS 0 = 8 11  and it's going too big (2^21), we only compute 8 and 11 


        int index = messages.Length;
        foreach (string message in messages)
        {
            if (isMessageValid(message))
            {
                result++;

                if (IsDebug)
                    Debug.Log("Word " + message + " matching !");
            }
            else
            {
                if (IsDebug)
                    Debug.Log("Word " + message + " not matching");
            }
            index--;
            if (IsDebug)
                Debug.Log("\t -> remaining words : " + index);
        }

        Debug.LogWarning(" Part " + (Part1 ? "1" : "2") + " result is : " + result.ToString());
    }

    bool isMessageValid(string message)
    {
        if (Part1)
        {
            bool valid = false;
            for (int i = 1; i < message.Length; i++)
            {
                valid = _rules[8].validMessages.Contains(message.Substring(0, i)) && _rules[11].validMessages.Contains(message.Substring(i));
                if (valid)
                    break;
            }
            return valid;
        }
        else
        {
            bool valid = false;
            for (int i = 1; i < message.Length; i++)
            {
                if (_rules[42].validMessages.Contains(message.Substring(0, i)))
                {
                    // either this is the only 8 (8 =42) message, or it's recursive (8 = 42 + 8)

                    // Only 8 case  (42)                    
                    valid = isValid11Message(message.Substring(i));    // Normal case 8 + 11

                    // Recursive 8 (42-8)
                    valid |= isMessageValid(message.Substring(i));  // recursive case for 8 : 8 + 8 + ... + 8 + 11

                    if (valid)
                        break;
                }
            }
            return valid;
        }
    }

    bool isValid11Message(string message)
    {
        if (_rules[11].validMessages.Contains(message))
            return true;

        bool valid = false;
        for (int i = 1; i < message.Length; i++)
        {
            if (_rules[42].validMessages.Contains(message.Substring(0, i)))
            {
                if (_rules[31].validMessages.Contains(message.Substring(i)))
                {
                    if (IsDebug)
                        Debug.Log("WEIRD");
                    return true;
                }

                string suffix = message.Substring(i);
                for (int j = 1; j < suffix.Length; j++)
                {
                    if (_rules[31].validMessages.Contains(suffix.Substring(j)))    // message has form 42 x 31 test if x match with 11
                    {
                        valid = isValid11Message(suffix.Substring(0, j));
                    }

                    if (valid)
                        break;
                }
            }

            if (valid)
                break;
        }

        return valid;
    }

    void computeValidMessages(int ruleIndex, int offset = 0, bool debugging = false)
    {
        if (IsDebug)
            Debug.Log(writeOffset(offset) + "computing validLengths for rule " + ruleIndex);

        if (!_rules.ContainsKey(ruleIndex))
        {
            if (IsDebug)
                Debug.Log("No rule index " + ruleIndex);

            return;
        }

        if (_rules[ruleIndex].rules == null || _rules[ruleIndex].rules.Count == 0)
        {
            if (IsDebug)
                Debug.Log(writeOffset(offset) + "\t -> Rule " + ruleIndex + " is terminal and final message is : " + _rules[ruleIndex].c.ToString());

            _rules[ruleIndex].validMessages = new HashSet<string>() { _rules[ruleIndex].c.ToString() };
            return;
        }

        int index = 0;
        HashSet<string> valid = new HashSet<string>();
        foreach (List<int> subrule in _rules[ruleIndex].rules)
        {
            index++;
            if (IsDebug)
                Debug.Log(writeOffset(offset) + "Rule " + ruleIndex + ", subrule " + index + " has " + subrule.Count + "members");

            if (subrule.Count == 0)
                continue;

            if (IsDebug)
                Debug.Log(writeOffset(offset) + "Testing subRule index : " + subrule[0]);

            if (_rules[subrule[0]].validMessages == null || _rules[subrule[0]].validMessages.Count == 0)
            {
                if (IsDebug)
                    Debug.Log(" -> must compute ! ");
                computeValidMessages(subrule[0], offset + 1);
            }
            else
            {
                if (IsDebug)
                    Debug.Log(" -> NO NEED ! ");
            }
            HashSet<string> prefixes = _rules[subrule[0]].validMessages;

            if (IsDebug)
                Debug.Log(writeOffset(offset) + "\t -> " + subrule[0] + " Has now computed validmessages, count is : " + _rules[subrule[0]].validMessages.Count);

            HashSet<string> tmp = new HashSet<string>();
            for (int i = 1; i < subrule.Count; i++) // (int subRuleIndex in subrule)
            {
                if (IsDebug)
                    Debug.Log(writeOffset(offset) + "Testing subRule index : " + subrule[i]);

                if (_rules[subrule[i]].validMessages == null || _rules[subrule[i]].validMessages.Count == 0)
                {
                    if (IsDebug)
                        Debug.Log(" -> must compute ! ");
                    computeValidMessages(subrule[i], offset + 1);
                }
                else
                {
                    if (IsDebug)
                        Debug.Log(" -> NO NEED ! ");
                }

                if (IsDebug)
                    Debug.Log(writeOffset(offset) + "\t -> " + subrule[i] + " Has computed validmessages, count is : " + _rules[subrule[i]].validMessages.Count);

                HashSet<string> validSuffix = _rules[subrule[i]].validMessages;

                int debugIndex = validSuffix.Count;
                foreach (string suffix in validSuffix)
                {
                    if (IsDebug && debugging)
                        Debug.Log(writeOffset(offset) + "remaining join " + debugIndex--);

                    tmp = new HashSet<string>(tmp.Union(prefixes.Select(x => x + suffix)));
                }

                prefixes = new HashSet<string>(tmp);
                tmp = new HashSet<string>();
            }

            valid = new HashSet<string>(valid.Union(prefixes));
        }

        if (IsDebug)
            Debug.Log(writeOffset(offset) + "(end) -> " + ruleIndex + " Has computed validmessages, count is : " + valid.Count);

        _rules[ruleIndex].validMessages = new HashSet<string>(valid);
    }

    string writeOffset(int offset)
    {
        string log = "";
        for (int off = 0; off < offset; off++) { log += "|."; }
        return log;
    }
}
