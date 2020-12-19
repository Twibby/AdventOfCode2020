using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft;
using System.IO;

public class LeaderboardParser : MonoBehaviour
{
    [Serializable]
    public class DayCompletion
    {
        //public int day;
        public long part1;
        public long part2;

        public void ReadFromXml(XmlNode node)
        {
            this.part1 = 0;
            this.part2 = 0;

            if (node.SelectSingleNode("_x0031_") != null
                && node.SelectSingleNode("_x0031_").SelectSingleNode("get_star_ts") != null)
                this.part1 = long.Parse(node.SelectSingleNode("_x0031_").SelectSingleNode("get_star_ts").InnerText);

            if (node.SelectSingleNode("_x0032_") != null
                && node.SelectSingleNode("_x0032_").SelectSingleNode("get_star_ts") != null)
                this.part2 = long.Parse(node.SelectSingleNode("_x0032_").SelectSingleNode("get_star_ts").InnerText);
        }
    }

    [Serializable]
    public class Member
    {
        public int id;
        public string name;
        //public long last_star_ts; //don't care
        public int stars;
        public int local_score;
        public int global_score;
        public Dictionary<int, DayCompletion> days;

        public void ReadFromXml(XmlNode node)
        {
            this.id = int.Parse(node.SelectSingleNode("id").InnerText);
            this.name = node.SelectSingleNode("name").InnerText;
            if (name == null || name == string.Empty)
                name = "User " + id;

            this.stars = int.Parse(node.SelectSingleNode("stars").InnerText);
            this.local_score = int.Parse(node.SelectSingleNode("global_score").InnerText);
            this.global_score = int.Parse(node.SelectSingleNode("local_score").InnerText);

            this.days = new Dictionary<int, DayCompletion>();
            foreach (XmlNode dayNode in node.SelectSingleNode("completion_day_level").ChildNodes)
            {
                DayCompletion dc = new DayCompletion();
                dc.ReadFromXml(dayNode);
                days.Add(int.Parse(dayNode.Name.Substring(5).Replace("_", string.Empty)), dc);
            }
        }

        public long GetTimeOfPart(int part)
        {
            if (days.ContainsKey((part + 1) / 2))
            {
                if (part % 2 == 1)
                    return days[(part + 1) / 2].part1;
                else
                    return days[(part + 1) / 2].part2;
            }
            return 0;
        }
    }

    [Serializable]
    public class Leaderboard
    {
        public int year;
        public List<Member> members;
        public int id;

        //public static Leaderboard CreateFromJSON(string jsonString)
        //{
        //    return JsonUtility.FromJson<Leaderboard>(jsonString);
        //}

        public Leaderboard()
        {
            this.year = 2020;
            this.members = new List<Member>();
            this.id = 0;
        }

        public void ReadFromXml(XmlNode node)
        {
            if (node.SelectSingleNode("owner_id") != null)
                this.id = int.Parse(node.SelectSingleNode("owner_id").InnerText);

            if (node.SelectSingleNode("event") != null)
                this.year = int.Parse(node.SelectSingleNode("event").InnerText);

            if (node.SelectSingleNode("members") != null)
            {
                foreach (XmlNode memberNode in node.SelectSingleNode("members").ChildNodes)
                {
                    Member m = new Member();
                    m.ReadFromXml(memberNode);
                    this.members.Add(m);
                }
            }
        }

        public Dictionary<int, List<KeyValuePair<string, long>>> GetRankingsByStar()
        {
            Dictionary<int, List<KeyValuePair<string, long>>> result = new Dictionary<int, List<KeyValuePair<string, long>>>();

            for (int day = 1; day < 25; day++)
            {
                List<KeyValuePair<string, long>> dayResultP1 = new List<KeyValuePair<string, long>>();
                List<KeyValuePair<string, long>> dayResultP2 = new List<KeyValuePair<string, long>>();

                // part 1
                foreach (Member m in members)
                {
                    if (m.days.ContainsKey(day))
                    {
                        if (m.days[day].part1 != 0)
                            dayResultP1.Add(new KeyValuePair<string, long>(m.name, m.days[day].part1));

                        if (m.days[day].part2 != 0)
                            dayResultP2.Add(new KeyValuePair<string, long>(m.name, m.days[day].part2));
                    }
                }

                if (dayResultP1.Count > 0)
                {
                    dayResultP1.Sort(delegate (KeyValuePair<string, long> score1, KeyValuePair<string, long> score2)
                        {
                            return score1.Value.CompareTo(score2.Value);
                        });

                    dayResultP2.Sort(delegate (KeyValuePair<string, long> score1, KeyValuePair<string, long> score2)
                    {
                        return score1.Value.CompareTo(score2.Value);
                    });

                    result.Add(2 * day - 1, dayResultP1);
                    result.Add(2 * day, dayResultP2);
                }
                else
                    break;
            }

            return result;
        }

