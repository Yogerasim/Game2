using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private GameModel model;
    private GameView view;

    [SerializeField] private Transform grid;
    [SerializeField] private Sprite[] ballSprites;
    [SerializeField] private Text scoreText; // Текст для отображения счёта

    void Start()
    {
        model = new GameModel();
        view = FindObjectOfType<GameView>();

        model.OnCellUpdated += view.UpdateCell;
        model.OnScoreUpdated += UpdateScore; // Подписываемся на обновление счёта
        model.OnGameReset += ResetView;

        view.Initialize(GameModel.SIZE_X, GameModel.SIZE_Y, GameModel.BALLS, grid);
        view.InitializeSprites(ballSprites);
        view.InitializeButtons(this);

        model.StartGame();
        
        UpdateScore(10); 
    }
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Имя вашей сцены с меню
    }

    public void OnCellClicked(int buttonIndex)
    {
        int x = buttonIndex % GameModel.SIZE_X;
        int y = buttonIndex / GameModel.SIZE_X;
        model.ClickCell(x, y);
    }

    private void UpdateScore(int newScore)
    {
        scoreText.text = $"Score: {newScore}"; // Обновляем текст счёта
        Debug.Log($"Controller: Score text updated to {newScore}"); // Проверяем обновление текста
    }

    private void ResetView()
    {
        Debug.Log("Game reset!");
    }
}