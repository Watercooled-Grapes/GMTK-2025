using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class LoopInstance : MonoBehaviour
{
    private List<Turn> _turns;
    private Vector2 _startPosition;
    private int _currentTurn;
    public int LoopCreatedIn { get; private set; }
    public int tilesToMove;

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

        if (turn.exe != null)
        {
            turn.exe.ghostCollect();
        }

        if (turn.tp != null)
        {
            transform.position = new Vector2(turn.Position.x, turn.Position.y);
            _currentTurn++;
            return;
        }

        float deltaX = turn.Position.x - transform.position.x;
        float deltaY = turn.Position.y - transform.position.y;

        if (Math.Abs(deltaX) > Math.Abs(deltaY) && deltaX > 0)
        {
            _animator.SetTrigger("right");
        }
        else if (Math.Abs(deltaX) > Math.Abs(deltaY) && deltaX < 0)
        {
            _animator.SetTrigger("left");
        }
        else if (Math.Abs(deltaY) > Math.Abs(deltaX) && deltaY > 0)
        {
            _animator.SetTrigger("up");
        }
        else
        {
            _animator.SetTrigger("down");
        }
        StartCoroutine(MoveToTile(turn.Position));
        _currentTurn++;
    }

    private IEnumerator MoveToTile(Vector2 target)
    {
        while ((new Vector2(transform.position.x, transform.position.y) - target).sqrMagnitude > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, 5f * Time.deltaTime);
            yield return null;
        }
        tilesToMove--;
        if (tilesToMove <= 0)
        {
            _animator.SetTrigger("idle");
        }   
    }
}

