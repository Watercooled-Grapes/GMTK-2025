using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GridManager _gridManager;
    private MainCharacter _mainCharacter;
    private LoopManager _loopManager;
    private static List<IResetable> _resetables = new List<IResetable>();
    
    public interface IResetable
    {
        void ResetForLoop();
    }

    void Start()
    {
        // Grid manager should be initialized before the main character
        Debug.Log("Init Grid Manager");
        _gridManager = FindFirstObjectByType<GridManager>();
        if (_gridManager != null) {
            _gridManager.GenerateGrid();
            _resetables.Add(_gridManager);
        } else {
            Debug.LogError("Grid Manager is NULL");
        }

        Debug.Log("Init Main Character");
        _mainCharacter = FindFirstObjectByType<MainCharacter>();
        if (_mainCharacter != null) {
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
    }
}
