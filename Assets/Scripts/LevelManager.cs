using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GridManager _gridManager;
    private MainCharacter _mainCharacter;
    private LoopManager _loopManager;
    private Goal _goal;

    public GridManager GridManager => _gridManager;
    public MainCharacter MainCharacter => _mainCharacter;
    public LoopManager LoopManager => _loopManager;
    public Goal Goal => _goal;

    public event Action<int[,], Vector2> ResetableCallbacks;

    [SerializeField] private string _mapFileName;

    private int[,] _mapData;
    public Vector2 StartPosition { get; private set; }
    public Vector2 EndPosition { get; private set; }
    [SerializeField] private List<GameObject> _appPrefabs;

    public static LevelManager Instance { get; private set; }

    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple LevelManager instances found. Destroying this one.");
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Load map
        Debug.Log("Loading Map");
        LoadMap();

        StartPosition = GridManager.FindFirstPositionOfType(_mapData, GridManager.TileType.StartTile);
        EndPosition = GridManager.FindFirstPositionOfType(_mapData, GridManager.TileType.EndTile);

        if (StartPosition == null)
        {
            Debug.LogError("Start position not found in map data!");
            return;
        }
        
        // Grid manager should be initialized before the main character
        // WARNING: the initialize orders matters.
        Debug.Log("Init Grid Manager");
        _gridManager = FindFirstObjectByType<GridManager>();
        if (_gridManager != null) {
            _gridManager.GenerateGrid(_mapData);
            ResetableCallbacks += _gridManager.OnResetForLoop;

        } else {
            Debug.LogError("Grid Manager is NULL");
        }
   
        Debug.Log("Init Loop Manager");
        _loopManager = FindFirstObjectByType<LoopManager>();
        if (_loopManager != null) {
            _loopManager.Init();
        } else {
            Debug.LogError("LoopManager is NULL");
        }

        Debug.Log("Init Main Character");
        _mainCharacter = FindFirstObjectByType<MainCharacter>();
        if (_mainCharacter != null) {
            _mainCharacter.Init(_mapData, StartPosition);
            ResetableCallbacks += _mainCharacter.OnResetForLoop;
        } else {
            Debug.LogError("Main Character is NULL");
        }

        Debug.Log("Init Goal");
        _goal = FindFirstObjectByType<Goal>();
        if (_goal != null) {
            _goal.Init(EndPosition);
        } else {
            Debug.LogError("Goal is NULL");
        }

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
            ResetableCallbacks += go.GetComponent<ExeScript>().OnResetForLoop;
        }

        Debug.Log("Init Doors and Levers");
        GameObject[] gates = GameObject.FindGameObjectsWithTag("Gates");
        foreach (GameObject go in gates)
        {
            go.GetComponent<GateScript>().Init();
            ResetableCallbacks += go.GetComponent<GateScript>().OnResetForLoop;
        }

        GameObject[] levers = GameObject.FindGameObjectsWithTag("Levers");
        foreach (GameObject go in levers)
        {
            go.GetComponent<LeverScript>().Init();
            ResetableCallbacks += go.GetComponent<LeverScript>().OnResetForLoop;
        }

        GameObject[] apps = GameObject.FindGameObjectsWithTag("Apps");
        foreach (GameObject go in apps)
        {
            go.GetComponent<AppController>().Init();
            ResetableCallbacks += go.GetComponent<AppController>().OnResetForLoop;
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
        
        _mapData = new int[width, height];
        for (int y = height-1; y >= 0; y--) {
            string[] cells = lines[height-1-y].Trim().Split(',');
            for (int x = 0; x < width; x++) {
                _mapData[x, y] = int.Parse(cells[x].Trim());  // flip Y
            }
        }

        Debug.Log($"Loaded map {width}x{height}");
    }

    public void RestartLevelWithLoop()
    {
        ResetableCallbacks?.Invoke(_mapData, StartPosition);
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
