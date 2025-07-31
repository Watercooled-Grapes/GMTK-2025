using System.Collections.Generic;
using UnityEngine;

public class LoopInstance : MonoBehaviour
{
    private List<Turn> _turns;
    private Vector2 _startPosition;
    private int _currentTurn;
    
    public void Init(List<Turn> turns, Vector2 startPosition)
    {
        _turns = turns;
        _startPosition = startPosition;
        Reset();
    }

    public void Reset()
    {
        transform.position = _startPosition;
        _currentTurn = 0;
    }
    
    public void ReplayNext()
    {
        if (_currentTurn >= _turns.Count) 
        {
            Debug.Log("Current turn out of bounds of turns");
            return;
        }
        // TODO: Implement throwing
        Turn turn = _turns[_currentTurn];
        transform.position = turn.Position;
        _currentTurn++;
    }
}
