using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day6 : MonoBehaviour
{
    public const int Day = 6;

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

        foreach (string form in Tools.Instance.Input.Split(new string[1] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)) 
        {
            List<char> yesAnswers = new List<char>();
            foreach (char c in form)
            {
                if (c == '\n')
                    continue;

                if (!yesAnswers.Contains(c))
                    yesAnswers.Add(c);
            }

            result += yesAnswers.Count;
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }

    void day_2()
    {
        int result = 0;

        foreach (string groupForm in Tools.Instance.Input.Split(new string[1] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries))
        {
            if (groupForm == "")
                continue;

            List<char> yesAnswers = new List<char>(groupForm.Split(new char[1] { '\n' })[0]);

            foreach (string personForm in groupForm.Split(new char[1] { '\n' }))
            { 
                if (personForm == "")
                    continue;

                yesAnswers = yesAnswers.Intersect<char>(new List<char>(personForm)).ToList<char>();
                if (yesAnswers.Count == 0)
                    break;

                //foreach (char c in yesAnswers)
                //{
                //    if (personForm.IndexOf(c) < 0)
                //        yesAnswers.Remove(c);
                //}
            }

            result += yesAnswers.Count;
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }
}
