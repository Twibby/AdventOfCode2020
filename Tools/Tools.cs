using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

// UnityWebRequest.Get example

// Access a website and use UnityWebRequest.Get to download a page.
// Also try to download a non-existing page. Display the error.

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
}