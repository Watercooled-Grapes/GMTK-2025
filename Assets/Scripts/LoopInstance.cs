using System.Collections.Generic;
using System.Collections;
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
        StartCoroutine(MoveToSquare(turn.Position));
        _currentTurn++;
    }

    private IEnumerator MoveToSquare(Vector2 target)
    {
        if (transform.position.x < target.x)
        {
            this.GetComponent<Animator>().SetTrigger("right");
        }
        else if (transform.position.x > target.x)
        {
            this.GetComponent<Animator>().SetTrigger("left");
        }
        else if (transform.position.y < target.y)
        {
            this.GetComponent<Animator>().SetTrigger("up");
        }
        else
        {
            this.GetComponent<Animator>().SetTrigger("down");
        }

        while ((new Vector2(transform.position.x, transform.position.y) - target).sqrMagnitude > 0.01f)
        {
            transform.position = Vector2.MoveTowards(transform.position, target, 5f * Time.deltaTime);
            yield return null;
        }

        this.GetComponent<Animator>().SetTrigger("idle");
    }
}