        public Dictionary<int, List<KeyValuePair<string, int>>> GetLeaderboardByDay()
        {
            Dictionary<int, List<KeyValuePair<string, int>>> leaderboardByDay = new Dictionary<int, List<KeyValuePair<string, int>>>();

            Dictionary<int, List<KeyValuePair<string, long>>> ranks = this.GetRankingsByStar();
            int membersCount = this.members.Count;

            for (int i = 1; i <= 25; i++)
            {
                Dictionary<string, int> scores = new Dictionary<string, int>();
                foreach (var m in this.members) { scores.Add(m.name, 0); }

                if (!ranks.ContainsKey(i))
                    break;

                string line = "Dec " + i.ToString() + ",";

                long dayTS = Tools.ConvertToUnixTimestamp(new DateTime(this.year, 12, i + 1, 5, 59, 59));

                // WARNING :
                // As Day 1 doesn't count because of outage, only start to part/star 3 if you want same thing as offical leaderboard!
                for (int part = 1; part <= i * 2; part++)
                {
                    if (!ranks.ContainsKey(part))
                        continue;

                    for (int pos = 0; pos < ranks[part].Count; pos++)
                    {
                        if (ranks[part][pos].Value < dayTS)
                        {
                            scores[ranks[part][pos].Key] += membersCount - pos;
                        }
                    }
                }

                string log = "After day " + i + ", scores are : ";
                foreach (var score in scores) { log += score.Key + " -> " + score.Value + " / "; }
                Debug.LogWarning(log);

                List<KeyValuePair<string, int>> sortedScores = new List<KeyValuePair<string, int>>();
                foreach (var score in scores)
                {
                    sortedScores.Add(new KeyValuePair<string, int>(score.Key, score.Value));
                }
                sortedScores.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));
                leaderboardByDay.Add(i, sortedScores);
            }

            return leaderboardByDay;
        }
    }



    private Leaderboard _myLeaderboard;

    void Start()
    {
        StartCoroutine(coDay());
    }

    IEnumerator coDay()
    {
        yield return StartCoroutine(Tools.Instance.GetLeaderoard());

        #region init leaderboard
        JsonTextReader reader = new JsonTextReader(new System.IO.StringReader(Tools.Instance.Input));
        XNode node = JsonConvert.DeserializeXNode(Tools.Instance.Input, "ROOT");
        Debug.LogWarning(node.ToString());

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(JsonConvert.DeserializeXNode(Tools.Instance.Input, "ROOT").ToString());
        XmlNode rootNode = doc.SelectSingleNode("ROOT");

        Debug.LogWarning(rootNode.InnerXml);

        _myLeaderboard = new Leaderboard();
        _myLeaderboard.ReadFromXml(rootNode);
        // END OF INITIALIZATION
        #endregion

        ExportCSVDatasByStars();
    }

    public void ExportCSVDatasByStars()
    {
        Dictionary<int, List<KeyValuePair<string, long>>> ranks = _myLeaderboard.GetRankingsByStar();

        string file = "External/Export_ByStars.csv";

        using (FileStream fs = new FileStream(file, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs, System.Text.Encoding.Default))
            {

                Dictionary<string, int> scores = new Dictionary<string, int>();

                // Write First line  && set scores
                string firstLine = "NAME,";
                foreach (var m in _myLeaderboard.members)
                {
                    firstLine += m.name + ",";
                    scores.Add(m.name, 0);
                }
                firstLine.TrimEnd(new char[] { ',' });
                writer.WriteLine(firstLine);

                int membersCount = _myLeaderboard.members.Count;

                for (int i=1; i<=25; i++)
                {
                    scores = new Dictionary<string, int>();
                    foreach (var m in _myLeaderboard.members) { scores.Add(m.name, 0); }

                    if (!ranks.ContainsKey(i))
                        break;

                    string line = "Dec " + i.ToString() + ",";

                    long dayTS = Tools.ConvertToUnixTimestamp(new DateTime(_myLeaderboard.year, 12, i+1, 5, 59, 59));

                    // WARNING :
                    // As Day 1 doesn't count because of outage, only start to part/star 3 if you want same thing as offical leaderboard!
                    for (int part=1; part <= i*2; part++)
                    {
                        if (!ranks.ContainsKey(part))
                            continue;

                        for (int pos = 0; pos < ranks[part].Count; pos++)
                        {
                            if (ranks[part][pos].Value < dayTS)
                            {
                                scores[ranks[part][pos].Key] += membersCount - pos;
                            }
                        }
                    }

                    string log = "After day " + i + ", scores are : ";
                    foreach (var score in scores)
                    {
                        log += score.Key + " -> " + score.Value + " / ";
                        line += score.Value + ",";
                    }
                    Debug.LogWarning(log);
                    line.TrimEnd(new char[] { ',' });
                    writer.WriteLine(line);
                }
            }
        }        
    }
}


