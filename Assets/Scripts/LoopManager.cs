using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoopManager : MonoBehaviour
{
    private List<GameObject> _loopInstances;
    [SerializeField] private int maxLoops;
    public int maxTurns;
    public int curMaxTurns;
    public int CurrentLoops { get; private set; } = 0;
    [SerializeField] private GameObject _clonePrefab;
    private CodeLineManager _codeLineManager;
    private InfoTextManager _infoTextManager;
    private int _tilesToMove;
    private bool _isRestarting = false;
    public bool _isWinning = false;
    public int tilesToMove
    {
        get
        {
            return _tilesToMove;
        }
        set
        {
            _tilesToMove = value;
            foreach (var loopInstance in _loopInstances)
            {
                loopInstance.GetComponent<LoopInstance>().tilesToMove = tilesToMove;
            }
        }
    }

    // Broadcast the signal when moving onto the tile position
    public Dictionary<Vector2, List<Action<int>>> TriggerableCallbacks = new();

    private LoopManager()
    {
        _loopInstances = new List<GameObject>();
    }

    public void Init()
    {
        curMaxTurns = maxTurns;
        _codeLineManager = FindFirstObjectByType<CodeLineManager>();
        _codeLineManager.Init(maxTurns);
        _infoTextManager = FindFirstObjectByType<InfoTextManager>();
        _infoTextManager.UpdateTurnLoopInfo(maxTurns, maxLoops - CurrentLoops);
    }

    public void RegisterTriggerableCallback(Vector2 _pos, Action<int> callback)
    {
        if (!TriggerableCallbacks.ContainsKey(_pos))
        {
            TriggerableCallbacks[_pos] = new List<Action<int>>();
        }

        TriggerableCallbacks[_pos].Add(callback);
    }

    public void Reset()
    {
        // Resets all loop data for the main character
        _loopInstances.Clear();
    }

    public void InitLoopInstances()
    {
        _isRestarting = false;
        foreach (var loopInstance in _loopInstances)
        {
            loopInstance.SetActive(true);
            loopInstance.GetComponent<LoopInstance>().Reset();
        }
    }

    IEnumerator RestartLevelWithLoop(float delayTime, List<Turn> turns)
    {
        curMaxTurns = maxTurns;

        // Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);
        yield return StartCoroutine(PlayRewindCoroutine());

        //Do the action after the delay time has finished.
        // 1) nothing is moving 2) we are not at goal 3) we HAVE loops available
        GameObject clone = Instantiate(_clonePrefab, Vector2.zero, Quaternion.identity);
        clone.SetActive(false);
        clone.GetComponent<LoopInstance>().Init(turns, LevelManager.Instance.StartPosition, CurrentLoops);
        _loopInstances.Add(clone);
        _codeLineManager.UpdateCode(1);
        LevelManager.Instance.ResumeLevel();


        LevelManager.Instance.RestartLevelWithLoop();
        CurrentLoops++;
        _infoTextManager.UpdateTurnLoopInfo(maxTurns, maxLoops - CurrentLoops);

        RestartLevelIfNecessary();
    }

    private void RestartLevelIfNecessary()
    {
        if (CurrentLoops > maxLoops && GameManager.Instance.CurrentState == GameState.PLAYING && !_isWinning)
        {
            GameManager.Instance.RestartLevel();
        }
    }

    private void InvokeCallbacksForPosition(Vector2 pos, int loopIndex)
    {
        if (!TriggerableCallbacks.ContainsKey(pos))
        {
            return;
        }
        foreach (var callback in TriggerableCallbacks[pos])
        {
            callback.Invoke(loopIndex);
        }
    }

    public void EndTurn(List<Turn> turns, bool emitMessage = true)
    {
        Turn currentTurn = turns[turns.Count - 1];

        // The main character
        if (emitMessage) InvokeCallbacksForPosition(currentTurn.Position, CurrentLoops);

        // Complete the turn and update all clones to take their next step
        Debug.Log("Number of clones " + _loopInstances.Count);
        foreach (var loopInstanceObj in _loopInstances)
        {
            LoopInstance loopInstance = loopInstanceObj.GetComponent<LoopInstance>();
            if (emitMessage) InvokeCallbacksForPosition(loopInstance.GetCurrentTilePosition(), loopInstance.LoopCreatedIn);
            loopInstance.ReplayNext();
        }

        _codeLineManager.UpdateCode(turns.Count + 1);
        _infoTextManager.UpdateTurnLoopInfo(curMaxTurns - turns.Count, maxLoops - CurrentLoops);

        // if no more turns can be made, restart the loop
        if (turns.Count >= curMaxTurns && !_isRestarting)
        {
            _isRestarting = true;
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            levelManager.PauseLevel();
            StartCoroutine(RestartLevelWithLoop(1, turns));
        }
    }

    public void AddTurns(int n)
    {
        curMaxTurns += n;
        _codeLineManager.addLines(n);
        _infoTextManager.UpdateTurnLoopInfo(curMaxTurns - LevelManager.Instance.MainCharacter.GetCurrentTurn(), maxLoops - CurrentLoops);
    }

    public void AddLoops(int n)
    {
        maxLoops += n;
        _infoTextManager.UpdateTurnLoopInfo(curMaxTurns - LevelManager.Instance.MainCharacter.GetCurrentTurn(), maxLoops - CurrentLoops);
    }

    public bool HasTurnsRemaining()
    {
        return curMaxTurns > LevelManager.Instance.MainCharacter.GetCurrentTurn();
    }

    // We rewind main character and the clones all the together. 
    private IEnumerator PlayRewindCoroutine()
    {
        LevelManager.Instance.MainCharacter.IsInteractable = false;

        int coroutinesRunning = 1;

        StartCoroutine(RunAndNotifyDone(LevelManager.Instance.MainCharacter.RewindVisual(), () => coroutinesRunning--));

        foreach (var loopInstanceObj in _loopInstances)
        {
            coroutinesRunning++;
            var loopInstance = loopInstanceObj.GetComponent<LoopInstance>();
            StartCoroutine(RunAndNotifyDone(loopInstance.RewindVisual(), () => coroutinesRunning--));
        }

        yield return new WaitUntil(() => coroutinesRunning == 0);

        LevelManager.Instance.MainCharacter.IsInteractable = true;
        LevelManager.Instance.ResumeLevel();
    }

    private IEnumerator RunAndNotifyDone(IEnumerator routine, Action onDone)
    {
        yield return StartCoroutine(routine);
        onDone?.Invoke();
    }
    
    public bool IsDeathLoop()
    {
        return CurrentLoops >= maxLoops;
    }
}
