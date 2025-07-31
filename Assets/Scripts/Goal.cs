using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] private Vector2 _pos;

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

    public void Init(int[,] mapData)
    {
        _gridManager = FindFirstObjectByType<GridManager>();
        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }

        Vector2Int? startPos = FindStartPosition(mapData, 3);
        if (startPos == null)
        {
            Debug.LogError("Start position not found in map data!");
            return;
        }

        Vector2Int start = startPos.Value;
        Vector3 pos = _gridManager.GetTileCenterPosition(start);
        transform.position = pos;

        _pos = start;
    }

    void Win()
    {
        // TODO: do something
    }
}
