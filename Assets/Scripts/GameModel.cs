using System;
using UnityEngine;

public class GameModel
{
    public static int SIZE_X = 10;
    public static int SIZE_Y = 12;
    public const int BALLS = 7;
    public const int BASE_BALLS_TO_ADD = 6; // Начальное количество шариков для добавления

    public event Action<int, int, int> OnCellUpdated;
    public event Action<int> OnScoreUpdated;
    public event Action OnGameReset;
    public event Action OnGameOver; // Событие окончания игры

    private int[,] map;
    private int score = 0;
    private int ballsToAdd = BASE_BALLS_TO_ADD; // Количество добавляемых шариков
    private int selectedX = -1;
    private int selectedY = -1;
    private bool isBallSelected = false;

    public GameModel()
    {
        map = new int[SIZE_X, SIZE_Y];
    }

    public void SetGridSize(int columns, int rows)
    {
        SIZE_X = columns;
        SIZE_Y = rows;
        map = new int[SIZE_X, SIZE_Y];
    }

    public void StartGame()
    {
        ClearMap();
        ballsToAdd = BASE_BALLS_TO_ADD; // Сбрасываем количество добавляемых шариков
        AddRandomBalls();
        UpdateScore(10);
        OnGameReset?.Invoke();
    }

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

    private void SelectBall(int x, int y)
    {
        selectedX = x;
        selectedY = y;
        isBallSelected = true;
        Debug.Log($"Ball selected at ({x}, {y})");
    }

    private void MoveBall(int x, int y)
    {
        if (selectedX == -1 || selectedY == -1 || map[x, y] != 0) return;

        SetMap(x, y, map[selectedX, selectedY]);
        SetMap(selectedX, selectedY, 0);

        Debug.Log($"Ball moved from ({selectedX}, {selectedY}) to ({x}, {y})");

        selectedX = -1;
        selectedY = -1;

        // Проверяем линии на удаление
        if (CutLines())
        {
            Debug.Log("Lines cut!");
        }
        else
        {
            UpdateScore(score - 4);
            AddRandomBalls();
        }
    }

    private void AddRandomBalls()
    {
        int added = 0;

        for (int i = 0; i < ballsToAdd; i++)
        {
            if (AddRandomBall())
            {
                added++;
            }
            else
            {
                // Если не удалось добавить шарик, поле заполнено
                OnGameOver?.Invoke();
                return;
            }
        }

        Debug.Log($"Added {added} random balls.");
        ballsToAdd += 2; // Увеличиваем количество шариков для следующего хода
    }

    private bool AddRandomBall()
    {
        int x, y;
        int attempts = 100;

        do
        {
            x = UnityEngine.Random.Range(0, SIZE_X);
            y = UnityEngine.Random.Range(0, SIZE_Y);
            attempts--;

            if (attempts <= 0) return false;
        } while (map[x, y] > 0);

        int ballType = UnityEngine.Random.Range(1, BALLS); // Генерация типа шарика от 1 до BALLS - 1
        SetMap(x, y, ballType);

        return true;
    }

    private bool CutLines()
    {
        bool hasCut = false;
        bool[,] mark = new bool[SIZE_X, SIZE_Y];
        int removedBalls = 0;

        // Проверяем горизонтальные линии
        for (int y = 0; y < SIZE_Y; y++)
        {
            for (int x = 0; x < SIZE_X - 2; x++)
            {
                int ball = map[x, y];
                if (ball > 0 && ball == map[x + 1, y] && ball == map[x + 2, y])
                {
                    int count = 0;
                    while (x + count < SIZE_X && map[x + count, y] == ball)
                    {
                        mark[x + count, y] = true;
                        count++;
                    }
                    removedBalls += count;
                    hasCut = true;
                    Debug.Log($"Horizontal line detected at row {y}, starting at column {x}, length {count}");

                    x += count - 1; // Пропускаем обработанную часть
                }
            }
        }

        // Проверяем вертикальные линии
        for (int x = 0; x < SIZE_X; x++)
        {
            for (int y = 0; y < SIZE_Y - 2; y++)
            {
                int ball = map[x, y];
                if (ball > 0 && ball == map[x, y + 1] && ball == map[x, y + 2])
                {
                    int count = 0;
                    while (y + count < SIZE_Y && map[x, y + count] == ball)
                    {
                        mark[x, y + count] = true;
                        count++;
                    }
                    removedBalls += count;
                    hasCut = true;
                    Debug.Log($"Vertical line detected at column {x}, starting at row {y}, length {count}");

                    y += count - 1; // Пропускаем обработанную часть
                }
            }
        }

        // Удаляем помеченные шарики
        for (int x = 0; x < SIZE_X; x++)
        {
            for (int y = 0; y < SIZE_Y; y++)
            {
                if (mark[x, y])
                {
                    SetMap(x, y, 0);
                }
            }
        }

        // Начисляем очки за удалённые шарики
        if (removedBalls > 0)
        {
            Debug.Log($"CutLines: Adding {removedBalls} points for removed balls.");
            UpdateScore(score + removedBalls);
        }

        return hasCut;
    }

    private void UpdateScore(int newScore)
    {
        score = newScore;
        OnScoreUpdated?.Invoke(score);

        if (score < 0)
        {
            OnGameOver?.Invoke(); // Завершаем игру при отрицательном счёте
        }

        Debug.Log($"Score updated to: {score}");
    }

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
}