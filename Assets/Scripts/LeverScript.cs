using System.Collections.Generic;
using UnityEngine;

public class LeverScript : MonoBehaviour
{
    [SerializeField] private bool _isTriggered = false;
    [SerializeField] private Vector2 _pos;
    [SerializeField] private List<GateScript> _connectedGates;

    private Animator _animator;

    public bool IsTriggered => _isTriggered;

    public void Init() {
        _animator = GetComponent<Animator>();
        transform.position = LevelManager.Instance.GridManager.GetTileCenterPosition(_pos);

        LevelManager.Instance.LoopManager.RegisterTriggerableCallback(_pos, (_) => Trigger());
    }

    public void Trigger()
    {
        if (_isTriggered) return;

        _isTriggered = true;
        Debug.Log($"{name} was triggered!");

         _animator?.SetTrigger("Trigger");

        foreach (var gate in _connectedGates)
        {
            gate.NotifyLeverStateChanged(this);
        }
    }

    public void Reset()
    {
        _isTriggered = false;
        foreach (var gate in _connectedGates)
        {
            gate.NotifyLeverStateChanged(this);
        }
    }

    public void OnResetForLoop(int[,] mapData, Vector2 pos)
    {
        Reset();
    }
}
