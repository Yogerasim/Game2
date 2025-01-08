using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Представление игры, отвечающее за отображение игрового поля и взаимодействие с игроком.
/// </summary>
public class GameView : MonoBehaviour
{
    /// <summary>
    /// Массив кнопок, представляющих ячейки игрового поля.
    /// </summary>
    public Button[,] buttons;

    /// <summary>
    /// Массив спрайтов для отображения различных типов шариков.
    /// </summary>
    public Sprite[] ballSprites;

    /// <summary>
    /// Инициализирует кнопки игрового поля и устанавливает их позиции в соответствии с заданным размером.
    /// </summary>
    /// <param name="sizeX">Количество колонок на игровом поле.</param>
    /// <param name="sizeY">Количество строк на игровом поле.</param>
    /// <param name="ballTypes">Количество различных типов шариков.</param>
    /// <param name="parent">Родительский объект, содержащий кнопки игрового поля.</param>
    public void Initialize(int sizeX, int sizeY, int ballTypes, Transform parent)
    {
        buttons = new Button[sizeX, sizeY];
        ballSprites = new Sprite[ballTypes];

        for (int i = 0; i < parent.childCount; i++)
        {
            int x = i % sizeX;
            int y = i / sizeX;
            buttons[x, y] = parent.GetChild(i).GetComponent<Button>();
        }
    }

    /// <summary>
    /// Устанавливает массив спрайтов для отображения типов шариков.
    /// </summary>
    /// <param name="sprites">Массив спрайтов, представляющих шарики.</param>
    public void InitializeSprites(Sprite[] sprites)
    {
        ballSprites = sprites;
    }

    /// <summary>
    /// Устанавливает обработчики событий нажатия для всех кнопок игрового поля.
    /// </summary>
    /// <param name="controller">Ссылка на контроллер игры, обрабатывающий события нажатия.</param>
    public void InitializeButtons(GameController controller)
    {
        for (int i = 0; i < buttons.GetLength(0); i++)
        {
            for (int j = 0; j < buttons.GetLength(1); j++)
            {
                int buttonIndex = j * GameModel.SIZE_X + i;

                buttons[i, j].onClick.RemoveAllListeners();
                buttons[i, j].onClick.AddListener(() => controller.OnCellClicked(buttonIndex));
            }
        }
    }

    /// <summary>
    /// Обновляет содержимое ячейки игрового поля в зависимости от типа шарика.
    /// </summary>
    /// <param name="x">Координата X ячейки.</param>
    /// <param name="y">Координата Y ячейки.</param>
    /// <param name="ballType">Тип шарика для отображения (или пустая ячейка).</param>
    public void UpdateCell(int x, int y, int ballType)
    {
        if (ballType < 0 || ballType >= ballSprites.Length)
        {
            Debug.LogError($"Invalid ballType: {ballType}");
            return;
        }

        buttons[x, y].GetComponent<Image>().sprite = ballSprites[ballType];
    }
}