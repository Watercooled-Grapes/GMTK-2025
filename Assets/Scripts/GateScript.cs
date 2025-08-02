using System.Collections.Generic;
using UnityEngine;

public class GateScript : MonoBehaviour
{
    [SerializeField] private List<LeverScript> _requiredLevers;
    [SerializeField] private Vector2 _pos;
    private bool _isOpen = false;
    private Tile _tile;

    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private Color _closedColor = Color.red;
    [SerializeField]
    private Color _openColor = Color.green;

    public void Init() {
        _tile =  LevelManager.Instance.GridManager.GetTileAtPosition(_pos);
        transform.position = LevelManager.Instance.GridManager.GetTileCenterPosition(_tile);

        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _closedColor;
        }

        _tile.IsOccupied = true;
    }

    public void NotifyLeverStateChanged(LeverScript changedLever)
    {
        if (_isOpen) return;

        foreach (var lever in _requiredLevers)
        {
            if (!lever.IsTriggered)
            {
                return;
            }
        }

        OpenGate();
    }

    private void OpenGate()
    {
        _isOpen = true;
        Debug.Log($"{name} is now OPEN!");

        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _openColor;
        }
        _tile.IsOccupied = false;
    }

    public void Reset()
    {
        _isOpen = false;
        _tile.IsOccupied = true;
        _spriteRenderer.color = _closedColor;
    }

    public void OnResetForLoop(int[,] mapData, Vector2 pos) {
        Reset();
    }
}
