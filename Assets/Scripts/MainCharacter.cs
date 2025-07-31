using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MainCharacter : MonoBehaviour
{
    [SerializeField] private Vector2 _startPosition = new Vector2(2, 3);
    [SerializeField] private Vector2 _currentPosition;
    [SerializeField] private int _steps = 5;

    private bool _isSelected = false;
    private List<Tile> _availableTiles;

    private GridManager _gridManager;

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
                    // Move player to the tile
                    transform.position = _gridManager.GetTileCenterPosition(targetTile);
                    _isSelected = false;
                    RemoveHightlights(_availableTiles);
                    _currentPosition = new Vector2(targetTile.X, targetTile.Y);
                    Debug.Log("Player moved to " + targetTile.name);
                    Debug.Log("Player moved to " + targetTile.X + targetTile.Y);

                    _availableTiles.Clear();
                }
            }
        }
    }

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
        _availableTiles = _gridManager.GetReachableTiles(_currentPosition, _steps);
        HighlightPotentialDestinationTiles(_availableTiles);
        _isSelected = true;
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
}

