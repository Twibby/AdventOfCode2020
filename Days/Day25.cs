using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day25 : MonoBehaviour
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
        long result = 0;

        long cardPK = long.Parse(_input.Split('\n')[0]);
        long doorPK = long.Parse(_input.Split('\n')[1]);
        if (IsDebug)
            Debug.Log("Card public key is " + cardPK + " & door public key is " + doorPK);

        long tmp = 1;
        int turnCount = 0;
        while (turnCount < 100000000)   //safety count
        {
            tmp = (tmp * 7) % 20201227;
            turnCount++;

            if (tmp == cardPK)
                break;
        }
        if (turnCount >= 100000000)
        {
            Debug.Log("Not enough turns");
            return;
        }
        else if (IsDebug)
        {
            Debug.Log("Card loop size is " + turnCount);
        }

        result = 1;
        for (int i = 0; i < turnCount; i++)
        {
            result = (result * doorPK) % 20201227;
        }
        Debug.Log("Part 1 result is : " + result + "\n");
    }

    Dictionary<Vector2, bool> _tilesFlipped = new Dictionary<Vector2, bool>();
    void day_2()
    {
        Debug.Log("Part 2 is free star");
    }
}
