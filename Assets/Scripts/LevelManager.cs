using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GridManager _gridManager;
    private MainCharacter _mainCharacter;
    private LoopManager _loopManager;

    public event Action<int[,], Vector2> Resetable;
    private Goal _goal;

    [SerializeField] private string _mapFileName;

    private int[,] _mapData;
    public Vector2 StartPosition { get; private set; }
    [SerializeField] private List<GameObject> _appPrefabs;

    void Start()
    {
        // Load map
        Debug.Log("Loading Map");
        LoadMap();

        Vector2Int? startPosition = GridManager.FindFirstPositionOfType(_mapData, GridManager.TileType.StartTile);
        Vector2Int? endPosition = GridManager.FindFirstPositionOfType(_mapData, GridManager.TileType.EndTile);

        if (startPosition == null)
        {
            Debug.LogError("Start position not found in map data!");
            return;
        }
        StartPosition = startPosition.Value;
        
        // Grid manager should be initialized before the main character
        Debug.Log("Init Grid Manager");
        _gridManager = FindFirstObjectByType<GridManager>();
        if (_gridManager != null) {
            _gridManager.GenerateGrid(_mapData);
            _gridManager.GenerateAppsIfMissing();
            Resetable += _gridManager.OnResetForLoop;

        } else {
            Debug.LogError("Grid Manager is NULL");
        }

        Debug.Log("Init Main Character");
        _mainCharacter = FindFirstObjectByType<MainCharacter>();
        if (_mainCharacter != null) {
            _mainCharacter.Init(_mapData, StartPosition);
            Resetable += _mainCharacter.OnResetForLoop;
        } else {
            Debug.LogError("Main Character is NULL");
        }

        Debug.Log("Init Goal");
        _goal = FindFirstObjectByType<Goal>();
        if (_goal != null) {
            _goal.Init(endPosition);
        } else {
            Debug.LogError("Goal is NULL");
        }
        
        Debug.Log("Init Loop Manager");
        _loopManager = FindFirstObjectByType<LoopManager>();
        if (_loopManager != null) {
            _loopManager.Init();
        } else {
            Debug.LogError("LoopManager is NULL");
        }
        _mainCharacter.TurnEnded += _loopManager.OnTurnEnd;

        Debug.Log("Init Folders");
        GameObject[] folders = GameObject.FindGameObjectsWithTag("Folder");
        foreach (GameObject go in folders)
        {
            Debug.Log(go);
            go.GetComponent<FolderScript>().Init(_mapData);
        }

        Debug.Log("Init EXE");
        GameObject[] exes = GameObject.FindGameObjectsWithTag("Exes");
        foreach (GameObject go in exes)
        {
            go.GetComponent<ExeScript>().Init(_mapData);
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

    public void RestartLevelWithLoop()
    {
        Resetable?.Invoke(_mapData, StartPosition);

        _loopManager.InitLoopInstances();
    }

    public void PauseLevel()
    {
        _mainCharacter.IsInteractable = false;
    }

    public void ResumeLevel()
    {
        _mainCharacter.IsInteractable = true;
    }
}
