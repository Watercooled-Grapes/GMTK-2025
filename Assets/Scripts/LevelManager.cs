using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private GridManager _gridManager;
    private MainCharacter _mainCharacter;

    void Start()
    {
        // Grid manager should be initialized before the main character
        Debug.Log("Init Grid Manager");
        _gridManager = FindFirstObjectByType<GridManager>();
        if (_gridManager != null) {
            _gridManager.GenerateGrid();
        } else {
            Debug.LogError("Grid Manager is NULL");
        }

        Debug.Log("Init Main Character");
        _mainCharacter = FindFirstObjectByType<MainCharacter>();
        if (_mainCharacter != null) {
            _mainCharacter.Init();
        } else {
            Debug.LogError("Main Character is NULL");
        }
    }
}
