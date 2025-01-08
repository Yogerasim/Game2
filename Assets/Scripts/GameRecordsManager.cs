using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Управляет сохранением, загрузкой и удалением рекордов в игре.
/// </summary>
public static class GameRecordsManager
{
    /// <summary>
    /// Ключ для хранения рекордов в PlayerPrefs.
    /// </summary>
    private const string RecordsKey = "GameRecords";

    /// <summary>
    /// Сохраняет новый рекорд в список рекордов.
    /// </summary>
    /// <param name="score">Очки, которые нужно сохранить как рекорд.</param>
    public static void SaveRecord(int score)
    {
        List<int> records = LoadRecords();
        records.Add(score); // Добавляем новый счёт в список

        Debug.Log($"Saving record: {score}"); // Логируем процесс сохранения

        // Сохраняем список рекордов в PlayerPrefs
        PlayerPrefs.SetString(RecordsKey, string.Join(",", records));
        PlayerPrefs.Save();

        Debug.Log($"Current records saved: {string.Join(",", records)}"); // Логируем сохранённые данные
    }

    /// <summary>
    /// Загружает список рекордов из PlayerPrefs.
    /// </summary>
    /// <returns>Список всех рекордов.</returns>
    public static List<int> LoadRecords()
    {
        string recordsString = PlayerPrefs.GetString(RecordsKey, "");
        Debug.Log($"Loaded records string: {recordsString}"); // Логируем загруженные данные

        if (string.IsNullOrEmpty(recordsString)) return new List<int>();

        List<int> records = new List<int>();
        foreach (string score in recordsString.Split(','))
        {
            if (int.TryParse(score, out int parsedScore))
            {
                records.Add(parsedScore); // Добавляем валидный счёт в список
            }
        }

        Debug.Log($"Loaded records: {string.Join(",", records)}"); // Логируем итоговый список
        return records;
    }

    /// <summary>
    /// Получает наивысший счёт из списка рекордов.
    /// </summary>
    /// <returns>Максимальный рекорд, либо 0, если список пуст.</returns>
    public static int GetHighestScore()
    {
        List<int> records = LoadRecords();
        return records.Count > 0 ? Mathf.Max(records.ToArray()) : 0;
    }

    /// <summary>
    /// Удаляет все сохранённые рекорды.
    /// </summary>
    public static void ClearRecords()
    {
        PlayerPrefs.DeleteKey(RecordsKey); // Удаляем ключ рекордов
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Логирует текущие рекорды, хранящиеся в PlayerPrefs.
    /// </summary>
    public static void DebugRecords()
    {
        string recordsString = PlayerPrefs.GetString(RecordsKey, "No Records Found");
        Debug.Log($"Current records in PlayerPrefs: {recordsString}");
    }
}
