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
    [SerializeField] private AudioClip splosionSound;
    private Boolean done = false;


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
        done = false;
        transform.position = _startPosition;
        GetComponent<SpriteRenderer>().color = new Color32(255,255,255,40);
        _currentTurn = 0;
    }

    public void ReplayNext()
    {
        if (_currentTurn >= _turns.Count - 1)
        {
            if (!done) StartCoroutine(Explode());
            done = true;
        }

        Turn turn = _turns[_currentTurn];
        _currentTurn = Math.Min(_currentTurn + 1,  _turns.Count - 1);

        if (turn.TeleportToPos != null)
        {
            transform.position = turn.TeleportToPos.Value;
            _animator.SetTrigger("idle");
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
    }

    private IEnumerator Explode()
    {
        StartCoroutine(ChangeColorOverTime(
            GetComponent<SpriteRenderer>().color, 
            new Color(0, 1, 0, 20f), 
            0.75f));
        StartCoroutine(ShakeLeftRight(0.75f, 0.1f));
        yield return new WaitForSeconds(0.75f);
        GetComponent<SpriteRenderer>().color = new Color32(0,0,0,0);
        transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        GetComponent<AudioSource>().PlayOneShot(splosionSound);
    }

    private IEnumerator ChangeColorOverTime(Color startColor, Color targetColor, float duration)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float elapsedTime = 0;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration); // Normalized time (0-1)
            
            // Use Color.Lerp for smooth transition
            spriteRenderer.color = Color.Lerp(startColor, targetColor, t);
            
            yield return null;
        }
        
        // Ensure we end at exactly the target color
        spriteRenderer.color = targetColor;
    }

    private IEnumerator ShakeLeftRight(float duration, float intensity)
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            float x = originalPosition.x + UnityEngine.Random.Range(-1f, 1f) * intensity;
            
            // Only modify the x position to shake left/right
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // Restore original position
        transform.position = originalPosition;
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

    public Vector2 GetCurrentTilePosition()
    {
        if (0 < _currentTurn || _currentTurn >= _turns.Count)
        {
            Debug.LogWarning("_currentTurn went out of range again!");
        }
        Turn turn = _turns[Math.Min(_currentTurn, _turns.Count - 1)];
        return turn.Position;
    }
}
