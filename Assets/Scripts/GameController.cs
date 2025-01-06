using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private GameModel model;
    private GameView view;

    [SerializeField] private Transform grid;
    [SerializeField] private Sprite[] ballSprites;
    [SerializeField] private Text scoreText;

    [SerializeField] private GameObject gameOverPopup; // Попап окончания игры
    [SerializeField] private Text currentGameScoreText; // Текст для счета конкретной игры
    [SerializeField] private Text highScoreText; // Текст для рекорда всех игр

    private int currentGameHighScore = 0; // Рекорд текущей игры
    private int allTimeHighScore = 0; // Рекорд за все игры

    void Start()
    {
        model = new GameModel();
        view = FindObjectOfType<GameView>();

        int columns = 12; // Установите значение constraint count
        int rows = Mathf.CeilToInt((float)grid.childCount / columns);
        model.SetGridSize(columns, rows);

        model.OnCellUpdated += view.UpdateCell;
        model.OnScoreUpdated += UpdateScore;
        model.OnGameReset += ResetView;

        view.Initialize(columns, rows, GameModel.BALLS, grid);
        view.InitializeSprites(ballSprites);
        view.InitializeButtons(this);

        allTimeHighScore = PlayerPrefs.GetInt("HighScore", 0); // Загрузка рекорда всех игр

        model.StartGame();
        gameOverPopup.SetActive(false); // Скрываем попап окончания игры
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnCellClicked(int buttonIndex)
    {
        int x = buttonIndex % GameModel.SIZE_X;
        int y = buttonIndex / GameModel.SIZE_X;
        model.ClickCell(x, y);
    }

    private void UpdateScore(int newScore)
    {
        if (newScore < 0)
        {
            EndGame(); // Показываем попап при отрицательном счёте
            return;
        }

        scoreText.text = $"Счёт: {newScore}";

        // Обновляем текущий рекорд игры
        if (newScore > currentGameHighScore)
        {
            currentGameHighScore = newScore;
        }

        // Обновляем рекорд всех игр
        if (newScore > allTimeHighScore)
        {
            allTimeHighScore = newScore;
            PlayerPrefs.SetInt("HighScore", allTimeHighScore);
            PlayerPrefs.Save();
        }
    }

    private void EndGame()
    {
        Debug.Log("Game Over!");
        gameOverPopup.SetActive(true);

        // Сохраняем рекорд текущей игры
        GameRecordsManager.SaveRecord(currentGameHighScore);

        // Отображаем рекорд текущей игры
        currentGameScoreText.text = $"Ваш рекорд: {currentGameHighScore}";

        // Отображаем общий рекорд
        highScoreText.text = $"Рекорд всех игр: {allTimeHighScore}";

        Time.timeScale = 0; // Останавливаем время
    }

    private void ResetView()
    {
        Debug.Log("Game reset!");
        Time.timeScale = 1; // Возобновляем время
    }
}