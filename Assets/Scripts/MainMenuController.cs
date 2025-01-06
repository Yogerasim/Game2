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
        foreach (Transform child in recordsContent)
        {
            Destroy(child.gameObject);
        }

        List<int> records = GameRecordsManager.LoadRecords();

        if (records.Count > 0)
        {
            highestScoreText.text = $"Лучший счёт за все игры: {GameRecordsManager.GetHighestScore()}";
        }
        else
        {
            highestScoreText.text = "Лучший счёт за все игры: 0";
        }

        records.Reverse();

        for (int i = 0; i < records.Count; i++)
        {
            GameObject recordEntry = Instantiate(recordPrefab, recordsContent);
            recordEntry.GetComponent<Text>().text = $"Лучший счёт за игру №{i + 1}: {records[i]}";
        }
    }
}