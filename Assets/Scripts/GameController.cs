using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private GameModel model;
    private GameView view;

    [SerializeField] private Transform grid; // Сетка игрового поля
    [SerializeField] private Sprite[] ballSprites; // Спрайты шариков
    [SerializeField] private Text scoreText; // Текст для отображения счёта
    [SerializeField] private GameObject gameOverPopup; // Попап окончания игры
    [SerializeField] private Text currentGameScoreText; // Текст текущего рекорда в попапе

    private int currentGameHighScore = 0;
    private bool isGameOver = false; // Флаг завершения игры

    void Start()
    {
        // Инициализация модели и представления
        model = new GameModel();
        view = FindObjectOfType<GameView>();

        // Установка размеров сетки
        int columns = 12;
        int rows = Mathf.CeilToInt((float)grid.childCount / columns);
        model.SetGridSize(columns, rows);

        // Подписка на события
        model.OnCellUpdated += view.UpdateCell;
        model.OnScoreUpdated += UpdateScore;
        model.OnGameReset += ResetView;
        model.OnGameOver += EndGame; // Подписываемся на событие завершения игры

        // Инициализация представления
        view.Initialize(columns, rows, GameModel.BALLS, grid);
        view.InitializeSprites(ballSprites);
        view.InitializeButtons(this);

        // Запуск игры
        model.StartGame();
        gameOverPopup.SetActive(false); // Убедимся, что попап скрыт в начале
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Возвращаемся в главное меню
    }

    public void OnCellClicked(int buttonIndex)
    {
        int x = buttonIndex % GameModel.SIZE_X;
        int y = buttonIndex / GameModel.SIZE_X;
        model.ClickCell(x, y);
    }

    private void UpdateScore(int newScore)
    {
        if (isGameOver) return; // Если игра уже завершена, ничего не делаем

        // Обновляем текст счёта перед проверкой на завершение игры
        scoreText.text = $"Счёт: {newScore}";
        Debug.Log($"Score updated to: {newScore}");

        // Обновляем текущий рекорд, если это необходимо
        if (newScore > currentGameHighScore)
        {
            currentGameHighScore = newScore;
        }
    }

    private IEnumerator ShowGameOverPopup()
    {
        yield return new WaitForEndOfFrame(); // Ждём один кадр перед активацией

        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(true); // Активируем попап

            Animator animator = gameOverPopup.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Show"); // Устанавливаем триггер для запуска анимации
                Debug.Log("Animation trigger 'Show' set.");
            }
            else
            {
                Debug.LogWarning("Animator not found on Game Over Popup.");
            }

            if (currentGameScoreText != null)
            {
                currentGameScoreText.text = $"Ваш рекорд: {currentGameHighScore}";
            }
            else
            {
                Debug.LogError("Current Game Score Text is not assigned!");
            }
        }
        else
        {
            Debug.LogError("Game Over Popup is not assigned!");
        }

        Debug.Log("Game Over Popup activation completed.");
    }

    private void EndGame()
    {
        if (isGameOver) return;

        isGameOver = true;

        Debug.Log("Game Over! Starting popup sequence...");
    
        // Сохраняем текущий рекорд
        GameRecordsManager.SaveRecord(currentGameHighScore);

        StartCoroutine(ShowGameOverPopup());
    }

    private void ResetView()
    {
        Debug.Log("Game reset!");
        Time.timeScale = 1; // Сбрасываем масштаб времени
    }
}