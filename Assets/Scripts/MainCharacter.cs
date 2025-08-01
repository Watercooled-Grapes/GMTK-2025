using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using static GridManager;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class MainCharacter : MonoBehaviour
{
    [SerializeField] public Vector2 _currentPosition;
    private List<Turn> _turnsThisLoop = new List<Turn>();

    private bool _isSelected = false;
    private Dictionary<Tile, int> _availableTiles;

    private GridManager _gridManager;
    private LoopManager _loopManager;

    // Events
    public event Action<List<Turn>> TurnEnded;

    public bool IsInteractable { get; set; } = true;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && IsInteractable)
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

                if (targetTile != null && targetTile.TileType != TileType.WallTile && _availableTiles.ContainsKey(targetTile))
                {
                    MoveMainCharacter(targetTile);
                }
            }
        }
    }

    public void TeleportMainCharacter(Tile newTile)
    {
        _isSelected = false;
        RemoveHightlights();
        Debug.Log("Player moved to " + newTile.name);
        _availableTiles.Clear();
        SetPositionWithLockedZ(_gridManager.GetTileCenterPosition(newTile));
        _currentPosition = new Vector2(newTile.X, newTile.Y);
    }

    private void MoveMainCharacter(Tile newTile)
    {
        // Move player to the tile
        List<Tile> path = _gridManager.GetPathToTile(_currentPosition, newTile);
        _isSelected = false;
        RemoveHightlights();
        Debug.Log("Player moved to " + newTile.name);

        _availableTiles.Clear();
        IsInteractable = false;
        StartCoroutine(MoveAlongPath(path));
    }

    private IEnumerator MoveAlongPath(List<Tile> path)
    {
        foreach (Tile tile in path)
        {
            Vector3 targetPos = _gridManager.GetTileCenterPosition(tile);
            targetPos.z = -5;

            if (_currentPosition.x < tile.X)
            {
                this.GetComponent<Animator>().SetTrigger("right");
            }
            else if (_currentPosition.x > tile.X)
            {
                this.GetComponent<Animator>().SetTrigger("left");
            }
            else if (_currentPosition.y < tile.Y)
            {
                this.GetComponent<Animator>().SetTrigger("up");
            }
            else
            {
                this.GetComponent<Animator>().SetTrigger("down");
            }

            while ((transform.position - targetPos).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, 5f * Time.deltaTime);
                yield return null;
            }

            SetPositionWithLockedZ(targetPos);

            _currentPosition = new Vector2(tile.X, tile.Y);

            // Optional: Wait between steps to show movement rhythm
            yield return new WaitForSeconds(0.1f);

            // Log or animate step if needed
            Debug.Log("Step to " + tile.name);

            if (tile.TileType == TileType.AppTile)
            {
                tile.appBroken = true;
            }

            Turn turn = new Turn
            {
                Position = _currentPosition
            };
            _turnsThisLoop.Add(turn);

            TurnEnded?.Invoke(_turnsThisLoop);
        }
        this.GetComponent<Animator>().SetTrigger("idle");
        IsInteractable = true;
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
        SetPositionWithLockedZ(pos);

        _currentPosition = startPosition;

        _turnsThisLoop.Clear();
        if (_availableTiles != null)
        {
            RemoveHightlights();
            _availableTiles.Clear();

        }

        _loopManager = FindFirstObjectByType<LoopManager>();
    }

    private void OnMouseDown()
    {
        if (IsInteractable)
        {
            _availableTiles = _gridManager.GetReachableTiles(_currentPosition, _loopManager.curMaxTurns - GetCurrentTurn());
            HighlightPotentialDestinationTiles();
            _isSelected = true;
        }
    }

    private void HighlightPotentialDestinationTiles()
    {
        foreach (var kvp in _availableTiles)
        {
            kvp.Key.HighlightAsMoveOption();
            kvp.Key.DisplayText(kvp.Value.ToString());
        }
    }

    private void RemoveHightlights()
    {
        foreach (var kvp in _availableTiles)
        {
            kvp.Key.RemoveHighlight();
            kvp.Key.DisplayText("");
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

    // We lock the Z coordinate to prevent accidentally change the Z coordinate of the main character
    // Otherwise it might break ray cast system.
    private void SetPositionWithLockedZ(Vector3 targetPos) {
        transform.position = new Vector3(targetPos.x, targetPos.y, -5);
    }
}
