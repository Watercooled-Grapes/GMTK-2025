using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MainCharacter : MonoBehaviour, LevelManager.IResetable
{
    [SerializeField] private Vector2 _startPosition = new Vector2(2, 3);
    [SerializeField] private Vector2 _currentPosition;
    [SerializeField] private int _steps = 5;
    public List<IObserver> observers = new List<IObserver>();
    private bool _isSelected = false;
    private List<Tile> _availableTiles;
    private Queue<Action> _actionsThisLoop = new Queue<Action>();

    private GridManager _gridManager;

    public interface IObserver
    {
        void OnTurnEnd(Queue<Action> action);
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
                if (targetTile != null && !targetTile.IsWall && _availableTiles.Contains(targetTile))
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
        Action action = new Action
        {
            Position = _currentPosition
        };
        _actionsThisLoop.Enqueue(action);
        Debug.Log("Player moved to " + targetTile.name);
        Debug.Log("Player moved to " + targetTile.X + targetTile.Y);
        _availableTiles.Clear();
        
        // TODO: This should be moved elsewhere if throwing mechanics are added -> after moving we should throw
        // Notify observers of turn end since player has completed move
        foreach (IObserver observer in observers)
        {
            observer.OnTurnEnd(_actionsThisLoop);
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
        observers.Clear();
        _actionsThisLoop.Clear();
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

    public void ResetForLoop()
    {
        Init();
        _actionsThisLoop.Clear();
    }
}

