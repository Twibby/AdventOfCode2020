using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day24 : MonoBehaviour
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
        int result = 0;

        Dictionary<Vector2, bool> tilesFlipped = new Dictionary<Vector2, bool>();
        foreach (string instruction in _input.Split('\n'))
        {
            bool isHalf = false;
            Vector2 pos = Vector2.zero;
            foreach (char c in instruction)
            {
                switch (c)
                {
                    case 's':
                        pos.y += -0.5f;
                        isHalf = true;
                        break;
                    case 'n':
                        pos.y += 0.5f;
                        isHalf = true;
                        break;
                    case 'e':
                        pos.x += (isHalf ? 0.5f : 1f);
                        isHalf = false;
                        break;
                    case 'w':
                        pos.x += (isHalf ? -0.5f : -1f);
                        isHalf = false;
                        break;
                    default:
                        Debug.LogError("Not supposed to happen, char '" + c + "' in instruction : " + instruction);
                        break;
                }
            }

            if (!tilesFlipped.ContainsKey(pos))
            {
                tilesFlipped.Add(pos, false);
            }
            tilesFlipped[pos] = !tilesFlipped[pos];
        }


        foreach (var tile in tilesFlipped)
        {
            Debug.Log("Tile in pos " + tile.Key.ToString() + " is Flipped : " + tile.Value);
            if (tile.Value)
                result++;
        }

        Debug.Log("Part 1 result is : " + result + "\n");
    }

    Dictionary<Vector2, bool> _tilesFlipped = new Dictionary<Vector2, bool>();
    void day_2()
    {
        long result = 0;

        //init tiles
        foreach (string instruction in _input.Split('\n'))
        {
            bool isHalf = false;
            Vector2 pos = Vector2.zero;
            foreach (char c in instruction)
            {
                switch (c)
                {
                    case 's':
                        pos.y += -0.5f;
                        isHalf = true;
                        break;
                    case 'n':
                        pos.y += 0.5f;
                        isHalf = true;
                        break;
                    case 'e':
                        pos.x += (isHalf ? 0.5f : 1f);
                        isHalf = false;
                        break;
                    case 'w':
                        pos.x += (isHalf ? -0.5f : -1f);
                        isHalf = false;
                        break;
                    default:
                        Debug.LogError("Not supposed to happen, char '" + c + "' in instruction : " + instruction);
                        break;
                }
            }

            if (!_tilesFlipped.ContainsKey(pos))
            {
                _tilesFlipped.Add(pos, false);
            }
            _tilesFlipped[pos] = !_tilesFlipped[pos];
        }


        int dayCount = 100;
        for (int day = 0; day<dayCount; day++)
        {
            if (IsDebug)
            {
                string log = "";
                foreach (var tile in _tilesFlipped) { if (tile.Value) { log += tile.Key.ToString() + " / "; } }
                Debug.Log(log);
            }

            Dictionary<Vector2, bool> tilesFlippedOfDay = new Dictionary<Vector2, bool>();
            foreach (var tile in _tilesFlipped)
            {
                bool debug = IsDebug && (tile.Key == new Vector2(-2,-1));
                if (!tile.Value)
                    continue;

                if (debug)
                    Debug.Log("doing " + tile.Key);

                // if black compute himself and his 6 neighbours
                if (!tilesFlippedOfDay.ContainsKey(tile.Key))
                {
                    int count = getNeighboursFlippedCount(tile.Key, debug);
                    if (debug)
                        Debug.Log(tile.Key.ToString() + " neighbours count " + count);
                    tilesFlippedOfDay.Add(tile.Key, (count == 1 || count == 2));
                }
                else
                {
                    if (debug)
                        Debug.Log(tile.Key.ToString() + " already computed");
                }

                foreach (Vector2 pos in getNeighbours(tile.Key))
                {
                    if (debug)
                        Debug.Log(tile.Key.ToString() + " neighbour is : " + pos);

                    if (tilesFlippedOfDay.ContainsKey(pos))
                        continue;

                    if (debug)
                        Debug.Log("\tNot computd yet");

                    int count = getNeighboursFlippedCount(pos, debug);

                    if (debug)
                        Debug.Log("\tNeighbour count is " +count);

                    bool isFlipped = _tilesFlipped.ContainsKey(pos) && _tilesFlipped[pos];
                    if (isFlipped)
                        tilesFlippedOfDay.Add(pos, (count == 1 || count == 2));
                    else
                        tilesFlippedOfDay.Add(pos, count == 2);

                    if (debug)
                        Debug.Log("\tNeighbour value is : " + tilesFlippedOfDay[pos]);
                }
            }

            _tilesFlipped = new Dictionary<Vector2, bool>(tilesFlippedOfDay);

            Debug.Log("Day " + (day+1).ToString() + ": " + _tilesFlipped.Where(x => x.Value).Count());
        }

        foreach (var tile in _tilesFlipped)
        {
            //Debug.Log("Tile in pos " + tile.Key.ToString() + " is Flipped : " + tile.Value);
            if (tile.Value)
                result++;
        }
        Debug.Log("Part 2 result is : " + result + "\n");
    }

    int getNeighboursFlippedCount(Vector2 tilePos, bool isDebug = false)
    {
        bool debug = isDebug; // || (tilePos == new Vector2(-2,-1));
        int count = 0;

        foreach (Vector2 pos in getNeighbours(tilePos))
        {
            if (_tilesFlipped.ContainsKey(pos) && _tilesFlipped[pos])
            {
                if (IsDebug && debug)
                    Debug.Log("_tiles contain neighbour flipped at pos : " + pos.ToString());

                count++;
            }
            else
            {
                if (debug)
                    Debug.Log("tile not flipped at pos : " + pos.ToString());
            }
        }

        ////NE
        //Vector2 pos = new Vector2(x + 0.5f, y + 0.5f);
        //if (_tilesFlipped.ContainsKey(pos) && _tilesFlipped[pos])
        //{
        //    if (IsDebudebug)
        //    Debug.Log("_tiles contain neighbour flipped at pos : " + pos.ToString());
        //    count++;
        //}

        ////E
        //pos = new Vector2(x, y + 1f);
        //if (_tilesFlipped.ContainsKey(pos) && _tilesFlipped[pos])
        //{
        //    Debug.Log("_tiles contain neighbour flipped at pos : " + pos.ToString());
        //    count++;
        //}
        ////SE
        //pos = new Vector2(x - 0.5f, y + 0.5f);
        //if (_tilesFlipped.ContainsKey(pos) && _tilesFlipped[pos])
        //{
        //    Debug.Log("_tiles contain neighbour flipped at pos : " + pos.ToString());
        //    count++;
        //}
        ////NW
        //pos = new Vector2(x + 0.5f, y - 0.5f);
        //if (_tilesFlipped.ContainsKey(pos) && _tilesFlipped[pos])
        //{
        //    Debug.Log("_tiles contain neighbour flipped at pos : " + pos.ToString());
        //    count++;
        //}
        ////W
        //pos = new Vector2(x, y - 1f);
        //if (_tilesFlipped.ContainsKey(pos) && _tilesFlipped[pos])
        //{
        //    Debug.Log("_tiles contain neighbour flipped at pos : " + pos.ToString());
        //    count++;
        //}

        ////SW
        //pos = new Vector2(x - 0.5f, y - 0.5f);
        //if (_tilesFlipped.ContainsKey(pos) && _tilesFlipped[pos])
        //{
        //    Debug.Log("_tiles contain neighbour flipped at pos : " + pos.ToString());
        //    count++;
        //}

        return count;
    }

    List<Vector2> getNeighbours(Vector2 pos)
    {
        List<Vector2> result = new List<Vector2>();
        result.Add(new Vector2(pos.x + 0.5f, pos.y + 0.5f));
        result.Add(new Vector2(pos.x + 1f, pos.y));
        result.Add(new Vector2(pos.x - 0.5f, pos.y + 0.5f));
        result.Add(new Vector2(pos.x + 0.5f, pos.y - 0.5f));
        result.Add(new Vector2(pos.x - 1f, pos.y));
        result.Add(new Vector2(pos.x - 0.5f, pos.y - 0.5f));
        return result;
    }
}
