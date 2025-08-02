using System.Collections.Generic;
using UnityEngine;

public class GateScript : MonoBehaviour
{
    [SerializeField] private List<LeverScript> _requiredLevers;
    [SerializeField] private Vector2 _pos;
    private bool _isOpen = false;

    private SpriteRenderer _spriteRenderer;

    [SerializeField]
    private Color _closedColor = Color.red;
    [SerializeField]
    private Color _openColor = Color.green;

    public void Init() {
        transform.position = LevelManager.Instance.GridManager.GetTileCenterPosition(_pos);

        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _closedColor;
        }
    }

    public void NotifyLeverStateChanged(LeverScript changedLever)
    {
        if (_isOpen) return;

        foreach (var lever in _requiredLevers)
        {
            if (!lever.IsTriggered)
            {
                Debug.Log($"{name} is still locked (waiting for all levers)");
                return;
            }
        }

        OpenGate();
    }

    private void OpenGate()
    {
        _isOpen = true;
        Debug.Log($"{name} is now OPEN!");

        // You can disable the collider or play animation/sound here
        // TODO: open gate logic goes here.
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = _openColor;
        }
    }

    public void Reset()
    {
        _isOpen = false;
        Debug.Log($"{name} is now CLOSED!");
    }

    public void OnResetForLoop(int[,] mapData, Vector2 pos) {
        Reset();
    }
}
