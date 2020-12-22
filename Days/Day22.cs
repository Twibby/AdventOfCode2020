using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day22 : MonoBehaviour
{
    public bool IsDebug = false;
    public bool IsTestInput = false;
    public bool Part1 = true;
    public bool Part2 = true;

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

    public Queue<int> _deck1 = new Queue<int>();
    public Queue<int> _deck2 = new Queue<int>();

    void initDecks()
    {
        _deck1 = new Queue<int>();
        _deck2 = new Queue<int>();

        List<string> preDeck = new List<string>(_input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[0].Split('\n'));
        preDeck.RemoveAt(0);
        _deck1 = new Queue<int>(preDeck.Select(int.Parse));

        preDeck = new List<string>(_input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries)[1].Split('\n'));
        preDeck.RemoveAt(0);
        _deck2 = new Queue<int>(preDeck.Select(int.Parse));
    }

    long computeWinningScore(Queue<int> deck)
    {
        long result = 0;
        while (deck.Count > 0)
        {
            result += deck.Count * deck.Dequeue();
        }
        return result;
    }

    void day_1()
    {
        long result = 0;

        initDecks();

        int safetyCount = 100000; // max 100k loops
        while (_deck1.Count > 0 && _deck2.Count > 0 && safetyCount > 0)
        {
            int a = _deck1.Dequeue();
            int b = _deck2.Dequeue();
            if (a > b)
            {
                _deck1.Enqueue(a);
                _deck1.Enqueue(b);
            }
            else
            {
                _deck2.Enqueue(b);
                _deck2.Enqueue(a);
            }
            safetyCount--;
        }
        Debug.Log((100000 - safetyCount).ToString() + " turns made");
        if (_deck2.Count > 0 && _deck1.Count > 0)
        {
            Debug.Log("need more turns ?");
            return;
        }

        result = computeWinningScore(_deck1.Count > 0 ? _deck1 : _deck2);

        Debug.Log("Part 1 result is : " + result + "\n");
    }


    void day_2()
    {
        long result = 0;

        initDecks();

        _mainMemory = new Dictionary<string, bool>();
        bool p1win = recursiveGame(_deck1, _deck2);

        result = computeWinningScore(p1win ? _deck1 : _deck2);
        Debug.Log("Part 2 result is : " + result + "\n");
    }

    Dictionary<string, bool> _mainMemory = new Dictionary<string, bool>();

    bool recursiveGame(Queue<int> deck1, Queue<int> deck2, int offset = 0)
    {
        if (IsDebug)
            Debug.Log(Tools.writeOffset(offset) + "=== STARTING SUBGAME " + (offset + 1) + " ===");

        int loopCount = 100000;
        int safetyCount = loopCount;
        List<string> memory = new List<string>();
        while (deck1.Count > 0 && deck2.Count > 0 && safetyCount > 0)
        {
            if (IsDebug)
            {
                Debug.Log(Tools.writeOffset(offset) + "-- Round " + (loopCount + 1 - safetyCount).ToString() + " (Game " + (offset + 1).ToString() + ") --");
                string log = "Player 1's deck: ";
                deck1.ToList().ForEach(x => log += x + " ");
                Debug.Log(Tools.writeOffset(offset) + log);
                log = "Player 2's deck: ";
                deck2.ToList().ForEach(x => log += x + " ");
                Debug.Log(Tools.writeOffset(offset) + log);
            }

            string decksToString = "";
            deck1.ToList().ForEach(x => decksToString += x + ",");
            decksToString += "/";
            deck2.ToList().ForEach(x => decksToString += x + ",");

            if (_mainMemory.ContainsKey(decksToString))
            {
                if (IsDebug)
                    Debug.LogWarning("Game previously played, result is : " + _mainMemory[decksToString].ToString());

                return _mainMemory[decksToString];
            }

            if (memory.Contains(decksToString))
            {
                if (IsDebug)
                    Debug.Log("Same decks already played so player 1 wins instantly !");

                return true;
            }
            memory.Add(decksToString);

            int a = deck1.Dequeue();
            int b = deck2.Dequeue();

            bool player1wins = true;
            if (a > deck1.Count || b > deck2.Count)
            {
                player1wins = a > b;
                if (IsDebug)
                    Debug.Log(Tools.writeOffset(offset) + "P1 plays : " + a + " | P2 plays : " + b + "  --> Player " + (player1wins ? "1" : "2") + " wins round " + (loopCount + 1 - safetyCount).ToString() + " of game " + (offset + 1).ToString() + " ! \n");
            }
            else
            {
                if (IsDebug)
                    Debug.Log(Tools.writeOffset(offset) + "P1 plays : " + a + " | P2 plays : " + b + "  --> Playing a sub-game to determine the winner... \n");

                Queue<int> subdeck1 = new Queue<int>(new List<int>(deck1).GetRange(0, a));
                Queue<int> subdeck2 = new Queue<int>(new List<int>(deck2).GetRange(0, b));
                player1wins = recursiveGame(subdeck1, subdeck2, offset + 1);
            }

            if (player1wins)
            {
                deck1.Enqueue(a);
                deck1.Enqueue(b);
            }
            else
            {
                deck2.Enqueue(b);
                deck2.Enqueue(a);
            }

            safetyCount--;
        }
        if (IsDebug)
            Debug.Log(Tools.writeOffset(offset) + (loopCount + 1 - safetyCount).ToString() + " turns made");

        if (deck2.Count > 0 && deck1.Count > 0)
        {
            Debug.LogError("need more turns ? in subgame " + offset);
            return true;
        }

        foreach (string decks in memory)
        {
            _mainMemory.Add(decks, deck1.Count > 0);
        }

        if (IsDebug)
        {
            Debug.Log(Tools.writeOffset(offset) + " ** " + memory.Count);
            foreach (var mem in _mainMemory)
            {
                Debug.Log(Tools.writeOffset(offset) + "MAAAIN MEMORY  //  " + mem);
            }
        }
        return deck1.Count > 0;
    }
}
