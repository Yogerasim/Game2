using System.Collections.Generic;
using UnityEngine;

public static class GameRecordsManager
{
    private const string RecordsKey = "GameRecords";

    public static void SaveRecord(int score)
    {
        List<int> records = LoadRecords();
        records.Add(score); // Просто добавляем новый счёт в конец списка

        Debug.Log($"Saving record: {score}"); // Логируем добавляемый счёт

        // Сохраняем список обратно в PlayerPrefs
        PlayerPrefs.SetString(RecordsKey, string.Join(",", records));
        PlayerPrefs.Save();

        Debug.Log($"Current records saved: {string.Join(",", records)}");
    }

    public static List<int> LoadRecords()
    {
        string recordsString = PlayerPrefs.GetString(RecordsKey, "");
        Debug.Log($"Loaded records string: {recordsString}"); // Логируем содержимое PlayerPrefs

        if (string.IsNullOrEmpty(recordsString)) return new List<int>();

        List<int> records = new List<int>();
        foreach (string score in recordsString.Split(','))
        {
            if (int.TryParse(score, out int parsedScore))
            {
                records.Add(parsedScore);
            }
        }

        Debug.Log($"Loaded records: {string.Join(",", records)}"); // Логируем загруженные записи
        return records;
    }

    public static int GetHighestScore()
    {
        List<int> records = LoadRecords();
        return records.Count > 0 ? Mathf.Max(records.ToArray()) : 0;
    }

    public static void ClearRecords()
    {
        PlayerPrefs.DeleteKey(RecordsKey); // Удаляем ключ с рекордами
        PlayerPrefs.Save();
    }

    public static void DebugRecords()
    {
        string recordsString = PlayerPrefs.GetString(RecordsKey, "No Records Found");
        Debug.Log($"Current records in PlayerPrefs: {recordsString}");
    }
}
