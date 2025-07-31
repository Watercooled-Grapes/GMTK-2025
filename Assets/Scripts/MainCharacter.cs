using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MainCharacter : MonoBehaviour
{
    [SerializeField] private Vector2 _currentPosition;
    [SerializeField] private int _steps = 5;
    private List<Turn> _turnsThisLoop = new List<Turn>();

    private bool _isSelected = false;
    private List<Tile> _availableTiles;

    private GridManager _gridManager;

    // Events
    public event Action<List<Turn>> TurnEnded;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            // Clicking main character itself
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);
            if (hit != null && hit.gameObject == gameObject)
            {
                _isSelected = true;
                Debug.Log("Player selected!");
                return;
            }

            // If the player is already selected
            if (_isSelected)
            {
                Tile targetTile = _gridManager.GetTileByWorldCoordinate(mouseWorldPos);
                if (targetTile != null && !targetTile.IsWall && _availableTiles.Contains(targetTile))
                {
                    MoveMainCharacter(targetTile);
                }
            }
        }
    }

    private void MoveMainCharacter(Tile newTile)
    {
        // Move player to the tile
        transform.position = _gridManager.GetTileCenterPosition(newTile);
        _isSelected = false;
        RemoveHightlights(_availableTiles);
        _currentPosition = new Vector2(newTile.X, newTile.Y);
        Debug.Log("Player moved to " + newTile.name);

        _availableTiles.Clear();

        Turn turn = new Turn
        {
            Position = _currentPosition
        };
        _turnsThisLoop.Add(turn);

        TurnEnded?.Invoke(_turnsThisLoop);
    }

    public void Init(int[,] mapData, Vector2 startPosition)
    {
        _gridManager = FindFirstObjectByType<GridManager>();
        _isSelected = false;

        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }
        Vector3 pos = _gridManager.GetTileCenterPosition(startPosition);
        transform.position = pos;

        _currentPosition = startPosition;

        _turnsThisLoop.Clear();
        if (_availableTiles != null)
        {
            RemoveHightlights(_availableTiles);
            _availableTiles.Clear();
        }
    }

    private void OnMouseDown()
    {
        if (enabled)
        {
            _availableTiles = _gridManager.GetReachableTiles(_currentPosition, _steps);
            HighlightPotentialDestinationTiles(_availableTiles);
            _isSelected = true;
        }
    }

    private void HighlightPotentialDestinationTiles(List<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            tile.HighlightAsMoveOption();
        }
    }

    private void RemoveHightlights(List<Tile> tiles)
    {
        foreach (Tile tile in tiles)
        {
            tile.RemoveHighlight();
        }
    }

    public void OnResetForLoop(int[,] mapData, Vector2 startPosition)
    {
        Init(mapData, startPosition);
        _turnsThisLoop.Clear();
    }

    public int GetCurrentTurn()
    {
        return _turnsThisLoop.Count;
    }
}
