using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MainCharacter : MonoBehaviour
{
    [SerializeField] private Vector2 _startPosition = new Vector2(2, 3);
    [SerializeField] private Vector2 _currentPosition;
    [SerializeField] private int _steps = 5;

    private GridManager _gridManager;

    public void Init()
    {
        _gridManager = FindFirstObjectByType<GridManager>();
        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }

        Vector3 pos = _gridManager.GetTileCenterPosition(_startPosition);
        transform.position = pos;

        _currentPosition = _startPosition;
    }

    private void OnMouseDown()
    {
        HighlightPotentialDestinationTiles();
    }

    private void HighlightPotentialDestinationTiles()
    {
        List<Tile> walkableTiles = _gridManager.GetReachableTiles(_currentPosition, _steps);
        foreach (Tile tile in walkableTiles)
        {
            tile.HighlightAsMoveOption();
        }
    }
}

