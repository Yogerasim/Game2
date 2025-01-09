using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Контроллер главного меню, управляющий отображением и взаимодействием с панелями меню и рекордов.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    /// <summary>
    /// Панель главного меню, отображающая основные элементы интерфейса: кнопки запуска игры, просмотра рекордов и выхода.
    /// </summary>
    [SerializeField]
    private GameObject mainMenuPanel;

    /// <summary>
    /// Панель с рекордами, отображающая список всех сохранённых результатов игр.
    /// </summary>
    [SerializeField]
    private GameObject recordsPanel;

    /// <summary>
    /// Контейнер для отображения списка рекордов. Элементы списка добавляются внутрь этого объекта.
    /// </summary>
    [SerializeField]
    private Transform recordsContent;

    /// <summary>
    /// Префаб для отображения одной записи рекорда. Каждая запись создаётся как объект этого типа.
    /// </summary>
    [SerializeField]
    private GameObject recordPrefab;

    /// <summary>
    /// Текстовый элемент, отображающий лучший счёт за все игры, сохранённый в PlayerPrefs.
    /// </summary>
    [SerializeField]
    private Text highestScoreText;

    /// <summary>
    /// Инициализация контроллера при запуске сцены.
    /// </summary>
    private void Start()
    {
        InitializePanels(); // Инициализация состояния панелей
        GameRecordsManager.DebugRecords(); // Отладка записей для проверки сохранённых данных
    }

    /// <summary>
    /// Устанавливает начальное состояние панелей (главное меню активно, панель рекордов скрыта).
    /// </summary>
    private void InitializePanels()
    {
        mainMenuPanel.SetActive(true);
        recordsPanel.SetActive(false);
    }

    /// <summary>
    /// Запускает игровую сцену.
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("Game"); // Переход к игровой сцене
    }

    /// <summary>
    /// Показывает панель с рекордами, скрывая главное меню.
    /// </summary>
    public void ShowRecords()
    {
        mainMenuPanel.SetActive(false);
        recordsPanel.SetActive(true);
        DisplayRecords(); // Обновляет отображение списка рекордов
    }

    /// <summary>
    /// Закрывает приложение.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Возвращает пользователя в главное меню, скрывая панель рекордов.
    /// </summary>
    public void BackToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        recordsPanel.SetActive(false);
    }

    /// <summary>
    /// Сбрасывает все сохранённые рекорды и обновляет интерфейс.
    /// </summary>
    public void ResetRecords()
    {
        GameRecordsManager.ClearRecords(); // Очистка всех записей рекордов
        DisplayRecords(); // Обновление интерфейса после очистки
    }

    /// <summary>
    /// Отображает список рекордов и лучший счёт.
    /// </summary>
    private void DisplayRecords()
    {
        foreach (Transform child in recordsContent)
        {
            Destroy(child.gameObject);
        }

        List<int> records = GameRecordsManager.LoadRecords();

        highestScoreText.text = records.Count > 0
            ? $"Лучший счёт за все игры: {GameRecordsManager.GetHighestScore()}"
            : "Лучший счёт за все игры: 0";

        for (int i = 0; i < records.Count; i++)
        {
            GameObject recordEntry = Instantiate(recordPrefab, recordsContent);
            recordEntry.transform.SetSiblingIndex(0);

            recordEntry.GetComponent<Text>().text = $"Лучший счёт за игру №{i + 1}: {records[i]}";
        }
    }
}