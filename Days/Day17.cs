using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day17 : MonoBehaviour
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
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        day_2();

        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        altDay_2();

        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(5f);
        UnityEditor.EditorApplication.isPlaying = false;
    }

    List<Vector3> _cubePos = new List<Vector3>();
    List<Vector4> _cubePos2 = new List<Vector4>();
    Dictionary<Vector4, bool> _altCubePos2 = new Dictionary<Vector4, bool>();

    void day_1()
    {
        int result = 0;

        List<string> _inputs =  new List<string>(Tools.Instance.Input.Split('\n')); // new List<string>(".#.\n..#\n###".Split('\n'));
        int rowLength = Tools.Instance.Input.IndexOf('\n');

        Debug.Log(rowLength + " [ " + _inputs.Count + " | " + Tools.Instance.Input.Split('\n').Length + " | " + _inputs[0].Length);

        Vector2 xBounds = new Vector2(_inputs[0].Length, 0);
        Vector2 yBounds = new Vector2(_inputs.Count, 0);
        Vector2 zBounds = Vector2.zero;


        _cubePos = new List<Vector3>();
        for (int j = 0; j < _inputs.Count; j++)
        {
            for (int i = 0; i <_inputs[j].Length; i++)
            {
                if (_inputs[j][i] == '#')
                {
                    _cubePos.Add(new Vector3(i, j, 0));
                    xBounds.x = Mathf.Min(xBounds.x, i);
                    xBounds.y = Mathf.Max(xBounds.y, i);
                    yBounds.x = Mathf.Min(yBounds.x, j);
                    yBounds.y = Mathf.Max(yBounds.y, j);
                }
            }
        }

        if (IsDebug)
        {
            Debug.Log("AFTER INIT");
            string log = "Cubes count is : " + _cubePos.Count + System.Environment.NewLine;
            _cubePos.ForEach(x => log += x.ToString() + ", ");
            Debug.Log(log);
        }

        for (int cycle = 0; cycle < 6; cycle++)
        {
            List<Vector3> newCubes = new List<Vector3>();
            Vector2 xBounds_new = new Vector2(xBounds.y+1, xBounds.x-1);
            Vector2 yBounds_new = new Vector2(yBounds.y+1, yBounds.x-1);
            Vector2 zBounds_new = new Vector2(zBounds.y+1, zBounds.x-1);

            // loop on x, y & z
            for (int x = (int)xBounds.x - 1; x <= xBounds.y + 1; x++)
            {
                for (int y = (int)yBounds.x - 1; y <= yBounds.y + 1; y++)
                {
                    for (int z = (int)zBounds.x - 1; z <= zBounds.y + 1; z++)
                    {
                        int count = getOccupiedNeighboursCount(x, y, z);
                        if (count == 3 
                            || (count == 2 && _cubePos.Contains(new Vector3(x,y,z))))
                        {
                            newCubes.Add(new Vector3(x, y, z));

                            xBounds_new.x = Mathf.Min(xBounds_new.x, x);
                            xBounds_new.y = Mathf.Max(xBounds_new.y, x);
                            yBounds_new.x = Mathf.Min(yBounds_new.x, y);
                            yBounds_new.y = Mathf.Max(yBounds_new.y, y);
                            zBounds_new.x = Mathf.Min(zBounds_new.x, z);
                            zBounds_new.y = Mathf.Max(zBounds_new.y, z);
                        }
                    }
                }
            }

            newCubes.Sort(delegate (Vector3 c1, Vector3 c2)
            {
                if (c1.z != c2.z)
                    return c1.z.CompareTo(c2.z);
                if (c1.x != c2.x)
                    return c1.x.CompareTo(c2.x);

                return c1.y.CompareTo(c2.y);
            }
            );

            _cubePos = new List<Vector3>(newCubes);
            xBounds = new Vector2(xBounds_new.x, xBounds_new.y);
            yBounds = new Vector2(yBounds_new.x, yBounds_new.y);
            zBounds = new Vector2(zBounds_new.x, zBounds_new.y);

            if (IsDebug)
            {
                Debug.Log("AFTER STEP " + cycle);
                string log = "Cubes count is : " + _cubePos.Count + System.Environment.NewLine;
                _cubePos.ForEach(x => log += x.ToString() + ", ");
                Debug.Log(log);
            }

        }

        result = _cubePos.Count;

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }


    void day_2()
    {
        long result = 0;

        List<string> _inputs = new List<string>(Tools.Instance.Input.Split('\n')); // new List<string>(".#.\n..#\n###".Split('\n'));
        int rowLength = Tools.Instance.Input.IndexOf('\n');

        Debug.Log(rowLength + " [ " + _inputs.Count + " | " + Tools.Instance.Input.Split('\n').Length + " | " + _inputs[0].Length);

        Vector2 xBounds = new Vector2(_inputs[0].Length, 0);
        Vector2 yBounds = new Vector2(_inputs.Count, 0);
        Vector2 zBounds = Vector2.zero;
        Vector2 wBounds = Vector2.zero;


        _cubePos2 = new List<Vector4>();
        for (int j = 0; j < _inputs.Count; j++)
        {
            for (int i = 0; i < _inputs[j].Length; i++)
            {
                if (_inputs[j][i] == '#')
                {
                    _cubePos2.Add(new Vector4(i, j, 0, 0));
                    xBounds.x = Mathf.Min(xBounds.x, i);
                    xBounds.y = Mathf.Max(xBounds.y, i);
                    yBounds.x = Mathf.Min(yBounds.x, j);
                    yBounds.y = Mathf.Max(yBounds.y, j);
                }
            }
        }

        if (IsDebug)
        {
            Debug.Log("AFTER INIT");
            string log = "Cubes count is : " + _cubePos2.Count + System.Environment.NewLine;
            _cubePos2.ForEach(x => log += x.ToString() + ", ");
            Debug.Log(log);
        }

        for (int cycle = 0; cycle < 6; cycle++)
        {
            List<Vector4> newCubes = new List<Vector4>();
            Vector2 xBounds_new = new Vector2(xBounds.y + 1, xBounds.x - 1);
            Vector2 yBounds_new = new Vector2(yBounds.y + 1, yBounds.x - 1);
            Vector2 zBounds_new = new Vector2(zBounds.y + 1, zBounds.x - 1);
            Vector2 wBounds_new = new Vector2(wBounds.y + 1, wBounds.x - 1);

            // loop on x, y & z & t
            for (int x = (int)xBounds.x - 1; x <= xBounds.y + 1; x++)
            {
                for (int y = (int)yBounds.x - 1; y <= yBounds.y + 1; y++)
                {
                    for (int z = (int)zBounds.x - 1; z <= zBounds.y + 1; z++)
                    {
                        for (int w = (int)wBounds.x - 1; w <= wBounds.y + 1; w++)
                        {
                            int count = getOccupiedNeighboursCount2(x, y, z,w);
                            if (count == 3
                                || (count == 2 && _cubePos2.Contains(new Vector4(x, y, z,w))))
                            {
                                newCubes.Add(new Vector4(x, y, z,w));

                                xBounds_new.x = Mathf.Min(xBounds_new.x, x);
                                xBounds_new.y = Mathf.Max(xBounds_new.y, x);
                                yBounds_new.x = Mathf.Min(yBounds_new.x, y);
                                yBounds_new.y = Mathf.Max(yBounds_new.y, y);
                                zBounds_new.x = Mathf.Min(zBounds_new.x, z);
                                zBounds_new.y = Mathf.Max(zBounds_new.y, z);
                                wBounds_new.x = Mathf.Min(wBounds_new.x, w);
                                wBounds_new.y = Mathf.Max(wBounds_new.y, w);
                            }
                        }
                    }
                }
            }

            newCubes.Sort(delegate (Vector4 c1, Vector4 c2)
            {
                if (c1.w != c2.w)
                    return c1.w.CompareTo(c2.w);
                if (c1.z != c2.z)
                    return c1.z.CompareTo(c2.z);
                if (c1.x != c2.x)
                    return c1.x.CompareTo(c2.x);

                return c1.y.CompareTo(c2.y);
            }
            );

            _cubePos2 = new List<Vector4>(newCubes);
            xBounds = new Vector2(xBounds_new.x, xBounds_new.y);
            yBounds = new Vector2(yBounds_new.x, yBounds_new.y);
            zBounds = new Vector2(zBounds_new.x, zBounds_new.y);
            wBounds = new Vector2(wBounds_new.x, wBounds_new.y);

            if (IsDebug)
            {
                Debug.Log("AFTER STEP " + cycle);
                string log = "Cubes count is : " + _cubePos2.Count + System.Environment.NewLine;
                _cubePos2.ForEach(x => log += x.ToString() + ", ");
                Debug.Log(log);
            }



            if (IsDebug)
            {
                Debug.LogWarning(xBounds.ToString() + " | " + yBounds.ToString() + " | " + zBounds.ToString() + " | " + wBounds.ToString());
            }

        }
        result = _cubePos2.Count;

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }

    // Main idea : foreach cube, compute future state of all his neighbours (if not already computed) so it costs less than compute all space with bounds. (something that is in the middle of nothing will not be computed)
    void altDay_2()
    {
        long result = 0;

        List<string> _inputs = new List<string>(Tools.Instance.Input.Split('\n')); //  new List<string>(".#.\n..#\n###".Split('\n')); 

        int rowLength = Tools.Instance.Input.IndexOf('\n');
        Debug.Log(rowLength + " [ " + _inputs.Count + " | " + Tools.Instance.Input.Split('\n').Length + " | " + _inputs[0].Length);


        _altCubePos2 = new Dictionary<Vector4, bool>();
        for (int j = 0; j < _inputs.Count; j++)
        {
            for (int i = 0; i < _inputs[j].Length; i++)
            {
                if (_inputs[j][i] == '#')
                {
                    _altCubePos2.Add(new Vector4(i, j, 0, 0), true);
                }
            }
        }

        if (IsDebug)
        {
            Debug.Log("AFTER INIT");
            string log = "Cubes count is : " + _altCubePos2.Count + System.Environment.NewLine;
            foreach (Vector4 cube in _altCubePos2.Keys)
            {
                log += cube.ToString() + ", ";
            }
            Debug.Log(log);
        }

        for (int cycle = 0; cycle < 6; cycle++)
        {
            Dictionary<Vector4, bool> newCubes = new Dictionary<Vector4, bool>();

            foreach (Vector4 cube in _altCubePos2.Keys)
            {
                if (!_altCubePos2[cube])
                    continue;

                // loop on x, y & z & w
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int z = -1; z <= 1; z++)
                        {
                            for (int w = -1; w <= 1; w++)
                            {
                                if (newCubes.ContainsKey(new Vector4(cube.x + x, cube.y + y, cube.z + z, cube.w + w)))  // pos already computed
                                    continue;

                                int count = altGetOccupiedNeighboursCount2((int)cube.x + x, (int)cube.y + y, (int)cube.z + z, (int)cube.w + w);
                                bool isCube = (count == 3
                                    || (count == 2 && _altCubePos2.ContainsKey(new Vector4((int)cube.x + x, (int)cube.y + y, (int)cube.z + z, (int)cube.w + w))));


                                newCubes.Add(new Vector4(cube.x + x, cube.y + y, cube.z + z, cube.w + w), isCube);

                            }
                        }
                    }
                }
            }

            _altCubePos2 = new Dictionary<Vector4, bool>();
            foreach (var cube in newCubes)
            {
                if (cube.Value)
                    _altCubePos2.Add(cube.Key, cube.Value);
            }

            if (IsDebug)
            {
                Debug.Log("AFTER STEP " + cycle);
                string log = "Cubes count is : " + _altCubePos2.Count + System.Environment.NewLine;
                foreach (Vector4 cube in _altCubePos2.Keys)
                {
                    log += cube.ToString() + ", ";
                }
                Debug.Log(log);
            }
        }
        result = _altCubePos2.Count;

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }


    int getOccupiedNeighboursCount(int x, int y, int z)
    {
        bool _debug = (x == 0 && y == 2);
        string log = "Current pos is " + x + "," + y + "," + z + " | ";

        int occupiedSeatsCount = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    if (i == 0 && j == 0 && k == 0)
                        continue;

                    if (_cubePos.Contains(new Vector3(x+i, y+j, z+k)))
                    {
                        occupiedSeatsCount++;
                        log += "Cube founded at coord " + (x+i).ToString() + ", " + (y+j).ToString() + ", " + (z+k).ToString();
                    }
                }
            }
        }

        if (IsDebug && _debug)
            Debug.LogWarning(log);

        return occupiedSeatsCount;
    }

    int getOccupiedNeighboursCount2(int x, int y, int z, int w)
    {
        bool _debug = false;// (x == 0 && y == 2);
        string log = "Current pos is " + x + "," + y + "," + z + " | ";

        int neighbourCubesCount = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    for (int l = -1; l <= 1; l++)
                    {
                        if (i == 0 && j == 0 && k == 0 && l == 0)
                            continue;

                        if (_cubePos2.Contains(new Vector4(x + i, y + j, z + k, w + l)))
                        {
                            neighbourCubesCount++;
                            log += "Cube founded at coord " + (x + i).ToString() + ", " + (y + j).ToString() + ", " + (z + k).ToString() + ", " + (w + l).ToString();
                        }
                    }
                }
            }
        }

        if (IsDebug && _debug)
            Debug.LogWarning(log);

        return neighbourCubesCount;
    }

    int altGetOccupiedNeighboursCount2(int x, int y, int z, int w)
    {
        bool _debug = z == 0 && w==0;
        string log = "Current pos is " + x + "," + y + "," + z + "," + w + " | ";

        int neighbourCubesCount = 0;
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    for (int l = -1; l <= 1; l++)
                    {
                        if (i == 0 && j == 0 && k == 0 && l == 0)
                            continue;

                        if (_altCubePos2.ContainsKey(new Vector4(x + i, y + j, z + k, w + l)))
                        {
                            neighbourCubesCount++;
                            log += "Cube founded at coord " + (x + i).ToString() + ", " + (y + j).ToString() + ", " + (z + k).ToString() + ", " + (w + l).ToString();
                        }
                    }
                }
            }
        }

        if (IsDebug && _debug)
            Debug.LogWarning(log + System.Environment.NewLine + " -> Count is : " + neighbourCubesCount);

        return neighbourCubesCount;
    }

}
