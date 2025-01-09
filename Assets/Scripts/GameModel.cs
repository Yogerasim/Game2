using System;
using UnityEngine;

/// <summary>
/// Модель игры, ответственная за управление игровым полем, очками и логикой ходов.
/// </summary>
public class GameModel
{
    /// <summary>
    /// Количество столбцов игрового поля.
    /// </summary>
    public static int SIZE_X = 12;

    /// <summary>
    /// Количество строк игрового поля.
    /// </summary>
    public static int SIZE_Y = 10;

    /// <summary>
    /// Количество типов шариков.
    /// </summary>
    public const int BALLS = 7;

    /// <summary>
    /// Начальное количество шариков, добавляемых на поле за ход.
    /// </summary>
    public const int BASE_BALLS_TO_ADD = 6;

    /// <summary>
    /// Увеличение количества добавляемых шариков за каждый ход.
    /// </summary>
    public const int INCREASE_PER_MOVE = 5;

    /// <summary>
    /// Событие, вызываемое при обновлении ячейки игрового поля.
    /// </summary>
    public event Action<int, int, int> OnCellUpdated;

    /// <summary>
    /// Событие, вызываемое при обновлении очков.
    /// </summary>
    public event Action<int> OnScoreUpdated;

    /// <summary>
    /// Событие, вызываемое при сбросе игрового поля.
    /// </summary>
    public event Action OnGameReset;

    /// <summary>
    /// Событие, вызываемое при завершении игры.
    /// </summary>
    public event Action OnGameOver;

    /// <summary>
    /// Двумерный массив, представляющий игровое поле.
    /// 0 - пустая ячейка, положительное значение - тип шарика.
    /// </summary>
    private int[,] map;

    /// <summary>
    /// Текущий счёт игрока.
    /// </summary>
    private int score = 0;

    /// <summary>
    /// Количество шариков, которое добавляется на поле при каждом новом ходе.
    /// </summary>
    private int ballsToAdd = BASE_BALLS_TO_ADD;

    /// <summary>
    /// Координата X выбранной ячейки (если шарик был выбран). -1, если шарик не выбран.
    /// </summary>
    private int selectedX = -1;

    /// <summary>
    /// Координата Y выбранной ячейки (если шарик был выбран). -1, если шарик не выбран.
    /// </summary>
    private int selectedY = -1;

    /// <summary>
    /// Флаг, указывающий, выбран ли шарик для перемещения.
    /// </summary>
    private bool isBallSelected = false;

    /// <summary>
    /// Конструктор игровой модели, инициализирующий игровое поле.
    /// </summary>
    public GameModel()
    {
        map = new int[SIZE_X, SIZE_Y];
    }

    /// <summary>
    /// Устанавливает размеры сетки игрового поля.
    /// </summary>
    /// <param name="columns">Количество столбцов.</param>
    /// <param name="rows">Количество строк.</param>
    public void SetGridSize(int columns, int rows)
    {
        SIZE_X = columns;
        SIZE_Y = rows;
        map = new int[SIZE_X, SIZE_Y];
    }

    /// <summary>
    /// Запускает игру: очищает игровое поле, добавляет начальные шарики и сбрасывает счёт.
    /// </summary>
    public void StartGame()
    {
        ClearMap();
        ballsToAdd = BASE_BALLS_TO_ADD;
        AddRandomBalls(ballsToAdd);
        UpdateScore(10);
        OnGameReset?.Invoke();
    }

    /// <summary>
    /// Обрабатывает клик по ячейке игрового поля.
    /// </summary>
    /// <param name="x">Координата X ячейки.</param>
    /// <param name="y">Координата Y ячейки.</param>
    public void ClickCell(int x, int y)
    {
        if (isBallSelected)
        {
            if (map[x, y] == 0)
            {
                MoveBall(x, y);
                isBallSelected = false;
            }
        }
        else
        {
            if (map[x, y] > 0)
            {
                SelectBall(x, y);
            }
        }
    }

    /// <summary>
    /// Выбирает шарик для перемещения.
    /// </summary>
    /// <param name="x">Координата X выбранной ячейки.</param>
    /// <param name="y">Координата Y выбранной ячейки.</param>
    private void SelectBall(int x, int y)
    {
        selectedX = x;
        selectedY = y;
        isBallSelected = true;
        Debug.Log($"Ball selected at ({x}, {y})");
    }

