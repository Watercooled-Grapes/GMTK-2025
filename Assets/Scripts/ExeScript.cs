using TMPro;
using UnityEngine;
using static GridManager;

public class ExeScript : MonoBehaviour
{
    [SerializeField] private Vector2 _pos;
    [SerializeField] private int _turnsToAdd;

    private MainCharacter _player;
    private GridManager _gridManager;
    private LoopManager _loopManager;
    private bool _collected = false;
    private int _collectedIn;
    private Tile _tile;
    private TextMeshPro _infoText;

    private SpriteRenderer _renderer;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _infoText = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        _infoText.text = $"+{_turnsToAdd} turns";
    }

    public void Init(int[,] mapData)
    {
        _player = LevelManager.Instance.MainCharacter;
        _loopManager = LevelManager.Instance.LoopManager;
        _gridManager = LevelManager.Instance.GridManager;
        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }
        _tile = _gridManager.GetTileAtPosition(_pos);
        Vector3 pos = _gridManager.GetTileCenterPosition(_tile);
        transform.position = pos;

        _loopManager.RegisterTriggerableCallback(_pos, TryCollect);

        GetComponent<Float>().Init();
        _tile.hoverEnter += onHoverEnter;
        _tile.hoverExit += onHoverExit;
    }

    public void TryCollect(int loopIndex)
    {
        // Not collected ever since level started OR
        // the one that triggers this is the clone that has previously collected it
        if (!_collected || (loopIndex != _loopManager.CurrentLoops && _collectedIn == loopIndex))
        {
            _loopManager.AddTurns(_turnsToAdd);
            _renderer.enabled = false;
            _collected = true;
            _collectedIn = loopIndex;
        }
    }

    public void OnResetForLoop(int[,] mapData, Vector2 startPosition)
    {
       _renderer.enabled = true;
        if (_collected)
        {
            Color color = _renderer.color;
            color.a = 0.3f;
            _renderer.color = color;
        }
    }

    public void CollectByClone()
    {
        _loopManager.AddTurns(_turnsToAdd);
        _renderer.enabled = false;
    }

    void onHoverEnter()
    {
        if (_collected)
        {
            return;
        }
        
        _infoText.enabled = true;
    }

    void onHoverExit()
    {
        if (_collected)
        {
            return;
        }

        _infoText.enabled = false;
    }
}
