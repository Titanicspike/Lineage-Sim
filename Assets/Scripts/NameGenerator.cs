using UnityEngine;
using System.Collections.Generic;

public static class NameGenerator
{
    public static List<string> maleFirstNames = new List<string>(){};
    public static List<string> femaleFirstNames = new List<string>(){};
    public static List<string> surnames = new List<string>(){};
    private static bool initialized = false;

    public static void Initialize()
    {
        if (initialized) return;

        maleFirstNames = LoadNames("Names/Male Names");
        femaleFirstNames = LoadNames("Names/Female Names");
        surnames = LoadNames("Names/Surnames");

        initialized = true;
    }

    private static List<string> LoadNames(string resourcePath)
    {
        TextAsset asset = Resources.Load<TextAsset>(resourcePath);
        if (asset == null)
        {
            Debug.LogError($"Could not find name file at Resources/{resourcePath}.txt");
            return new List<string>();
        }

        List<string> result = new List<string>();
        string[] lines = asset.text.Split('\n');
        foreach (string line in lines)
        {
            string trimmed = line.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                result.Add(trimmed);
            }
        }
        return result;
    }
    public static string GenerateFirstName(Person person)
    {
        if(person.gender == Person.Gender.Male)
        {
            return maleFirstNames[Random.Range(0, maleFirstNames.Count)];
        }
        else
        {
            return femaleFirstNames[Random.Range(0, femaleFirstNames.Count)];
        }
    }
    public static string GenerateFirstName(Person.Gender gender)
    {
        if(gender == Person.Gender.Male)
        {
            return maleFirstNames[Random.Range(0, maleFirstNames.Count)];
        }
        else
        {
            return femaleFirstNames[Random.Range(0, femaleFirstNames.Count)];
        }
    }
    public static string GenerateLastName()
    {
        return surnames[Random.Range(0, surnames.Count)];
    }
}
