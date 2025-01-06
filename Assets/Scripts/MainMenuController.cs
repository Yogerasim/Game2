using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject recordsPanel;
    [SerializeField] private Transform recordsContent;
    [SerializeField] private GameObject recordPrefab;
    [SerializeField] private Text highestScoreText;

    private void Start()
    {
        InitializePanels();
        GameRecordsManager.DebugRecords(); // Отладка записей
    }

    private void InitializePanels()
    {
        mainMenuPanel.SetActive(true);
        recordsPanel.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ShowRecords()
    {
        mainMenuPanel.SetActive(false);
        recordsPanel.SetActive(true);
        DisplayRecords();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        recordsPanel.SetActive(false);
    }

    public void ResetRecords()
    {
        GameRecordsManager.ClearRecords(); // Очистка всех рекордов
        DisplayRecords(); // Обновление интерфейса
    }

    private void DisplayRecords()
    {
        // Удаляем старые записи
        foreach (Transform child in recordsContent)
        {
            Destroy(child.gameObject);
        }

        // Загружаем рекорды
        List<int> records = GameRecordsManager.LoadRecords();

        // Обновляем текст с лучшим счётом за все игры
        highestScoreText.text = records.Count > 0
            ? $"Лучший счёт за все игры: {GameRecordsManager.GetHighestScore()}"
            : "Лучший счёт за все игры: 0";

        // Формируем список записей с корректной нумерацией
        for (int i = 0; i < records.Count; i++)
        {
            int gameNumber = i + 1; // Номер игры (от первой до последней)
            GameObject recordEntry = Instantiate(recordPrefab, recordsContent);

            // Вставляем элемент в начало списка
            recordEntry.transform.SetSiblingIndex(0);

            recordEntry.GetComponent<Text>().text = $"Лучший счёт за игру №{gameNumber}: {records[i]}";
        }
    }
}