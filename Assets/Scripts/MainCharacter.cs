using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using static GridManager;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class MainCharacter : MonoBehaviour
{
    public enum PopupTypes { Str, Img };

    [SerializeField] public Vector2 _currentPosition;
    [SerializeField] private PopupManager _popupManager;
    [SerializeField] private List<int> popupOnTurns;
    [SerializeField] private List<PopupTypes> popupType;
    private int _popupTypeIt = 0;
    private List<Turn> _turnsThisLoop = new List<Turn>();

    private bool _isSelected = false;
    private Dictionary<Tile, int> _availableTiles;

    private GridManager _gridManager;
    private LoopManager _loopManager;
    private GameObject[] _exes;
    private GameObject[] _folders;

    private AudioSource _audioSource; 
    private Animator _animator;
    [SerializeField] private AudioClip _stepSoundEffect; 

    public bool IsInteractable { get; set; } = true;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _animator = this.GetComponent<Animator>();
    }

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
        LevelManager.Instance.LoopManager.tilesToMove = path.Count;
        StartCoroutine(MoveAlongPath(path));
    }

    private IEnumerator MoveAlongPath(List<Tile> path)
    {
        foreach (Tile tile in path)
        {
            Vector3 targetPos = _gridManager.GetTileCenterPosition(tile);
            targetPos.z = -5;

            _audioSource.PlayOneShot(_stepSoundEffect);

            if (_currentPosition.x < tile.X)
            {
                _animator.SetTrigger("right");
            }
            else if (_currentPosition.x > tile.X)
            {
                _animator.SetTrigger("left");
            }
            else if (_currentPosition.y < tile.Y)
            {
                _animator.SetTrigger("up");
            }
            else
            {
                _animator.SetTrigger("down");
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

            Boolean turnAdded = false;


            foreach (GameObject go in _exes)
            {
                ExeScript e = go.GetComponent<ExeScript>().TryCollect();
                if (e != null)
                {
                    Turn turn = new Turn {
                        Position = _currentPosition,
                        exe = e
                    };
                    _turnsThisLoop.Add(turn);
                    _loopManager.EndTurn(_turnsThisLoop);
                    turnAdded = true;
                    break;
                }
            }

            foreach (GameObject go in _folders)
            {
                Debug.Log(go);
                FolderScript f = go.GetComponent<FolderScript>().TryTeleport();
                if (f != null)
                {
                    Turn turn = new Turn
                    {
                        Position = _currentPosition,
                        tp = f
                    };
                    _turnsThisLoop.Add(turn);
                    _loopManager.EndTurn(_turnsThisLoop);
                    turnAdded = true;
                    break;
                }
            }

            if (!turnAdded)
            {
                BroadcastTurnEnded(_currentPosition);
            }
        }
        _animator.SetTrigger("idle");
        IsInteractable = true;
    }
    
    private void BroadcastTurnEnded(Vector2 currentPosition)
    {
        Turn turn = new Turn
        {
            Position = currentPosition,
        };
        _turnsThisLoop.Add(turn);

        if (popupOnTurns.Contains(_turnsThisLoop.Count))
        {
            _popupManager.SpawnPopup(popupType[_popupTypeIt]);
            _popupTypeIt++;
        }

        _loopManager.EndTurn(_turnsThisLoop);
    }
    
    public void Init(int[,] mapData, Vector2 startPosition)
    {
        _gridManager = LevelManager.Instance.GridManager;
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

        _loopManager = LevelManager.Instance.LoopManager;
        _exes = GameObject.FindGameObjectsWithTag("Exes");
        _folders = GameObject.FindGameObjectsWithTag("Folder");
    }

    private void OnMouseDown()
    {
        if (IsInteractable)
        {
            Debug.Log("GridManager", _gridManager);
            Debug.Log("_loopManager", _loopManager);
            _availableTiles = LevelManager.Instance.GridManager.GetReachableTiles(_currentPosition, LevelManager.Instance.LoopManager.curMaxTurns - GetCurrentTurn());
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
        _popupTypeIt = 0;
        _turnsThisLoop.Clear();
    }

    public int GetCurrentTurn()
    {
        return _turnsThisLoop.Count;
    }

    // We lock the Z coordinate to prevent accidentally change the Z coordinate of the main character
    // Otherwise it might break ray cast system.
    private void SetPositionWithLockedZ(Vector3 targetPos)
    {
        transform.position = new Vector3(targetPos.x, targetPos.y, -5);
    }

    public Vector2 GetCurrentPosition()
    {
        return _currentPosition;
    }

    public void AddTurn(Turn newTurn)
    {
        _turnsThisLoop.Add(newTurn);
    }

    public List<Turn> GetTurns()
    {
        return _turnsThisLoop;
    }
}
