using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day21 : MonoBehaviour
{
    public bool IsDebug = false;
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
        long result = 0;


        List<string> allIngredients = new List<string>();
        Dictionary<string, List<string>> possibleIngredientsForAlergen = new Dictionary<string, List<string>>();
        foreach (string recipe in _input.Split('\n'))
        {
            string ingredients = recipe;
            int alergenIndex = recipe.IndexOf('(');
            List<string> alergens = new List<string>();
            if (alergenIndex > 0)
            {
                ingredients = recipe.Substring(0, alergenIndex - 1).Trim();
                alergens = new List<string>(recipe.Substring(alergenIndex + 10).TrimEnd(')').Split(new string[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries));
                foreach (string alergen in alergens)
                {
                    if (!possibleIngredientsForAlergen.ContainsKey(alergen))
                        possibleIngredientsForAlergen.Add(alergen, new List<string>(ingredients.Split(' ')));
                    else
                        possibleIngredientsForAlergen[alergen] = possibleIngredientsForAlergen[alergen].Intersect(ingredients.Split(' ')).ToList();
                }
            }
            allIngredients.AddRange(ingredients.Split(' ')); 
        }

        foreach (var alergen in possibleIngredientsForAlergen)
        {
            alergen.Value.ForEach(x => allIngredients.RemoveAll(y => x == y));
        }
        result = allIngredients.Count;

        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 1 result is : " + result.ToString());
    }


    void day_2()
    {
        string result = "";

        List<string> allIngredients = new List<string>();
        Dictionary<string, List<string>> possibleIngredientsForAlergen = new Dictionary<string, List<string>>();
        foreach (string recipe in Tools.Instance.Input.Split('\n'))
        {
            string ingredients = recipe;
            int alergenIndex = recipe.IndexOf('(');
            List<string> alergens = new List<string>();
            if (alergenIndex > 0)
            {
                ingredients = recipe.Substring(0, alergenIndex - 1).Trim();
                alergens = new List<string>(recipe.Substring(alergenIndex + 10).TrimEnd(')').Split(new string[] { ", " }, System.StringSplitOptions.RemoveEmptyEntries));
                foreach (string alergen in alergens)
                {
                    if (!possibleIngredientsForAlergen.ContainsKey(alergen))
                        possibleIngredientsForAlergen.Add(alergen, new List<string>(ingredients.Split(' ')));
                    else
                        possibleIngredientsForAlergen[alergen] = possibleIngredientsForAlergen[alergen].Intersect(ingredients.Split(' ')).ToList();
                }
            }
            allIngredients.AddRange(ingredients.Split(' '));
        }

        SortedDictionary<string, string> uniqueAllergenIngredient = new SortedDictionary<string, string>();
        int safetyCount = possibleIngredientsForAlergen.Count;
        do
        {
            foreach (var alergen in possibleIngredientsForAlergen)
            {
                if (uniqueAllergenIngredient.ContainsKey(alergen.Key))
                    continue;

                if (alergen.Value.Count == 1)
                    uniqueAllergenIngredient.Add(alergen.Key, alergen.Value[0]);
                else
                {
                    List<string> ingredients = new List<string>(alergen.Value);
                    foreach (string ingredient in alergen.Value)
                    {
                        if (uniqueAllergenIngredient.ContainsValue(ingredient))
                        {
                            ingredients.Remove(ingredient);
                        }
                    }
                    if (ingredients.Count == 1)
                        uniqueAllergenIngredient.Add(alergen.Key, ingredients[0]);
                }
            }
            safetyCount--;
        } while (uniqueAllergenIngredient.Count < possibleIngredientsForAlergen.Count && safetyCount >= 0);



        foreach (var tmp in uniqueAllergenIngredient)
        {
            result += tmp.Value + ",";
        }
        result = result.TrimEnd(',');
        Debug.LogWarning("[" + this.GetType().ToString() + "] Part 2 result is : " + result.ToString());
    }
}
