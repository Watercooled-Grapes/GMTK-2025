using UnityEngine;

public class ExeScript : MonoBehaviour
{
    [SerializeField] private Vector2 _pos;
    [SerializeField] private int _turnsToAdd;

    private MainCharacter _player;
    private GridManager _gridManager;
    private LoopManager _loopManager;
    private bool _collectable = true;

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
        Vector3 pos = _gridManager.GetTileCenterPosition(_pos);
        transform.position = pos;

        _loopManager.RegisterTriggerableCallback(_pos, TryCollect);
    }

    public void TryCollect(int loopIndex)
    {
        if (_collectable)
        {
            _loopManager.addTurns(_turnsToAdd);
            GetComponent<SpriteRenderer>().enabled = false;
            _collectable = false;
        }
    }

    public void OnResetForLoop(int[,] mapData, Vector2 startPosition)
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void CollectByClone()
    {
        _loopManager.addTurns(_turnsToAdd);
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
