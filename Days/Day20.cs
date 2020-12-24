using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day20 : MonoBehaviour
{
    public bool IsDebug = false;
    public bool IsTestInput = false;
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

        _tiles = new Dictionary<int, Tile>();
        foreach (string inputTile in _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries))
        {
            int index = int.Parse(inputTile.Substring(5, 4)); // we could make it dynamic but dc
            List<string> content = new List<string>(inputTile.Split('\n'));
            content.RemoveAt(0);

            Tile myTile = new Tile(index, content);

            Dictionary<string, int> tmp = new Dictionary<string, int>();
            foreach (string edge in myTile.edges.Keys)
            {
                tmp.Add(edge, 0);
                foreach (Tile t in _tiles.Values)
                {
                    if (t.edges.ContainsKey(edge))
                    {
                        t.edges[edge] += 1;
                        tmp[edge] += 1;
                    }

                    if (t.reversedEdges.ContainsKey(edge))
                    {
                        t.reversedEdges[edge] += 1;
                        tmp[edge] += 1;
                    }
                }
            }
            myTile.edges = new Dictionary<string, int>(tmp);

            tmp = new Dictionary<string, int>();
            foreach (string edge in myTile.reversedEdges.Keys)
            {
                tmp.Add(edge, 0);
                foreach (Tile t in _tiles.Values)
                {
                    if (t.edges.ContainsKey(edge))
                    {
                        t.edges[edge] += 1;
                        tmp[edge] += 1;
                    }

                    if (t.reversedEdges.ContainsKey(edge))
                    {
                        t.reversedEdges[edge] += 1;
                        tmp[edge] += 1;
                    }
                }
            }
            myTile.reversedEdges = new Dictionary<string, int>(tmp);

            _tiles.Add(index, myTile);
        }
        result = 1;
        foreach (Tile t in _tiles.Values)
        {
            if (IsDebug)
            {
                Debug.Log("Tile " + t.id + " has " + t.EdgesMatchingCount + " edges matching");
                string log = "Edges : ";
                foreach (var edge in t.edges) { log += edge.Key + " (" + edge.Value + ")  /  "; if (edge.Value > 1) { Debug.LogWarning("!!! " + edge.Value); } }
                log += "\t\t\t";
                foreach (var edge in t.reversedEdges) { log += edge.Key + " (" + edge.Value + ")  /  "; if (edge.Value > 1) { Debug.LogWarning("!!! " + edge.Value); } }

                Debug.Log(log);
            }

            if (t.EdgesMatchingCount <= 2)
            {
                result *= t.id;
            }
        }    

        Debug.LogWarning(" Part 1 result is : " + result.ToString());
    }


    Dictionary<int, Tile> _tiles = new Dictionary<int, Tile>();
    List<KeyValuePair<Edge, Edge>> _matchings = new List<KeyValuePair<Edge, Edge>>();
    Dictionary<KeyValuePair<int, int>, KeyValuePair<Edge, bool>> _grid;
    Vector2 xBounds = Vector2.zero;
    Vector2 yBounds = Vector2.zero;
    const int tileSize = 10;

    void day_2()
    {
        long result = 0;

        #region INIT tiles
        _tiles = new Dictionary<int, Tile>();
        foreach (string inputTile in _input.Split(new string[] { "\n\n" }, System.StringSplitOptions.RemoveEmptyEntries))
        {
            int index = int.Parse(inputTile.Substring(5, 4)); // we could make it dynamic but dc
            List<string> content = new List<string>(inputTile.Split('\n'));
            content.RemoveAt(0);

            Tile myTile = new Tile(index, content);

            foreach (Edge edge in myTile.edgesMatch)
            {
                foreach (Tile t in _tiles.Values)
                {
                    Edge matching = t.edgesMatch.Find(x => x.value == edge.value);
                    if (matching != null)
                    {
                        _matchings.Add(new KeyValuePair<Edge, Edge>(edge, matching));
                        _matchings.Add(new KeyValuePair<Edge, Edge>(matching, edge));
                    }
                }
            }

            Dictionary<string, int> tmp = new Dictionary<string, int>();
            foreach (string edge in myTile.edges.Keys)
            {
                tmp.Add(edge, 0);
                foreach (Tile t in _tiles.Values)
                {
                    if (t.edges.ContainsKey(edge))
                    {
                        t.edges[edge] += 1;
                        tmp[edge] += 1;
                    }

                    if (t.reversedEdges.ContainsKey(edge))
                    {
                        t.reversedEdges[edge] += 1;
                        tmp[edge] += 1;
                    }
                }
            }
            myTile.edges = new Dictionary<string, int>(tmp);

            tmp = new Dictionary<string, int>();
            foreach (string edge in myTile.reversedEdges.Keys)
            {
                tmp.Add(edge, 0);
                foreach (Tile t in _tiles.Values)
                {
                    if (t.edges.ContainsKey(edge))
                    {
                        t.edges[edge] += 1;
                        tmp[edge] += 1;
                    }

                    if (t.reversedEdges.ContainsKey(edge))
                    {
                        t.reversedEdges[edge] += 1;
                        tmp[edge] += 1;
                    }
                }
            }
            myTile.reversedEdges = new Dictionary<string, int>(tmp);

            _tiles.Add(index, myTile);
        }
        #endregion

        _grid = new Dictionary<KeyValuePair<int, int>, KeyValuePair<Edge, bool>>();
        xBounds = Vector2.zero;
        yBounds = Vector2.zero;

        if (IsDebug)
        {
            string logM = "Matchings : " + _matchings.Count + System.Environment.NewLine;
            _matchings.ForEach(x => logM += x.Key.ToString() + " -- " + x.Value.ToString() + System.Environment.NewLine);
            Debug.Log(logM);
        }

        Tile tile0 = _tiles.ElementAt(0).Value;
        _grid.Add(new KeyValuePair<int, int>(0, 0), new KeyValuePair<Edge, bool>(tile0.edgesMatch[0], false));
        searchNeighbours(0, 0, EdgeSide.TOP);

        List<string> charGrid = new List<string>();
        for (int j = (int)yBounds.y; j >= (int)yBounds.x; j--)
        {
            List<string> chars = new List<string>();
            for (int t = 0; t < tileSize - 2; t++) { chars.Add(""); }

            for (int i = (int)xBounds.x; i <= (int)xBounds.y; i++)
            {
                if (_grid.ContainsKey(new KeyValuePair<int, int>(i, j)))
                {
                    Edge currEdge = _grid[new KeyValuePair<int, int>(i, j)].Key;
                    List<string> suffix = _tiles[currEdge.tileID].GetInnerContent(currEdge);
                    for (int cnt = 0; cnt < tileSize - 2; cnt++)
                    {
                        chars[cnt] += suffix[cnt];
                    }
                }
                else
                    Debug.LogWarning("No tile for coords " + i + "," + j);
            }

            charGrid.AddRange(chars);
        }

        foreach (string s in charGrid) { Debug.Log(s); }

        int rocksCount = 0;
        foreach (string s in charGrid) { rocksCount += s.Count(x => x == '#'); }
        //Debug.Log("Total of # : " + rocksCount);

        for (int i = 0; i < 4; i++)
        {
            if (IsDebug)
            {
                Debug.Log("\n GRID IS \n");
                foreach (string s in charGrid) { Debug.Log(s); }
            }

            int monsterCount = getSeaMonsterCount(charGrid);
            
            if (IsDebug)
                Debug.Log("Monsters Found : " + monsterCount);
            
            if (monsterCount > 0)
            {
                result = rocksCount - monsterCount * 15;
                break;
            }

            charGrid = rotateAnticlockwise(charGrid);
        }

        Debug.LogWarning(" Part 2 result is : " + result.ToString());
    }

    List<string> rotateAnticlockwise(List<string> grid)
    {
        if (grid == null || grid.Count == 0)
        {
            Debug.LogError("Grid malformed ... ");
            return new List<string>();
        }

        List<string> result = new List<string>();
        for (int i = 0; i < grid[0].Length; i++)
        {
            result.Add("");
            for (int j = 0; j < grid.Count; j++)
            {
                result[i] += grid[j][grid[j].Length - 1 - i];
            }

        }
        return result;
    }

    int getSeaMonsterCount(List<string> charGrid)
    {
        int result = 0;
        for (int line = 1; line < charGrid.Count - 1; line++)
        {
            for (int col = 0; col < charGrid[line].Length - 19; col++)
            {
                bool valid = charGrid[line][col] == '#'
                    && charGrid[line + 1][col + 1] == '#'
                    && charGrid[line + 1][col + 4] == '#'
                    && charGrid[line][col + 5] == '#'
                    && charGrid[line][col + 6] == '#'
                    && charGrid[line + 1][col + 7] == '#'
                    && charGrid[line + 1][col + 10] == '#'
                    && charGrid[line][col + 11] == '#'
                    && charGrid[line][col + 12] == '#'
                    && charGrid[line + 1][col + 13] == '#'
                    && charGrid[line + 1][col + 16] == '#'
                    && charGrid[line][col + 17] == '#'
                    && charGrid[line][col + 18] == '#'
                    && charGrid[line - 1][col + 18] == '#'
                    && charGrid[line][col + 19] == '#';

                if (valid)
                {
                    Debug.Log("Monster found at " + line + "," + col);
                    result++;
                }
            }
        }
        return result;
    }

    void searchNeighbours(int x, int y, EdgeSide sideOnTop, int offset = 0)
    {
        if (offset > _tiles.Count + 1)
            return;

        if (IsDebug)
            Debug.Log(Tools.writeOffset(offset) + "Pos " + x + ", " + y + "  (side on top is : " + sideOnTop.ToString() + ")");

        if (!_grid.ContainsKey(new KeyValuePair<int, int>(x, y)))
        {
            Debug.LogError(Tools.writeOffset(offset) + "Problem, _grid should contain tile at " + x + "," + y);
            return;
        }

        Tile currentTile = _tiles[_grid[new KeyValuePair<int, int>(x, y)].Key.tileID];
        bool isFlip = _grid[new KeyValuePair<int, int>(x, y)].Value;

        if (IsDebug)
            Debug.Log(Tools.writeOffset(offset) + " flip ? " + isFlip.ToString());

        EdgeSide currentSide = EdgeSide.TOP;
        int i = x;
        int j = y;

        //Looking for neighbour on TOP
        currentSide = EdgeSide.TOP;
        i = x;
        j = y + 1;
        computeNeighbour(i, j, currentSide, sideOnTop, currentTile, isFlip, offset);

        if (IsDebug)
            Debug.Log(Tools.writeOffset(offset) + "(memo) Pos " + x + ", " + y + "  (side on top is : " + sideOnTop.ToString() + ")");   // memo

        //Looking for neighbour on BOT
        currentSide = EdgeSide.BOT;
        i = x;
        j = y - 1;
        computeNeighbour(i, j, currentSide, sideOnTop, currentTile, isFlip, offset);

        if (IsDebug)
            Debug.Log(Tools.writeOffset(offset) + "(memo) Pos " + x + ", " + y + "  (side on top is : " + sideOnTop.ToString() + ")");   // memo

        //Looking for neighbour on LEFT
        currentSide = EdgeSide.LEFT;
        i = x - 1;
        j = y;
        computeNeighbour(i, j, currentSide, sideOnTop, currentTile, isFlip, offset);

        if (IsDebug)
            Debug.Log(Tools.writeOffset(offset) + "(memo) Pos " + x + ", " + y + "  (side on top is : " + sideOnTop.ToString() + ")");   // memo

        //Looking for neighbour on RIGHT
        currentSide = EdgeSide.RIGHT;
        i = x + 1;
        j = y;
        computeNeighbour(i, j, currentSide, sideOnTop, currentTile, isFlip, offset);
    }

    void computeNeighbour(int i, int j, EdgeSide currentSide, EdgeSide sideOnTop, Tile currentTile, bool isFlip, int offset = 0)
    {
        if (!_grid.ContainsKey(new KeyValuePair<int, int>(i, j)))
        {
            Debug.Log(Tools.writeOffset(offset) + "Computing " + currentSide);

            EdgeSide EdgeSide = getEdgeSide(currentSide, sideOnTop, isFlip);
            Edge edge = currentTile.edgesMatch.Find(e => e.flipped == isFlip && e.side == EdgeSide);

            if (IsDebug)
                Debug.Log(Tools.writeOffset(offset) + "\t" + currentSide + " edge is " + edge.ToString());

            Edge matchingEdge = _matchings.Find(e => e.Key == edge).Value;
            if (matchingEdge != null)
            {
                if (IsDebug)
                    Debug.Log(Tools.writeOffset(offset) + "\t\tFound matching edge for " + currentSide + " : " + matchingEdge.ToString());

                bool flip = (edge.flipped != matchingEdge.flipped) ? isFlip : !isFlip;


                Edge topMatchingEdge = getAbsoluteTopEdge(matchingEdge, currentSide, flip);
                if (IsDebug)
                    Debug.Log(Tools.writeOffset(offset) + "\t -> topMatchingEdge is  :" + topMatchingEdge.ToString());

                _grid.Add(new KeyValuePair<int, int>(i, j), new KeyValuePair<Edge, bool>(topMatchingEdge, flip));

                if (i < xBounds.x) { xBounds = new Vector2(i, xBounds.y); } 
                if (i > xBounds.y) { xBounds = new Vector2(xBounds.x, i); } 

                if (j < yBounds.x) { yBounds = new Vector2(j, yBounds.y); }
                if (j > yBounds.y) { yBounds = new Vector2(yBounds.x, j); } 

                Debug.Log(Tools.writeOffset(offset) + "\t bounds are " + xBounds.ToString() + " && " + yBounds.ToString());

                searchNeighbours(i, j, topMatchingEdge.side, offset + 1);
            }
            else if (IsDebug)
            {
                Debug.Log(Tools.writeOffset(offset) + "\t\tNo match found, we're on edge of puzzle");
            }
        }
        else if (IsDebug)
        {
            Debug.Log(Tools.writeOffset(offset) + currentSide + " is already in grid, skip ! ");
        }
    }

    public EdgeSide getEdgeSide(EdgeSide lookedSide, EdgeSide sideOnTop, bool isFlipped)
    {
        int res = 0;
        if (!isFlipped)
        {
            res = (int)lookedSide + (int)sideOnTop;
            res %= 4;
        }
        else
        {
            res = (int)lookedSide + 4 - (int)sideOnTop;
            res = (8 - res) % 4;
        }

        return (EdgeSide)res;
    }

    public EdgeSide getOppositeEdge(EdgeSide side)
    {
        int sideNumber = System.Enum.GetValues(typeof(EdgeSide)).Length;
        return (EdgeSide)(((int)side + 2) % sideNumber);
    }

    public Edge getAbsoluteTopEdge(Edge match, EdgeSide side, bool isFlipped)
    {
        int res = 0;
        if (isFlipped)
        {
            res = (int)match.side + (int)side + 2;
        }
        else
        {
            res = (int)match.side + (6 - (int)side);
        }
        res %= 4;

        Edge edge = _tiles[match.tileID].edgesMatch.Find(e => e.side == (EdgeSide)res && e.flipped == isFlipped);
        if (edge == null)
        {
            Debug.LogWarning("$$$$$ Edge not found ?? in getAbsoluteTopEdge " + match.ToString() + "  || " + side.ToString());
        }

        return edge;
    }

    public enum EdgeSide { TOP, RIGHT, BOT, LEFT }

    public class Edge
    {

        public string value;
        public int tileID;
        public EdgeSide side;
        public bool flipped;

        public Edge() { this.value = ""; this.tileID = -1; this.side = EdgeSide.TOP; this.flipped = false; }
        public Edge(string p_value, int p_id, EdgeSide p_side, bool p_flipped) { this.value = p_value; this.tileID = p_id; this.flipped = p_flipped; this.side = p_side; }

        public override string ToString() { return this.tileID + "/" + side.ToString() + "/" + (flipped ? "Flipped" : "Normal"); }
    }

    class Tile
    {
        public int id = -1;
        public List<string> tileRows = new List<string>();

        // Used in part 1, we should refacto part 1 to only use Edges
        public Dictionary<string, int> edges = new Dictionary<string, int>();
        public Dictionary<string, int> reversedEdges = new Dictionary<string, int>();

        // used in part 2
        public List<Edge> edgesMatch = new List<Edge>();

        public Tile(int p_id, List<string> p_content)
        {
            this.id = p_id;
            this.tileRows = new List<string>(p_content);

            edges = new Dictionary<string, int>();
            reversedEdges = new Dictionary<string, int>();
            if (p_content.Count > 0)
            {
                edges.Add(p_content[0], 0);
                edges.Add(p_content[p_content.Count - 1], 0);

                string left = "", right = "";
                for (int i = 0; i < p_content.Count; i++)
                {
                    left += p_content[i][0];
                    right += p_content[i][p_content[i].Length - 1];
                }
                edges.Add(left, 0);
                edges.Add(right, 0);

                reversedEdges.Add(Tools.Reverse(p_content[0]), 0);
                reversedEdges.Add(Tools.Reverse(p_content[p_content.Count - 1]), 0);
                reversedEdges.Add(Tools.Reverse(left), 0);
                reversedEdges.Add(Tools.Reverse(right), 0);

                edgesMatch.Add(new Edge(p_content[0], p_id, EdgeSide.TOP, false));
                edgesMatch.Add(new Edge(Tools.Reverse(left), p_id, EdgeSide.LEFT, false));
                edgesMatch.Add(new Edge(Tools.Reverse(p_content[p_content.Count - 1]), p_id, EdgeSide.BOT, false));
                edgesMatch.Add(new Edge(right, p_id, EdgeSide.RIGHT, false));

                edgesMatch.Add(new Edge(Tools.Reverse(p_content[0]), p_id, EdgeSide.TOP, true));
                edgesMatch.Add(new Edge(left, p_id, EdgeSide.LEFT, true));
                edgesMatch.Add(new Edge(p_content[p_content.Count - 1], p_id, EdgeSide.BOT, true));
                edgesMatch.Add(new Edge(Tools.Reverse(right), p_id, EdgeSide.RIGHT, true));
            }
        }

        public int EdgesMatchingCount { get { return System.Math.Max(edges.Where(x => x.Value > 0).Count(), reversedEdges.Where(x => x.Value > 0).Count()); } }

        public int InnerRockCounts()
        {
            int total = 0;
            for (int i = 1; i < tileRows.Count - 1; i++)
            {
                string row = tileRows[i];
                total += row.Count(x => x == '#');
                if (row[0] == '#')
                    total--;
                if (row[row.Length - 1] == '#')
                    total--;
            }
            Debug.Log("Tile " + this.id + ": # count is " + total);
            return total;
        }

        public List<string> GetInnerContent(Edge topEdge)
        {
            List<string> res = new List<string>();
            for (int i = 1; i < tileSize - 1; i++)
            {
                string line = "";
                for (int j = 1; j < tileSize - 1; j++)
                {

                    switch (topEdge.side)
                    {
                        case EdgeSide.TOP:
                            line += (topEdge.flipped ? tileRows[i][tileSize - j - 1] : tileRows[i][j]);
                            break;
                        case EdgeSide.BOT:
                            line += (topEdge.flipped ? tileRows[tileSize - i - 1][j] : tileRows[tileSize - i - 1][tileSize - j - 1]);
                            break;
                        case EdgeSide.LEFT:
                            line += (topEdge.flipped ? tileRows[j][i] : tileRows[tileSize - j - 1][i]);
                            break;
                        case EdgeSide.RIGHT:
                            line += (topEdge.flipped ? tileRows[tileSize - j - 1][tileSize - i - 1] : tileRows[j][tileSize - i - 1]);
                            break;
                    }
                }
                res.Add(line);
            }
            return res;
        }
    }
}
