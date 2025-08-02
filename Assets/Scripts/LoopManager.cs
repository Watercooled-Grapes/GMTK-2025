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
    public static int CurrentLoops = 0;
    [SerializeField] private GameObject _clonePrefab;
    private CodeLineManager _codeLineManager;
    private InfoTextManager _infoTextManager;
    private int _tilesToMove;
    public int tilesToMove {
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
        foreach (var loopInstance in _loopInstances)
        {
            loopInstance.SetActive(true);
            loopInstance.GetComponent<LoopInstance>().Reset();
        }
    }

    IEnumerator RestartLevelWithLoop(float delayTime, List<Turn> turns, LevelManager levelManager)
    {
        curMaxTurns = maxTurns;

        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);
        //Do the action after the delay time has finished.
        // 1) nothing is moving 2) we are not at goal 3) we HAVE loops available
        GameObject clone = Instantiate(_clonePrefab, Vector2.zero, Quaternion.identity);
        clone.SetActive(false);
        clone.GetComponent<LoopInstance>().Init(new List<Turn>(turns), levelManager.StartPosition, CurrentLoops);
        _loopInstances.Add(clone);
        _codeLineManager.UpdateCode(1);
        levelManager.ResumeLevel();

        
        levelManager.RestartLevelWithLoop();
        CurrentLoops++;
        _infoTextManager.UpdateTurnLoopInfo(maxTurns, maxLoops - CurrentLoops);

        RestartLevelIfNecessary();
    }

    private void RestartLevelIfNecessary()
    {
        if (CurrentLoops >= maxLoops && GameManager.Instance.CurrentState == GameState.PLAYING)
        {
            GameManager.Instance.LoadNextLevelWithCutscene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    private void InvokeCallbacksForPosition(Vector2 pos, int loopIndex)
    {
        if (!TriggerableCallbacks.ContainsKey(pos)) {
            return;
        }
        foreach (var callback in TriggerableCallbacks[pos])
        {
            callback.Invoke(loopIndex);
        }
    }
    
    public void EndTurn(List<Turn> turns)
    {
        Turn currentTurn = turns[turns.Count - 1];

        // The main character
        InvokeCallbacksForPosition(currentTurn.Position, -1);
        
        // Complete the turn and update all clones to take their next step
        foreach (var loopInstanceObj in _loopInstances)
        {
            LoopInstance loopInstance = loopInstanceObj.GetComponent<LoopInstance>();
            loopInstance.ReplayNext();
            InvokeCallbacksForPosition(currentTurn.Position, loopInstance.LoopCreatedIn);
        }
        _codeLineManager.UpdateCode(turns.Count + 1);
        _infoTextManager.UpdateTurnLoopInfo(curMaxTurns - turns.Count, maxLoops - CurrentLoops);
        // if no more turns can be made, restart the loop
        if (turns.Count >= curMaxTurns)
        {
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            levelManager.PauseLevel();
            StartCoroutine(RestartLevelWithLoop(1, turns, levelManager));
        }
    }

    public void addTurns(int n)
    {
        curMaxTurns += n;
        _codeLineManager.addLines(n);
        _infoTextManager.UpdateTurnLoopInfo(curMaxTurns - LevelManager.Instance.MainCharacter.GetCurrentTurn(), maxLoops - CurrentLoops);
    }

    public void addLoops(int n)
    {
        maxLoops += n;
        _infoTextManager.UpdateTurnLoopInfo(curMaxTurns - LevelManager.Instance.MainCharacter.GetCurrentTurn(), maxLoops - CurrentLoops);

    }

    public bool HasTurnsRemaining()
    {
        return curMaxTurns > LevelManager.Instance.MainCharacter.GetCurrentTurn();
    }
}
