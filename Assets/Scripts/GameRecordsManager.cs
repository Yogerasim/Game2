using System.Collections.Generic;
using UnityEngine;

public static class GameRecordsManager
{
    private const string RecordsKey = "GameRecords";

    public static void SaveRecord(int score)
    {
        List<int> records = LoadRecords();
        records.Add(score);
        records.Sort((a, b) => b.CompareTo(a)); // Сортировка по убыванию

        PlayerPrefs.SetString(RecordsKey, string.Join(",", records));
        PlayerPrefs.Save();
    }

    public static List<int> LoadRecords()
    {
        string recordsString = PlayerPrefs.GetString(RecordsKey, "");
        if (string.IsNullOrEmpty(recordsString)) return new List<int>();

        List<int> records = new List<int>();
        foreach (string score in recordsString.Split(','))
        {
            if (int.TryParse(score, out int parsedScore))
            {
                records.Add(parsedScore);
            }
        }

        return records;
    }

    public static int GetHighestScore()
    {
        List<int> records = LoadRecords();
        return records.Count > 0 ? records[0] : 0;
    }
    
    public static void ClearRecords()
    {
        PlayerPrefs.DeleteKey(RecordsKey); // Удаляем ключ с рекордами
        PlayerPrefs.Save();
    }
}
