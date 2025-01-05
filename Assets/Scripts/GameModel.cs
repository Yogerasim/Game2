using UnityEngine;

using System;

public class GameModel
{
    public const int SIZE_X = 10;
    public const int SIZE_Y = 12;
    public const int BALLS = 7;
    public const int ADD_BALLS = 6;

    public event Action<int, int, int> OnCellUpdated;
    public event Action<int> OnScoreUpdated;
    public event Action OnGameReset; // Событие для сброса игры

    private int[,] map = new int[SIZE_X, SIZE_Y];
    private int score = 0;
    private int moveCount = 0;
    private int selectedX = -1;
    private int selectedY = -1;
    private bool isBallSelected = false;

    public void StartGame()
    {
        ClearMap();
        AddRandomBalls();
        UpdateScore(0);
        moveCount = 0;
        OnGameReset?.Invoke(); // Уведомляем подписчиков о сбросе игры
    }

    public void ClickCell(int x, int y)
    {
        if (isBallSelected)
        {
            if (map[x, y] == 0) // Если ячейка пустая
            {
                MoveBall(x, y);
                isBallSelected = false;
            }
            else
            {
                Debug.Log("Cannot move to a non-empty cell.");
            }
        }
        else
        {
            if (map[x, y] > 0) // Если ячейка с шариком
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
        if (selectedX == -1 || selectedY == -1) return;

        // Перемещение шарика
        SetMap(x, y, map[selectedX, selectedY]);
        SetMap(selectedX, selectedY, 0);

        Debug.Log($"Ball moved from ({selectedX}, {selectedY}) to ({x}, {y})");

        // Сброс выбора
        selectedX = -1;
        selectedY = -1;

        // Увеличиваем количество ходов
        moveCount++;

        // Уменьшаем счёт за ход
        int pointsToDeduct = 3 + moveCount; // 3 + T
        Debug.Log($"Move cost: -{pointsToDeduct} points (T={moveCount})");
        UpdateScore(score - pointsToDeduct);

        // Проверяем линии на удаление
        if (CutLines())
        {
            Debug.Log("Lines cut!");
        }
        else
        {
            // Добавляем новые шарики, если линии не удаляются
            AddRandomBalls();
        }
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
                    mark[x, y] = mark[x + 1, y] = mark[x + 2, y] = true;
                    hasCut = true;
                    removedBalls += 3; // Учитываем 3 удалённых шарика
                    Debug.Log($"Horizontal line detected at row {y}, starting at column {x}");
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
                    mark[x, y] = mark[x, y + 1] = mark[x, y + 2] = true;
                    hasCut = true;
                    removedBalls += 3; // Учитываем 3 удалённых шарика
                    Debug.Log($"Vertical line detected at column {x}, starting at row {y}");
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
            int pointsToAdd = removedBalls; // 1 очко за каждый удалённый шарик
            Debug.Log($"CutLines: Adding {pointsToAdd} points for removed balls.");
            UpdateScore(score + pointsToAdd);
        }

        return hasCut;
    }

    private void UpdateScore(int newScore)
    {
        score = newScore;
        Debug.Log($"Score updated to: {score}");
        OnScoreUpdated?.Invoke(score);
    }

    private void SetMap(int x, int y, int ball)
    {
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

    private void AddRandomBalls()
    {
        for (int i = 0; i < ADD_BALLS; i++)
        {
            if (!AddRandomBall())
            {
                Debug.Log("No space left for new balls. Game over!");
                return;
            }
        }
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

        int ballType = UnityEngine.Random.Range(1, BALLS);
        SetMap(x, y, ballType);
        return true;
    }
}