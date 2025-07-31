using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MainCharacter : MonoBehaviour, LevelManager.IResetable
{
    [SerializeField] private Vector2 _currentPosition;
    [SerializeField] private int _steps = 5;
    public List<IObserver> observers = new List<IObserver>();
    private bool _isSelected = false;
    private List<Tile> _availableTiles;
    private List<Turn> _turnsThisLoop = new List<Turn>();

    private GridManager _gridManager;

    public interface IObserver
    {
        void OnTurnEnd(List<Turn> turns);
    }
    
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
                if (targetTile != null && !targetTile.IsWall && _availableTiles.Contains(targetTile) && targetTile != _gridManager.GetTileByWorldCoordinate(_currentPosition))
                {
                    MoveMainCharacter(targetTile);
                }
            }
        }
    }

    private void MoveMainCharacter(Tile targetTile)
    {
        // Move player to the tile
        transform.position = _gridManager.GetTileCenterPosition(targetTile);
        _isSelected = false;
        RemoveHightlights(_availableTiles);
        _currentPosition = new Vector2(targetTile.X, targetTile.Y);
        Turn turn = new Turn
        {
            Position = _currentPosition
        };
        _turnsThisLoop.Add(turn);
        Debug.Log("Player moved to " + targetTile.name);
        Debug.Log("Player moved to " + targetTile.X + targetTile.Y);
        _availableTiles.Clear();
        
        // TODO: This should be moved elsewhere if throwing mechanics are added -> after moving we should throw
        // Notify observers of turn end since player has completed move
        // foreach (IObserver observer in observers)
        // {
        //     observer.OnTurnEnd(_turnsThisLoop);
        // }
        FindFirstObjectByType<LoopManager>().OnTurnEnd(_turnsThisLoop);
    }
    
    public void Init(int[,] mapData)
    {
        _gridManager = FindFirstObjectByType<GridManager>();
        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }

        Vector2Int? startPos = FindStartPosition(mapData, 2); // 2 is the player start marker
        if (startPos == null)
        {
            Debug.LogError("Start position not found in map data!");
            return;
        }

        Vector2Int start = startPos.Value;
        Vector3 pos = _gridManager.GetTileCenterPosition(start);
        transform.position = pos;

        observers.Clear();
        _turnsThisLoop.Clear();
        _currentPosition = start;
        Turn turn = new Turn
        {
            Position = _currentPosition
        };
        _turnsThisLoop.Add(turn);
    }

    public Vector2Int? FindStartPosition(int[,] mapData, int startValue)
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

    public void ResetForLoop(int[,] mapData)
    {
        Init(mapData);
        _turnsThisLoop.Clear();
    }
}