    /// <summary>
    /// Перемещает выбранный шарик в указанную ячейку.
    /// </summary>
    /// <param name="x">Координата X целевой ячейки.</param>
    /// <param name="y">Координата Y целевой ячейки.</param>
    private void MoveBall(int x, int y)
    {
        if (selectedX == -1 || selectedY == -1 || map[x, y] != 0) return;

        SetMap(x, y, map[selectedX, selectedY]);
        SetMap(selectedX, selectedY, 0);

        Debug.Log($"Ball moved from ({selectedX}, {selectedY}) to ({x}, {y})");

        selectedX = -1;
        selectedY = -1;

        if (CutLines())
        {
            Debug.Log("Lines cut!");
        }
        else
        {
            UpdateScore(score - 4);
            ballsToAdd += INCREASE_PER_MOVE;
            if (!AddRandomBalls(ballsToAdd))
            {
                OnGameOver?.Invoke();
            }
        }
    }

    /// <summary>
    /// Удаляет линии одинаковых шариков и начисляет очки.
    /// </summary>
    /// <returns>Возвращает true, если были удалены линии.</returns>
    private bool CutLines()
    {
        bool hasCut = false;
        bool[,] mark = new bool[SIZE_X, SIZE_Y];
        int removedBalls = 0;

        // Проверяет горизонтальные линии.
        for (int y = 0; y < SIZE_Y; y++)
        {
            for (int x = 0; x < SIZE_X - 2; x++)
            {
                int ball = map[x, y];
                if (ball > 0 && ball == map[x + 1, y] && ball == map[x + 2, y])
                {
                    for (int k = 0; k < 3; k++)
                    {
                        mark[x + k, y] = true;
                    }
                    removedBalls += 3;
                    hasCut = true;
                }
            }
        }

        // Проверяет вертикальные линии.
        for (int x = 0; x < SIZE_X; x++)
        {
            for (int y = 0; y < SIZE_Y - 2; y++)
            {
                int ball = map[x, y];
                if (ball > 0 && ball == map[x, y + 1] && ball == map[x, y + 2])
                {
                    for (int k = 0; k < 3; k++)
                    {
                        mark[x, y + k] = true;
                    }
                    removedBalls += 3;
                    hasCut = true;
                }
            }
        }

        // Удаляет помеченные шарики.
        for (int x = 0; x < SIZE_X; x++)
        {
            for (int y = 0; y < SIZE_Y; y++)
            {
                if (mark[x, y]) SetMap(x, y, 0);
            }
        }

        if (removedBalls > 0)
        {
            UpdateScore(score + removedBalls);
        }

        return hasCut;
    }

    /// <summary>
    /// Обновляет счёт и вызывает событие обновления очков.
    /// </summary>
    /// <param name="newScore">Новое значение счёта.</param>
    private void UpdateScore(int newScore)
    {
        score = newScore;
        OnScoreUpdated?.Invoke(score);
    }

    /// <summary>
    /// Устанавливает значение в ячейке игрового поля.
    /// </summary>
    /// <param name="x">Координата X ячейки.</param>
    /// <param name="y">Координата Y ячейки.</param>
    /// <param name="ball">Тип шарика (0 для пустой ячейки).</param>
    private void SetMap(int x, int y, int ball)
    {
        if (ball < 0 || ball >= BALLS)
        {
            Debug.LogError($"Invalid ballType: {ball}");
            return;
        }
        map[x, y] = ball;
        OnCellUpdated?.Invoke(x, y, ball);
    }

    /// <summary>
    /// Очищает игровое поле.
    /// </summary>
    private void ClearMap()
    {
        for (int x = 0; x < SIZE_X; x++)
        {
            for (int y = 0; y < SIZE_Y; y++)
            {
                SetMap(x, y, 0);
            }
        }
    }

    /// <summary>
    /// Добавляет указанное количество случайных шариков на поле.
    /// Завершает игру, если поле полностью заполнено.
    /// </summary>
    /// <param name="count">Количество шариков для добавления.</param>
    /// <returns>True, если шарики были успешно добавлены; иначе False.</returns>
    private bool AddRandomBalls(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (!AddRandomBall())
            {
                Debug.LogWarning("Field is full. Ending the game.");
                OnGameOver?.Invoke(); // Завершаем игру
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Добавляет один случайный шарик на игровое поле.
    /// </summary>
    /// <returns>True, если шарик добавлен успешно, иначе false.</returns>
    private bool AddRandomBall()
    {
        for (int attempts = 0; attempts < 100; attempts++)
        {
            int x = UnityEngine.Random.Range(0, SIZE_X);
            int y = UnityEngine.Random.Range(0, SIZE_Y);
            if (map[x, y] == 0)
            {
                int ballType = UnityEngine.Random.Range(1, BALLS);
                SetMap(x, y, ballType);
                return true;
            }
        }
        return false;
    }
}