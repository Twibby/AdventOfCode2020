using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Day16 : MonoBehaviour
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

    Dictionary<string, List<Vector2>> _fields = new Dictionary<string, List<Vector2>>();
    Dictionary<string, int> _matchings = new Dictionary<string, int>();

    void day_1()
    {
        long result = 0;

        string[] inputs = Tools.Instance.Input.Split(new string[1] { "\n\n" },StringSplitOptions.RemoveEmptyEntries);
        string[] classes = inputs[0].Split('\n');
        string[] passports = inputs[2].Split('\n');

        List<Vector2> validBounds = new List<Vector2>();

        // Init rules
        foreach (string field in classes)
        {
            if (field.IndexOf(':') < 0)
            {
                Debug.LogError("field malformed " + field);
                continue;
            }

            //string fieldName = field.Substring(0, field.IndexOf(':'));
            //_fields.Add(fieldName, new List<Vector2>());
            foreach (string bounds in field.Substring(field.IndexOf(':') +1).Split(new string[1] { " or " }, StringSplitOptions.RemoveEmptyEntries))
            {
                //_fields[field].Add(new Vector2(int.Parse(bounds.Substring(0, bounds.IndexOf('-'))), int.Parse(bounds.Substring(bounds.IndexOf('-') + 1))));
                validBounds.Add(new Vector2(int.Parse(bounds.Substring(0, bounds.IndexOf('-'))), int.Parse(bounds.Substring(bounds.IndexOf('-') + 1))));
            }
        }

        foreach (string passport in passports)
        {
            if (passport.StartsWith("nearby tickets") || passport == "")
                continue;

            foreach (int val in passport.Split(',').Select(int.Parse))
            {
                bool valid = false;
                foreach (Vector2 bounds in validBounds)
                {
                    valid = val >= bounds.x && val <= bounds.y;
                    if (valid)
                        break;
                }

                if (!valid)
                    result += val;
            }
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }

    void day_2()
    {
        long result = 0;

        string[] inputs = Tools.Instance.Input.Split(new string[1] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
        string[] classes = inputs[0].Split('\n');
        string[] passports = inputs[2].Split('\n');
        int[] myPassport = inputs[1].Split('\n')[1].Split(',').Select(int.Parse).ToArray();

        List<Vector2> validBounds = new List<Vector2>();

        // Init rules
        foreach (string field in classes)
        {
            if (field.IndexOf(':') < 0)
            {
                Debug.LogError("field malformed " + field);
                continue;
            }

            string fieldName = field.Substring(0, field.IndexOf(':'));
            _fields.Add(fieldName, new List<Vector2>());
            foreach (string bounds in field.Substring(field.IndexOf(':') + 1).Split(new string[1] { " or " }, StringSplitOptions.RemoveEmptyEntries))
            {
                _fields[fieldName].Add(new Vector2(int.Parse(bounds.Substring(0, bounds.IndexOf('-'))), int.Parse(bounds.Substring(bounds.IndexOf('-') + 1))));
                validBounds.Add(new Vector2(int.Parse(bounds.Substring(0, bounds.IndexOf('-'))), int.Parse(bounds.Substring(bounds.IndexOf('-') + 1))));
            }
        }

        List<List<int>> fieldValues = new List<List<int>>();
        for (int i = 0; i < _fields.Count; i++)
        {
            fieldValues.Add(new List<int>());
        }

        // First, excludes invalid passports, second add each field to a list depending of his position so we can have list of all values for first field grouped in a one list, and same for each field
        foreach (string passport in passports)
        {
            if (passport.StartsWith("nearby tickets") || passport == "")
                continue;

            bool valid = false;
            foreach (int val in passport.Split(',').Select(int.Parse))
            {
                foreach (Vector2 bounds in validBounds)
                {
                    valid = val >= bounds.x && val <= bounds.y;
                    if (valid)
                        break;
                }

                if (!valid)
                    break; ;
            }

            if (valid)
            {
                int index = 0;
                foreach (int val in passport.Split(',').Select(int.Parse))
                {
                    fieldValues[index].Add(val);
                    index++;
                }
            }
        }

        if (IsDebug)
        {
            Debug.LogWarning("Writing memory");
            foreach (var field in _fields)
            {
                string log = field.Key + " -> ";
                field.Value.ForEach(x => log += x.ToString() + ", ");
                Debug.Log(log);
            }
        }

        int safetyCount = 20;
        while (safetyCount > 0 && _matchings.Count < _fields.Count)
        {

            for (int i = 0; i < fieldValues.Count; i++)
            {
                if (_matchings.ContainsValue(i))    // we already know what field correspond to position 
                    continue;

                List<string> matchingFields = getValidFields(fieldValues[i]);
                if (matchingFields.Count == 0)      // No matching field, weird.
                {
                    string log = "";
                    fieldValues[i].ForEach(x => log += x + ",");
                    Debug.LogError("heeeem fu : " + log);
                }
                else if (matchingFields.Count == 1)// Only 1 matching field for all values, perfect, make the link between position i and 
                {
                    _matchings[matchingFields[0]] = i;
                }
                else                                // Multiple fields OK for all values in position i, so we can't link it to a field yet; try again in next loop (as there should be less fields to test)
                {
                    string log = "";
                    matchingFields.ForEach(x => log += x + ", ");

                    if (IsDebug)
                        Debug.LogWarning("Can't say yet for field number " + i + " : " + log);
                }
            }

            if (IsDebug)
            {
                string matchingLog = "AFTER LOOP " + (21 - safetyCount).ToString() + " : ";
                foreach (var match in _matchings)
                {
                    matchingLog += match.Key + " -> " + match.Value + ", ";
                }
                Debug.LogWarning(matchingLog);
            }
            safetyCount--;
        }



        result = 1;
        foreach (var match in _matchings)
        {
            if (match.Key.StartsWith("departure"))
            {
                Debug.Log("Field : " + match.Key + " -> " + myPassport[match.Value]);
                result *= myPassport[match.Value];
            }
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }

    List<string> getValidFields(List<int> values)
    {
        List<string> result = new List<string>();
        string log = "";

        bool valid = true;
        foreach (var field in _fields)
        {
            log += "trying field : " + field.Key;
            if (_matchings.ContainsKey(field.Key))
            {
                log += " is in _mathcings so CONTINUE";
                continue;
            }

            log += " | Field not in _matchings yet, bounds are : ";
            field.Value.ForEach(x => log += x.ToString() + ", ");
            log += "  -->  ";

            valid = true;
            foreach (int val in values)
            {
                valid = false;
                foreach (Vector2 bounds in field.Value)
                {
                    valid |= val >= bounds.x && val<=bounds.y;
                }
                if (!valid)
                {
                    log += val + " is NOT in range so " ;
                    break;
                }
            }
            log += "VALID FIELD : " + valid.ToString().ToUpper() + System.Environment.NewLine;

            if (valid)
                result.Add(field.Key);
        }

        if (IsDebug)
            Debug.Log(log);

        return result;
    }


   //void writeMemory()
   // {
   //     string log = "";
   //     foreach (var x in _memory) { log += x.Key + " -> " + x.Value + "; "; }
   //     Debug.Log(log);
   // }
}
