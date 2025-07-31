using System.Collections.Generic;
using UnityEngine;

public class LoopInstance : MonoBehaviour
{
    private List<Turn> _turns;
    private int _currentTurn;

    public LoopInstance(List<Turn> turns)
    {
        _turns = turns;
    }

    public void Reset()
    {
        // TODO: Move to start
        if (_turns != null && _turns.Count > 0)
        {
            transform.position = _turns[0].Position;
            _currentTurn++;
        }
    }
    
    public void ReplayNext()
    {
        if (_currentTurn >= _turns.Count)
            Debug.Log("Current turn out of bounds of turns");
        Turn turn = _turns[_currentTurn];
        transform.position = turn.Position;
        _currentTurn++;
    }
}
