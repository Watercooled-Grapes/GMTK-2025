using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    private GridManager _gridManager;

    private Vector2Int? FindStartPosition(int[,] mapData, int startValue)
    {
        int width = mapData.GetLength(0);
        int height = mapData.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (mapData[x, y] == startValue)
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return null;
    }

    public void Init(Vector2 pos)
    {
        _gridManager = LevelManager.Instance.GridManager;
        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }

        if (pos == null)
        {
            Debug.LogError("Start position not found in map data!");
            return;
        }

        Vector3 posVector3 = _gridManager.GetTileCenterPosition(pos);
        transform.position = posVector3;
        
        LevelManager.Instance.LoopManager.RegisterTriggerableCallback(pos, (_) => Trigger());
    }

    public void Trigger()
    {
        GameManager.Instance.LoadNextLevelWithCutscene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
