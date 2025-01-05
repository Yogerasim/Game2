using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    public Button[,] buttons;
    public Sprite[] ballSprites;

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
    
    public void InitializeSprites(Sprite[] sprites)
    {
        if (sprites.Length != ballSprites.Length)
        {
            Debug.LogError("Sprite array length does not match ball types.");
            return;
        }

        for (int i = 0; i < sprites.Length; i++)
        {
            ballSprites[i] = sprites[i];
        }
    }


    public void InitializeButtons(GameController controller)
    {
        for (int i = 0; i < buttons.GetLength(0); i++)
        {
            for (int j = 0; j < buttons.GetLength(1); j++)
            {
                int buttonIndex = j * GameModel.SIZE_X + i;

                buttons[i, j].onClick.RemoveAllListeners();
                buttons[i, j].onClick.AddListener(() =>
                {
                    // Убираем лишнее логирование
                    controller.OnCellClicked(buttonIndex);
                });
            }
        }
    }

    public void UpdateCell(int x, int y, int ballType)
    {
        if (ballType >= 0 && ballType < ballSprites.Length)
        {
            buttons[x, y].GetComponent<Image>().sprite = ballSprites[ballType];
        }
        else
        {
            Debug.LogError($"Invalid ballType: {ballType}");
        }
    }
}