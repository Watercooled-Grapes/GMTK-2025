using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GridManager _gridManager;
    private MainCharacter _mainCharacter;
<<<<<<< HEAD
    private LoopManager _loopManager;
    private static List<IResetable> _resetables = new List<IResetable>();
    
    public interface IResetable
    {
        void ResetForLoop();
    }
=======
    private Goal _goal;

    [SerializeField] private string _mapFileName;

    private int[,] _mapData;
>>>>>>> 5e3594b31cefce582280be7d2dd60947925a1e6e

    void Start()
    {
        // Load map
        Debug.Log("Loading Map");
        LoadMap();

        // Grid manager should be initialized before the main character
        Debug.Log("Init Grid Manager");
        _gridManager = FindFirstObjectByType<GridManager>();
        if (_gridManager != null) {
<<<<<<< HEAD
            _gridManager.GenerateGrid();
            _resetables.Add(_gridManager);
=======
            _gridManager.GenerateGrid(_mapData);
>>>>>>> 5e3594b31cefce582280be7d2dd60947925a1e6e
        } else {
            Debug.LogError("Grid Manager is NULL");
        }

        Debug.Log("Init Main Character");
        _mainCharacter = FindFirstObjectByType<MainCharacter>();
        if (_mainCharacter != null) {
<<<<<<< HEAD
            _mainCharacter.Init();
            _resetables.Add(_mainCharacter);
        } else {
            Debug.LogError("Main Character is NULL");
        }
        
        Debug.Log("Init Loop Manager");
        _loopManager = FindFirstObjectByType<LoopManager>();

        _mainCharacter.observers.Add(_loopManager);
    }

    public static void RestartLevelWithLoop()
    {
        foreach (var resetable in _resetables)
        {
            resetable.ResetForLoop();
        }
=======
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
>>>>>>> 5e3594b31cefce582280be7d2dd60947925a1e6e
    }
}
