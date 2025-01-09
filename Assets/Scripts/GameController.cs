using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// Контроллер игры, управляющий игровой логикой и взаимодействием между моделью и представлением.
/// </summary>
public class GameController : MonoBehaviour
{
    /// <summary>
    /// Модель игры, отвечающая за логику игрового процесса.
    /// </summary>
    private GameModel model;

    /// <summary>
    /// Представление игры, управляющее визуальным отображением.
    /// </summary>
    private GameView view;

    /// <summary>
    /// Трансформ для сетки игрового поля, содержащей кнопки ячеек.
    /// </summary>
    [SerializeField]
    private Transform grid;

    /// <summary>
    /// Массив спрайтов для визуализации шариков разных типов.
    /// </summary>
    [SerializeField]
    private Sprite[] ballSprites;

    /// <summary>
    /// UI-элемент для отображения текущего счёта игрока.
    /// </summary>
    [SerializeField]
    private Text scoreText;

    /// <summary>
    /// UI-элемент для отображения попапа завершения игры.
    /// </summary>
    [SerializeField]
    private GameObject gameOverPopup;

    /// <summary>
    /// UI-элемент для отображения рекорда текущей игры в попапе завершения.
    /// </summary>
    [SerializeField]
    private Text currentGameScoreText;

    /// <summary>
    /// Панель с правилами игры, отображаемая при нажатии на кнопку "Правила игры".
    /// </summary>
    [SerializeField]
    private GameObject rulesPanel;

    /// <summary>
    /// Лучший счёт, достигнутый в текущей игровой сессии.
    /// </summary>
    private int currentGameHighScore = 0;

    /// <summary>
    /// Флаг, указывающий, завершена ли игра.
    /// </summary>
    private bool isGameOver = false;

    /// <summary>
    /// Инициализация контроллера при старте сцены.
    /// </summary>
    void Start()
    {
        model = new GameModel();
        view = FindObjectOfType<GameView>();

        int columns = 12;
        int rows = Mathf.CeilToInt((float)grid.childCount / columns);
        model.SetGridSize(columns, rows);

        model.OnCellUpdated += view.UpdateCell;
        model.OnScoreUpdated += UpdateScore;
        model.OnGameReset += ResetView;
        model.OnGameOver += EndGame;

        view.Initialize(columns, rows, GameModel.BALLS, grid);
        view.InitializeSprites(ballSprites);
        view.InitializeButtons(this);

        model.StartGame();
        gameOverPopup.SetActive(false);
        rulesPanel.SetActive(false); // Скрываем панель правил при старте
    }

    /// <summary>
    /// Показывает панель правил игры.
    /// </summary>
    public void ShowRules()
    {
        Time.timeScale = 0; // Останавливаем время игры
        rulesPanel.SetActive(true);
    }

    /// <summary>
    /// Возвращает из панели правил обратно в игру.
    /// </summary>
    public void HideRules()
    {
        rulesPanel.SetActive(false);
        Time.timeScale = 1; // Возобновляем время игры
    }

    /// <summary>
    /// Возвращает игрока в главное меню.
    /// </summary>
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
/// Обрабатывает нажатие на ячейку игрового поля.
/// </summary>
/// <param name="buttonIndex">Индекс кнопки, соответствующий выбранной ячейке.</param>
public void OnCellClicked(int buttonIndex)
{
    // Вычисление координат ячейки на основе индекса кнопки
    int x = buttonIndex % GameModel.SIZE_X;
    int y = buttonIndex / GameModel.SIZE_X;

    // Передача координат в модель для обработки
    model.ClickCell(x, y);
}

    /// <summary>
    /// Обновляет текущий счёт и проверяет, достигнут ли условия завершения игры.
    /// </summary>
    /// <param name="newScore">Новый текущий счёт.</param>
    private void UpdateScore(int newScore)
    {
        if (isGameOver) return;

        // Обновляем текст счёта
        scoreText.text = $"Счёт: {newScore}";
        Debug.Log($"Score updated to: {newScore}");

        // Завершаем игру, если счёт становится отрицательным
        if (newScore < 0)
        {
            Debug.Log("Score is negative. Ending the game.");
            EndGame();
            return;
        }

        // Обновляем текущий рекорд, если необходимо
        if (newScore > currentGameHighScore)
        {
            currentGameHighScore = newScore;
        }
    }

/// <summary>
/// Отображает попап завершения игры с анимацией.
/// </summary>
/// <returns>Корутина для активации попапа после одного кадра.</returns>
private IEnumerator ShowGameOverPopup()
{
    // Ждём один кадр перед активацией попапа
    yield return new WaitForEndOfFrame();

    if (gameOverPopup != null)
    {
        // Активируем попап
        gameOverPopup.SetActive(true);

        // Пытаемся запустить анимацию попапа
        Animator animator = gameOverPopup.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Show"); // Устанавливаем триггер для анимации
        }
    }
}

/// <summary>
/// Завершает игру, отображает попап завершения и сохраняет текущий рекорд.
/// </summary>
private void EndGame()
{
    if (isGameOver) return;

    isGameOver = true;

    Debug.Log("Game Over! Starting popup sequence...");

    // Сохраняем текущий рекорд
    if (currentGameHighScore > 0)
    {
        GameRecordsManager.SaveRecord(currentGameHighScore);
    }

    // Обновляем текст текущего рекорда в попапе
    if (currentGameScoreText != null)
    {
        currentGameScoreText.text = $"Ваш лучший счёт: {currentGameHighScore}";
        Debug.Log($"Displayed current game high score: {currentGameHighScore}");
    }
    else
    {
        Debug.LogError("Current Game Score Text is not assigned!");
    }

    // Показываем попап завершения игры
    StartCoroutine(ShowGameOverPopup());
}

/// <summary>
/// Сбрасывает представление игры для новой сессии.
/// </summary>
private void ResetView()
{
    // Возвращаем нормальную скорость игрового времени
    Time.timeScale = 1;
}
}