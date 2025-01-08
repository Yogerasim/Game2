using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Контроллер игры, управляющий игровой логикой и взаимодействиет между моделью и представлением.
/// </summary>
public class GameController : MonoBehaviour
{
    private GameModel model; // Модель игры
    private GameView view; // Представление игры

    [SerializeField]
    private Transform grid; // Сетка игрового поля

    [SerializeField]
    private Sprite[] ballSprites; // Спрайты шариков

    [SerializeField]
    private Text scoreText; // Текст для отображения текущего счёта

    [SerializeField]
    private GameObject gameOverPopup; // Попап окончания игры

    [SerializeField]
    private Text currentGameScoreText; // Текст текущего рекорда в попапе

    private int currentGameHighScore = 0; // Лучший счёт за текущую игру
    private bool isGameOver = false; // Флаг завершения игры

    /// <summary>
    /// Инициализация контроллера при старте сцены.
    /// </summary>
    void Start()
    {
        // Инициализация модели и представления
        model = new GameModel();
        view = FindObjectOfType<GameView>();

        // Установка размеров сетки
        int columns = 12;
        int rows = Mathf.CeilToInt((float)grid.childCount / columns);
        model.SetGridSize(columns, rows);

        // Подписка на события модели
        model.OnCellUpdated += view.UpdateCell;
        model.OnScoreUpdated += UpdateScore;
        model.OnGameReset += ResetView;
        model.OnGameOver += EndGame; // Подписка на событие завершения игры

        // Инициализация представления
        view.Initialize(columns, rows, GameModel.BALLS, grid);
        view.InitializeSprites(ballSprites);
        view.InitializeButtons(this);

        // Запуск игры
        model.StartGame();
        gameOverPopup.SetActive(false); // Убедимся, что попап скрыт в начале
    }

    /// <summary>
    /// Возвращает игрока в главное меню.
    /// </summary>
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Загрузка сцены главного меню
    }

    /// <summary>
    /// Обрабатывает нажатие на ячейку игрового поля.
    /// </summary>
    /// <param name="buttonIndex">Индекс кнопки, на которую нажали.</param>
    public void OnCellClicked(int buttonIndex)
    {
        int x = buttonIndex % GameModel.SIZE_X;
        int y = buttonIndex / GameModel.SIZE_X;
        model.ClickCell(x, y);
    }

    /// <summary>
    /// Обновляет текущий счёт и проверяет, достигнут ли конец игры.
    /// </summary>
    /// <param name="newScore">Новый текущий счёт.</param>
    private void UpdateScore(int newScore)
    {
        if (isGameOver) return; // Если игра завершена, пропускаем

        // Обновляем текст счёта
        scoreText.text = $"Счёт: {newScore}";
        Debug.Log($"Score updated to: {newScore}");

        // Обновляем текущий рекорд, если это необходимо
        if (newScore > currentGameHighScore)
        {
            currentGameHighScore = newScore;
        }
    }

    /// <summary>
    /// Отображает попап завершения игры с анимацией.
    /// </summary>
    private IEnumerator ShowGameOverPopup()
    {
        yield return new WaitForEndOfFrame(); // Ждём один кадр перед активацией

        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(true); // Активируем попап

            Animator animator = gameOverPopup.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Show"); // Запуск анимации попапа
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

    /// <summary>
    /// Завершает игру, отображает попап завершения и сохраняет текущий рекорд.
    /// </summary>
    private void EndGame()
    {
        if (isGameOver) return; // Проверяем, завершена ли игра

        isGameOver = true; // Устанавливаем флаг завершения игры

        Debug.Log("Game Over! Starting popup sequence...");

        // Сохраняем текущий рекорд
        GameRecordsManager.SaveRecord(currentGameHighScore);

        StartCoroutine(ShowGameOverPopup());
    }

    /// <summary>
    /// Сбрасывает состояние игры и снимает флаг завершения.
    /// </summary>
    private void ResetView()
    {
        Debug.Log("Game reset!");
        Time.timeScale = 1; // Сбрасываем масштаб времени
    }
}