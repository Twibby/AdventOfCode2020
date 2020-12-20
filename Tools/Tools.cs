using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;


public class Tools : MonoBehaviour
{
    private static Tools _instance;
    public static Tools Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject().AddComponent<Tools>();
            }
            return _instance;
        }
    }

    private string _input = "";
    public string Input { get { return _input; } }

    public IEnumerator GetInput(int day)
    {
        yield return GetInput(day.ToString());
    }

    public IEnumerator GetInput(string day)
    {
        _input = "";
        string uri = "https://adventofcode.com/2020/day/" + day.ToString() + "/input";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SetRequestHeader("Cookie", "session=" + EasyAccessValues.CookieSession);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                _input = webRequest.downloadHandler.text.TrimEnd('\n');
                
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }

    public IEnumerator GetTestInput(string uri)
    {
        _input = "";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError)
                Debug.LogError("Test input : Error: " + webRequest.error);
            else
            {
                _input = webRequest.downloadHandler.text.TrimEnd('\n');
                Debug.Log("Test input received: " + webRequest.downloadHandler.text);
            }
        }
    }

    public IEnumerator GetLeaderoard()
    {
        //var textfile = Resources.Load<TextAsset>("lb");
        //yield return new WaitForEndOfFrame();
        //_input = textfile.text;
        //Debug.LogWarning("input : " + _input);
        //yield break;


        _input = "";
        string uri = "https://adventofcode.com/2020/leaderboard/private/view/" + EasyAccessValues.LeaderboardId + ".json";


        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SetRequestHeader("Cookie", "session=" + EasyAccessValues.CookieSession);

            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                _input = webRequest.downloadHandler.text.TrimEnd('\n');

                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
            }
        }
    }

    public static DateTime ConvertFromUnixTimestamp(double timestamp)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return origin.AddSeconds(timestamp).AddHours(1) ;
    }

    public static long ConvertToUnixTimestamp(DateTime date)
    {
        DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        TimeSpan diff = date.ToUniversalTime() - origin;
        return (long)Math.Floor(diff.TotalSeconds);
    }

    public static string writeOffset(int offset)
    {
        string log = "";
        for (int off = 0; off < offset; off++) { log += "|."; }
        return log;
    }

    public static string Reverse(string s)
    {
        char[] charArray = s.ToCharArray();
        System.Array.Reverse(charArray);
        return new string(charArray);
    }

}