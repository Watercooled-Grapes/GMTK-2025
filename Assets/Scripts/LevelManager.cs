using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GridManager _gridManager;
    private MainCharacter _mainCharacter;
    private Goal _goal;

    [SerializeField] private string _mapFileName;

    private int[,] _mapData;

    void Start()
    {
        // Load map
        Debug.Log("Loading Map");
        LoadMap();

        // Grid manager should be initialized before the main character
        Debug.Log("Init Grid Manager");
        _gridManager = FindFirstObjectByType<GridManager>();
        if (_gridManager != null) {
            _gridManager.GenerateGrid(_mapData);
        } else {
            Debug.LogError("Grid Manager is NULL");
        }

        Debug.Log("Init Main Character");
        _mainCharacter = FindFirstObjectByType<MainCharacter>();
        if (_mainCharacter != null) {
            _mainCharacter.Init(_mapData);
        } else {
            Debug.LogError("Main Character is NULL");
        }

        Debug.Log("Init Goal");
        _goal = FindFirstObjectByType<Goal>();
        if (_goal != null) {
            _goal.Init(_mapData);
        } else {
            Debug.LogError("Goal is NULL");
        }
    }

    private void LoadMap()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(_mapFileName);
        if (textAsset == null) {
            Debug.LogError("Map file not found in Resources/!");
            return;
        }

        string[] lines = textAsset.text.Trim().Split('\n');
        int height = lines.Length;
        int width = lines[0].Split(',').Length;

        _mapData = new int[height, width];

        for (int y = 0; y < height; y++) {
            string[] cells = lines[y].Trim().Split(',');
            for (int x = 0; x < width; x++) {
                _mapData[x, height - 1 - y] = int.Parse(cells[x]);  // flip Y
            }
        }

        Debug.Log($"Loaded map {width}x{height}");
    }
}
