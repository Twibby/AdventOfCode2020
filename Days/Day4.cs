using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day4 : MonoBehaviour
{
    public const int Day = 4;

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
        int count = 0;

        List<string> requiredFields = new List<string>() { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };

        foreach (string passport in Tools.Instance.Input.Split(new string[1] { "\n\n" }, System.StringSplitOptions.None)) 
        {
            bool correct = true;
            foreach (string field in requiredFields)
            {
                correct &= passport.Contains(field + ":");
                if (!correct)
                    break;
            }

            if (correct)
                count++;
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + count.ToString());
    }

    void day_2()
    {
        int count = 0;

        List<string> requiredFields = new List<string>() { "byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid" };
        List<string> authorizedEyeColor = new List<string>() { "amb", "blu", "brn", "gry", "grn", "hzl", "oth"};

        foreach (string passport in Tools.Instance.Input.Split(new string[1] { "\n\n" }, System.StringSplitOptions.None))
        {
            if (passport == "")
                continue;

            bool byrOK = false, iyrOK = false, eyrOK = false, hgtOK = false, hclOK = false, eclOK = false, pidOK = false;
            foreach (string brutField in passport.Split(new char[2] { ' ', '\n' }))
            {
                string field = brutField.Trim();
                if (field.Length < 4)
                {
                    Debug.LogWarning("field not correct : " + field);
                    continue;
                }

                switch (field.Substring(0,3))
                {
                    case "byr":
                        int byr = -1;
                        byrOK = int.TryParse(field.Substring(4), out byr) && byr >= 1920 && byr <= 2002;
                        break;

                    case "iyr":
                        int iyr = -1;
                        iyrOK = int.TryParse(field.Substring(4), out iyr) && iyr >= 2010 && iyr <= 2020;
                        break;

                    case "eyr":
                        int eyr = -1;
                        eyrOK = int.TryParse(field.Substring(4), out eyr) && eyr >= 2020 && eyr <= 2030;
                        break;

                    case "hgt":
                        int hgt = -1;
                        if (field.EndsWith("cm"))
                            hgtOK = int.TryParse(field.Substring(4, field.Length - 6), out hgt) && hgt >= 150 && hgt <= 193;
                        else if (field.EndsWith("in"))
                            hgtOK = int.TryParse(field.Substring(4, field.Length - 6), out hgt) && hgt >= 59 && hgt <= 76;
                        break;

                    case "hcl":
                        hclOK = field.Length == 11;
                        for (int i=5;i < field.Length;i++)
                        {
                            hclOK &= (field[i] >= '0' && field[i] <= '9') || (field[i] >= 'a' && field[i] <= 'f');
                        }
                        break;

                    case "ecl":
                        eclOK = authorizedEyeColor.Contains(field.Substring(4));
                        break;

                    case "pid":
                        int pid = -1;
                        pidOK = field.Length == 13 && int.TryParse(field.Substring(4), out pid);
                        break;

                    default:
                        break;
                }
            }

            if (byrOK && iyrOK && eyrOK && hgtOK && hclOK && eclOK && pidOK)
                count++;
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + count.ToString());
    }
}
