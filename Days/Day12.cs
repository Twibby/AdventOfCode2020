using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day12 : MonoBehaviour
{
    public const int Day = 12;
    public bool IsDebug = false;

    void Start()
    {
        StartCoroutine(coDay());
    }

    IEnumerator coDay()
    {
        Debug.Log("Day is : " + this.GetType().ToString().Substring(3));
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
        float result = 0;

        Vector2 boatPos = Vector2.zero;
        float angleWithEast = 0;        // in deg

        foreach (string instruction in Tools.Instance.Input.Split('\n'))
        {
            int value = -1;
            if (!int.TryParse(instruction.Substring(1), out value))
            {
                Debug.LogError("Input is malformed : " + instruction + " | '" + instruction.Substring(1) + "'");
                continue;
            }

            switch (instruction[0])
            {
                case 'N': boatPos.y += value; break;
                case 'S': boatPos.y -= value; break;
                case 'W': boatPos.x -= value; break;
                case 'E': boatPos.x += value; break;
                case 'R': angleWithEast -= value; break;
                case 'L': angleWithEast += value; break;
                case 'F':
                    boatPos.x += Mathf.Cos(Mathf.Deg2Rad * angleWithEast) * value;
                    boatPos.y += Mathf.Sin(Mathf.Deg2Rad * angleWithEast) * value;
                    break;
            }
        }

        Debug.Log("[Day 12] Boat position is : " + boatPos.ToString() + " | " + Mathf.Round(Mathf.Abs(boatPos.x)) + " | " + Mathf.Round(Mathf.Abs(boatPos.y)));

        result = Mathf.Round(Mathf.Abs(boatPos.x)) + Mathf.Round(Mathf.Abs(boatPos.y));

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }


    void day_2()
    {
        float result = 0;

        Vector2 boatPos = Vector2.zero;
        Vector2 relativeWaypoint = new Vector2(10f, 1f);

        foreach (string instruction in Tools.Instance.Input.Split('\n'))
        {
            int value = -1;
            if (!int.TryParse(instruction.Substring(1), out value))
            {
                Debug.LogError("Input is malformed : " + instruction + " | '" + instruction.Substring(1) + "'");
                continue;
            }

            switch (instruction[0])
            {
                case 'N': relativeWaypoint.y += value; break;
                case 'S': relativeWaypoint.y -= value; break;
                case 'W': relativeWaypoint.x -= value; break;
                case 'E': relativeWaypoint.x += value; break;
                case 'R':
                    Vector2 tmp = Vector2.zero;
                    tmp.x = relativeWaypoint.x * Mathf.Cos(Mathf.Deg2Rad * ((360 - value) % 360)) - relativeWaypoint.y * Mathf.Sin(Mathf.Deg2Rad * ((360 - value) % 360));
                    tmp.y = relativeWaypoint.x * Mathf.Sin(Mathf.Deg2Rad * ((360 - value) % 360)) + relativeWaypoint.y * Mathf.Cos(Mathf.Deg2Rad * ((360 - value) % 360));
                    relativeWaypoint = tmp;
                    break;
                case 'L':
                    Vector2 temp = Vector2.zero;
                    temp.x = relativeWaypoint.x * Mathf.Cos(Mathf.Deg2Rad * value) - relativeWaypoint.y * Mathf.Sin(Mathf.Deg2Rad * value);
                    temp.y = relativeWaypoint.x * Mathf.Sin(Mathf.Deg2Rad * value) + relativeWaypoint.y * Mathf.Cos(Mathf.Deg2Rad * value);
                    relativeWaypoint = temp;
                    break;
                case 'F':
                    boatPos.x += relativeWaypoint.x * value;
                    boatPos.y += relativeWaypoint.y * value;
                    break;
            }
        }

        Debug.Log("[Day 12] Boat position is : " + boatPos.ToString() + " | " + Mathf.Round(Mathf.Abs(boatPos.x)) + " | " + Mathf.Round(Mathf.Abs(boatPos.y)));

        result = Mathf.Round(Mathf.Abs(boatPos.x)) + Mathf.Round(Mathf.Abs(boatPos.y));

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }
}
