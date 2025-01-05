using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel; // Панель главного меню
    [SerializeField] private GameObject recordsPanel; // Панель рекордов

    private void Start()
    {
        InitializePanels(); // Инициализация активных панелей
    }

    private void InitializePanels()
    {
        mainMenuPanel.SetActive(true); // Включаем главное меню
        recordsPanel.SetActive(false); // Выключаем панель рекордов
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game"); // Загрузка игровой сцены
    }

    public void ShowRecords()
    {
        mainMenuPanel.SetActive(false); // Скрываем главное меню
        recordsPanel.SetActive(true); // Показываем панель рекордов
    }

    public void ExitGame()
    {
        Application.Quit(); // Завершаем приложение
    }

    public void BackToMainMenu()
    {
        mainMenuPanel.SetActive(true); // Показываем главное меню
        recordsPanel.SetActive(false); // Скрываем панель рекордов
    }
}