using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class LoopInstance : MonoBehaviour
{
    private List<Turn> _turns;
    private Vector2 _startPosition;
    private int _currentTurn;
    public int LoopCreatedIn { get; private set; }

    private Animator _animator;


    public void Init(List<Turn> turns, Vector2 startPosition, int loopCreatedIn)
    {
        LoopCreatedIn = loopCreatedIn;
        _turns = turns;
        _startPosition = startPosition;
        Reset();

        _animator = GetComponent<Animator>();
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

        Turn turn = _turns[_currentTurn];
        StartCoroutine(MoveToTile(turn.Position));
        _currentTurn++;
    }

    private IEnumerator MoveToTile(Vector2 target)
    {
        if (transform.position.x < target.x)
        {
            _animator.SetTrigger("right");
        }
        else if (transform.position.x > target.x)
        {
            _animator.SetTrigger("left");
        }
        else if (transform.position.y < target.y)
        {
            _animator.SetTrigger("up");
        }
        else
        {
            _animator.SetTrigger("down");
        }

        while ((new Vector2(transform.position.x, transform.position.y) - target).sqrMagnitude > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, 5f * Time.deltaTime);
            yield return null;
        }

        _animator.SetTrigger("idle");
    }

    public Turn GetCurrentTurn()
    {
        return _turns[_currentTurn];
    }
}

