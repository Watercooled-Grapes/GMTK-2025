using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private MainCharacter _mainCharacter;
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

    private LoopManager()
    {
        _loopInstances = new List<GameObject>();
    }

    public void Init(MainCharacter mainCharacter)
    {
        curMaxTurns = maxTurns;
        _codeLineManager = FindFirstObjectByType<CodeLineManager>();
        _codeLineManager.Init(maxTurns);
        _infoTextManager = FindFirstObjectByType<InfoTextManager>();
        _infoTextManager.UpdateTurnLoopInfo(maxTurns, maxLoops - CurrentLoops);
        _mainCharacter = mainCharacter;
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
        if (CurrentLoops >= maxLoops)
        {
            // TODO: Full Reset the level here @MinghaoLi
            Debug.Log("####FAILED LEVEL####");
        }
    }
    
    /**
     * Returns false when no further turns can be made, true otherwise
     */
    public void EndTurn(List<Turn> turns)
    {
        // Complete the turn and update all clones to take their next step
        foreach (var loopInstance in _loopInstances)
        {
            loopInstance.GetComponent<LoopInstance>().ReplayNext();
        }
        
        // if no more turns can be made, Restart the loop
        if (turns.Count >= curMaxTurns)
        {
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            levelManager.PauseLevel();
            StartCoroutine(RestartLevelWithLoop(1, turns, levelManager));
        }
        
        _codeLineManager.UpdateCode(turns.Count + 1);
        _infoTextManager.UpdateTurnLoopInfo(curMaxTurns - turns.Count, maxLoops - CurrentLoops);
    }

    public void addTurns(int n)
    {
        curMaxTurns += n;
        _codeLineManager.addLines(n);
        _infoTextManager.UpdateTurnLoopInfo(curMaxTurns - _mainCharacter.GetCurrentTurn(), maxLoops - CurrentLoops);
    }

    public void addLoops(int n)
    {
        maxLoops += n;
        _infoTextManager.UpdateTurnLoopInfo(curMaxTurns - _mainCharacter.GetCurrentTurn(), maxLoops - CurrentLoops);

    }

    public bool HasTurnsRemaining()
    {
        return curMaxTurns > _mainCharacter.GetCurrentTurn();
    }
}
