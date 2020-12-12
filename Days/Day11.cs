using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day11 : MonoBehaviour
{
    public const int Day = 11;
    public bool IsDebug = false;

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

    List<string> _inputs = new List<string>();
    int _rowLength = 0;

    void day_1()
    {
        int result = 0;
        int safetyCount = 0;
        bool hasChanged = false;

        _inputs = new List<string>(Tools.Instance.Input.Split('\n'));
        _rowLength = Tools.Instance.Input.IndexOf('\n');

        //Debug.Log(_rowLength + " [ " + inputs.Count + " | " + Tools.Instance.Input.Split('\n').Length);
        //string log = "";
        //inputs.GetRange(12 * _rowLength, _rowLength).ForEach(x => log += x);
        //Debug.Log(log);

        do
        {
            hasChanged = false;

            List<string> newInputs = new List<string>();
            for (int i = 0; i < _inputs.Count; i++)
            {
                newInputs.Add("");
                for (int j = 0; j < _rowLength; j++)
                {
                    switch (_inputs[i][j])
                    {
                        case '.': newInputs[i] += '.'; break;
                        case 'L':
                            if (getOccupiedNeighboursSeatsCount(i,j) == 0)
                            {
                                newInputs[i] += '#';
                                hasChanged = true;
                            }
                            else
                            {
                                newInputs[i] += 'L';
                            }
                            break;
                        case '#':
                            if (getOccupiedNeighboursSeatsCount(i,j) >= 4)
                            {
                                newInputs[i] += 'L';
                                hasChanged = true;
                            }
                            else
                            {
                                newInputs[i] += '#';
                            }
                            break;
                        default:
                            Debug.LogError("[Day11] Error in inputs format : " + _inputs[i]);
                            break;
                    }
                }
            }

            safetyCount++;

            if (IsDebug)
            {
                Debug.Log("AFTER STEP " + safetyCount);
                writeInputs(newInputs);
            }

            _inputs = new List<string>(newInputs);


        } while (safetyCount < 1000 && hasChanged);

        Debug.Log(safetyCount);

        foreach (string row in _inputs)
        {
            result += row.Where(x => x == '#').Count();
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }


    void day_2()
    {
        long result = 0;

        int safetyCount = 0;
        bool hasChanged = false;

        _inputs = new List<string>(Tools.Instance.Input.Split('\n'));
        _rowLength = Tools.Instance.Input.IndexOf('\n');

        //Debug.Log(_rowLength + " [ " + inputs.Count + " | " + Tools.Instance.Input.Split('\n').Length);
        //string log = "";
        //inputs.GetRange(12 * _rowLength, _rowLength).ForEach(x => log += x);
        //Debug.Log(log);

        do
        {
            hasChanged = false;

            List<string> newInputs = new List<string>();
            for (int i = 0; i < _inputs.Count; i++)
            {
                newInputs.Add("");
                for (int j = 0; j < _rowLength; j++)
                {
                    switch (_inputs[i][j])
                    {
                        case '.': newInputs[i] += '.'; break;
                        case 'L':
                            if (getOccupiedNeighboursSeatsCount2(i, j) == 0)
                            {
                                newInputs[i] += '#';
                                hasChanged = true;
                            }
                            else
                            {
                                newInputs[i] += 'L';
                            }
                            break;
                        case '#':
                            if (getOccupiedNeighboursSeatsCount2(i, j) >= 5)
                            {
                                newInputs[i] += 'L';
                                hasChanged = true;
                            }
                            else
                            {
                                newInputs[i] += '#';
                            }
                            break;
                        default:
                            Debug.LogError("[Day11] Error in inputs format : " + _inputs[i]);
                            break;
                    }
                }
            }

            safetyCount++;

            if (IsDebug)
            {
                Debug.Log("AFTER STEP " + safetyCount);
                writeInputs(newInputs);
            }

            _inputs = new List<string>(newInputs);


        } while (safetyCount < 1000 && hasChanged);

        Debug.Log(safetyCount);

        foreach (string row in _inputs)
        {
            result += row.Where(x => x == '#').Count();
        }

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }


    int getOccupiedNeighboursSeatsCount(int row, int column)
    {
        bool _debug = (row == 3 && column == 3);
        string log = "Current seat is " + _inputs[row][column] + " | inputs.Count is : " + _inputs.Count + " & rowLength is " + _rowLength;

        int occupiedSeatsCount = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j<=1; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                if (row +i >=0 && row+i < _inputs.Count
                    && column+j >= 0 && column+j < _rowLength)
                {

                    occupiedSeatsCount += _inputs[row+i][column+j] == '#' ? 1 : 0;
                    log += "Seat of line " + (row + i).ToString() + " & column " + (column + j).ToString() + " is : " + _inputs[row + i][column + j];
                }
            }
        }

        if (IsDebug && _debug)
            Debug.LogWarning(log);

        return occupiedSeatsCount;
    }

    int getOccupiedNeighboursSeatsCount2(int row, int column)
    {
        bool _debug = (row == 48 && column == 41);
        string log = "Current seat is " + _inputs[row][column] + " | inputs.Count is : " + _inputs.Count + " & rowLength is " + _rowLength + System.Environment.NewLine;

        int occupiedSeatsCount = 0;
        int i = 0, j = 0;


        //top-left
        i = row - 1; j = column - 1;
        while (isFloor(i, j))
        {
            i--; j--;
        }
        if (validIndex(i, j))
            occupiedSeatsCount += _inputs[i][j] == '#' ? 1 : 0;
        if (_debug)
            log += "row is " + i + " & column is " + j + " | valid ? " + (validIndex(i, j) ? validIndex(i, j).ToString() + " -> " + _inputs[i][j] : false.ToString()) + System.Environment.NewLine;

        //top-middle
        i = row - 1; j = column;
        while (isFloor(i, j))
        {
            i--;
        }
        if (validIndex(i, j))
            occupiedSeatsCount += _inputs[i][j] == '#' ? 1 : 0;
        if (_debug)
            log += "row is " + i + " & column is " + j + " | valid ? " + (validIndex(i, j) ? validIndex(i, j).ToString() + " -> " + _inputs[i][j] : false.ToString()) + System.Environment.NewLine;

        //top-right
        i = row - 1; j = column + 1;
        while (isFloor(i, j))
        {
            i--; j++;
        }
        if (validIndex(i, j))
            occupiedSeatsCount += _inputs[i][j] == '#' ? 1 : 0;
        if (_debug)
            log += "row is " + i + " & column is " + j + " | valid ? " + (validIndex(i, j) ? validIndex(i, j).ToString() + " -> " + _inputs[i][j] : false.ToString()) + System.Environment.NewLine;

        //middle-left
        i = row; j = column - 1;
        while (isFloor(i, j))
        {
            j--;
        }
        if (validIndex(i, j))
            occupiedSeatsCount += _inputs[i][j] == '#' ? 1 : 0;
        if (_debug)
            log += "row is " + i + " & column is " + j + " | valid ? " + (validIndex(i, j) ? validIndex(i, j).ToString() + " -> " + _inputs[i][j] : false.ToString()) + System.Environment.NewLine;

        //middle-right
        i = row ; j = column + 1;
        while (isFloor(i, j))
        {
            j++;
        }
        if (validIndex(i, j))
            occupiedSeatsCount += _inputs[i][j] == '#' ? 1 : 0;
        if (_debug)
            log += "row is " + i + " & column is " + j + " | valid ? " + (validIndex(i, j) ? validIndex(i, j).ToString() + " -> " + _inputs[i][j] : false.ToString()) + System.Environment.NewLine;

        //bottom-left
        i = row + 1; j = column - 1;
        while (isFloor(i, j))
        {
            i++; j--;
        }
        if (validIndex(i, j))
            occupiedSeatsCount += _inputs[i][j] == '#' ? 1 : 0;
        if (_debug)
            log += "row is " + i + " & column is " + j + " | valid ? " + (validIndex(i, j) ? validIndex(i, j).ToString() + " -> " + _inputs[i][j] : false.ToString()) + System.Environment.NewLine;

        //bottom-middle
        i = row + 1; j = column;
        while (isFloor(i, j))
        {
            i++;
        }
        if (validIndex(i, j))
            occupiedSeatsCount += _inputs[i][j] == '#' ? 1 : 0;
        if (_debug)
            log += "row is " + i + " & column is " + j + " | valid ? " + (validIndex(i, j) ? validIndex(i, j).ToString() + " -> " + _inputs[i][j] : false.ToString()) + System.Environment.NewLine;

        //bottom-right
        i = row + 1; j = column + 1;
        while (isFloor(i, j))
        {
            i++; j++;
        }
        if (validIndex(i, j))
            occupiedSeatsCount += _inputs[i][j] == '#' ? 1 : 0;
        if (_debug)
            log += "row is " + i + " & column is " + j + " | valid ? " + (validIndex(i, j) ? validIndex(i, j).ToString() + " -> " + _inputs[i][j] : false.ToString()) + System.Environment.NewLine;

        if (IsDebug && _debug)
            Debug.LogWarning(log);

        return occupiedSeatsCount;
    }

    bool validIndex(int row, int column)
    {
        return row >= 0 && row < _inputs.Count
            && column >= 0 && column < _rowLength;
    }

    bool isFloor(int row, int column)  { return validIndex(row, column) && _inputs[row][column] == '.'; }
    

    void writeInputs(List<string> inputs)
    {
        string log = "";
        inputs.ForEach(x => log += x + System.Environment.NewLine);
        Debug.Log(log);
    }
}
