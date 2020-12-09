using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day8 : MonoBehaviour
{
    public const int Day = 8;

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
        yield return new WaitForSeconds(5f);
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void day_1()
    {
        int result = 0;

        string[] instructions = Tools.Instance.Input.Split('\n');
        int safetyCount = 0;
        HashSet<int> readLines = new HashSet<int>();
        int currentLineIndex = 0;

        while (safetyCount < instructions.Length)
        {
            Debug.Log("current line is " + currentLineIndex + " -> " + instructions[currentLineIndex]);

            if (readLines.Contains(currentLineIndex))
                break;

            if (currentLineIndex > instructions.Length || currentLineIndex < 0)
            {
                Debug.LogError("[Day8] line index out of bounds : " + currentLineIndex + " / " + instructions.Length);
                break;
            }

            readLines.Add(currentLineIndex);

            switch (instructions[currentLineIndex].Substring(0, 3))
            {
                case "jmp":
                    int offset = 0;
                    if (int.TryParse(instructions[currentLineIndex].Substring(4), out offset))
                    {
                        currentLineIndex += offset;
                    }
                    else
                    {
                        Debug.LogError("[Day8] can't parse offset in jmp line " + instructions[currentLineIndex].Substring(4) + " | current index : " + currentLineIndex);
                    }                        
                    break;

                case "acc":
                    int offsetAcc = 0;
                    if (int.TryParse(instructions[currentLineIndex].Substring(4), out offsetAcc))
                    {
                        result += offsetAcc;
                    }
                    else
                    {
                        Debug.LogError("[Day8] can't parse offset in acc line " + instructions[currentLineIndex].Substring(4) + " | current index : " + currentLineIndex);
                    }
                    currentLineIndex++;
                    break;

                case "nop":
                    currentLineIndex++;
                    break;

                default:
                    Debug.LogError("[Day8] Instruction Line malformed : " + instructions[currentLineIndex]);
                    break;
            }


            safetyCount++;
        }

      
        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }


    void day_2()
    {
        int result = 0;

        string[] instructions = Tools.Instance.Input.Split('\n');
        HashSet<int> readLines = new HashSet<int>();

        for (int i = 0; i < instructions.Length; i++)
        {
            if (instructions[i].StartsWith("acc"))
                continue;

            List<string> modifiedInstructions = new List<string>(instructions);
            if (instructions[i].StartsWith("jmp"))
                modifiedInstructions[i] = modifiedInstructions[i].Replace("jmp", "nop");
            else if (instructions[i].StartsWith("nop"))
                modifiedInstructions[i] = modifiedInstructions[i].Replace("nop", "jmp");

            if (modifiedInstructions.Last() == "")
            {
                modifiedInstructions.RemoveAt(modifiedInstructions.Count - 1);
            }

            Debug.Log("testing with swap in line " + i + " | instructions count is : " + modifiedInstructions.Count);
            KeyValuePair<bool, int> auxResult = simulateInstructions(modifiedInstructions, (i == 359));
            Debug.Log("   --> result is " + (auxResult.Key ? auxResult.Value.ToString() : "fail"));
            if (auxResult.Key)
            {
                result = auxResult.Value;
                break;
            }
        }

        //int maxJmpIndex = -1, index = 0;
        //List<int> exits = new List<int>();
        //foreach (string instr in instructions)
        //{
        //    if (instr.StartsWith("jmp -"))
        //            maxJmpIndex = index;
        //    else if (instr.StartsWith("jmp +"))
        //    {
        //        int offsetjmp = int.Parse(instr.Substring(5));
        //        if (index + offsetjmp > 620)
        //        {
        //            exits.Add(index);
        //            Debug.LogWarning("index is " + index + " and offset is " + offsetjmp);
        //        }
        //    }

        //    index++;
        //}
        //Debug.LogWarning("maxJmp index is " + maxJmpIndex);

        //List<int> jmpBeforeExits = new List<int>();
        //string log = "";
        //foreach (int exitIndex in exits)
        //{
        //    for (int i = exitIndex - 1; i >=0; i--)
        //    {
        //        if (instructions[i].StartsWith("jmp -"))
        //        {
        //            jmpBeforeExits.Add(i);
        //            log += i +", ";
        //            break;
        //        }
        //        else if (instructions[i].StartsWith("jmp +")
        //            && i+int.Parse(instructions[i].Substring(5)) > exitIndex)
        //        {
        //            jmpBeforeExits.Add(i);
        //            log += i + ", ";
        //            break;
        //        }
        //    }
        //}
        //Debug.LogWarning("JmpBeforeExits are " + log);

        //while (safetyCount < instructions.Length)
        //{
        //    Debug.Log("current line is " + currentLineIndex + " -> " + instructions[currentLineIndex]);

        //    if (readLines.Contains(currentLineIndex))
        //    {
        //        Debug.LogWarning("[Day8_2] Doublon for index : " + currentLineIndex);
        //        break;
        //    }

        //    if (currentLineIndex > instructions.Length || currentLineIndex < 0)
        //    {
        //        Debug.LogError("[Day8] line index out of bounds : " + currentLineIndex + " / " + instructions.Length);
        //        break;
        //    }

        //    readLines.Add(currentLineIndex);

        //    //specialcases for part2
        //    if (jmpBeforeExits.Contains(currentLineIndex))
        //    {
        //        Debug.LogWarning("Tata");
        //        instructions[currentLineIndex] = instructions[currentLineIndex].Replace("jmp", "nop");
        //    }
        //    if (instructions[currentLineIndex].StartsWith("nop"))
        //    {
        //        Debug.Log("tutu");
        //        int offsetNop = int.Parse(instructions[currentLineIndex].Substring(4));
        //        if (offsetNop + currentLineIndex > maxJmpIndex)
        //        {
        //            Debug.LogWarning("yes");
        //            instructions[currentLineIndex] = instructions[currentLineIndex].Replace("nop", "jmp");
        //        }
        //    }
        //    else if (currentLineIndex == maxJmpIndex)
        //    {
        //        Debug.Log("toto");
        //        instructions[currentLineIndex] = instructions[currentLineIndex].Replace("jmp", "nop");
        //    }

        //    //if (currentLineIndex == 339)
        //    //{
        //    //    Debug.LogWarning("tata");
        //    //    instructions[currentLineIndex] = instructions[currentLineIndex].Replace("jmp", "nop");
        //    //}

        //    switch (instructions[currentLineIndex].Substring(0, 3))
        //    {
        //        case "jmp":
        //            int offset = 0;
        //            if (int.TryParse(instructions[currentLineIndex].Substring(4), out offset))
        //            {
        //                currentLineIndex += offset;
        //            }
        //            else
        //            {
        //                Debug.LogError("[Day8] can't parse offset in jmp line " + instructions[currentLineIndex].Substring(4) + " | current index : " + currentLineIndex);
        //            }
        //            break;

        //        case "acc":
        //            int offsetAcc = 0;
        //            if (int.TryParse(instructions[currentLineIndex].Substring(4), out offsetAcc))
        //            {
        //                result += offsetAcc;
        //            }
        //            else
        //            {
        //                Debug.LogError("[Day8] can't parse offset in acc line " + instructions[currentLineIndex].Substring(4) + " | current index : " + currentLineIndex);
        //            }
        //            currentLineIndex++;
        //            break;

        //        case "nop":
        //            currentLineIndex++;
        //            break;

        //        default:
        //            Debug.LogError("[Day8] Instruction Line malformed : " + instructions[currentLineIndex]);
        //            break;
        //    }


        //    safetyCount++;
        //}

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }

    KeyValuePair<bool, int> simulateInstructions(List<string> instructions, bool debug = false)
    {
        int result = 0;
        int safetyCount = 0;
        HashSet<int> readLines = new HashSet<int>();
        int currentLineIndex = 0;

        while (safetyCount <= instructions.Count)
        {
            if (currentLineIndex == instructions.Count)
            {
                Debug.LogWarning("SUCCESS");
                return new KeyValuePair<bool, int>(true, result);
            }

            if (debug)
                Debug.Log("current line is " + currentLineIndex + " -> " + instructions[currentLineIndex]);

            if (readLines.Contains(currentLineIndex))
            {
                Debug.Log("   -> [Day8_2] Doublon for index : " + currentLineIndex);
                return new KeyValuePair<bool, int>(false, -1);
            }

            if (currentLineIndex > instructions.Count || currentLineIndex < 0)
            {
                Debug.LogError("[Day8] line index out of bounds : " + currentLineIndex + " / " + instructions.Count);
                return new KeyValuePair<bool, int>(false, -1);
            }

            readLines.Add(currentLineIndex);

            switch (instructions[currentLineIndex].Substring(0, 3))
            {
                case "jmp":
                    int offset = 0;
                    if (int.TryParse(instructions[currentLineIndex].Substring(4), out offset))
                        currentLineIndex += offset;
                    else
                        Debug.LogError("[Day8] can't parse offset in jmp line " + instructions[currentLineIndex].Substring(4) + " | current index : " + currentLineIndex);
                    break;

                case "acc":
                    int offsetAcc = 0;

                    if (int.TryParse(instructions[currentLineIndex].Substring(4), out offsetAcc))
                        result += offsetAcc;
                    else
                        Debug.LogError("[Day8] can't parse offset in acc line " + instructions[currentLineIndex].Substring(4) + " | current index : " + currentLineIndex);

                    currentLineIndex++;
                    break;

                case "nop":
                    currentLineIndex++;
                    break;

                default:
                    Debug.LogError("[Day8] Instruction Line malformed : " + instructions[currentLineIndex]);
                    break;
            }

            safetyCount++;
        }
        return new KeyValuePair<bool, int>(true, result);
    }

}
