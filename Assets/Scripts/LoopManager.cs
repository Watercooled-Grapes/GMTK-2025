using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopManager : MonoBehaviour
{
    private List<GameObject> _loopInstances;
    [SerializeField] private int maxLoops;
    public int maxTurns;
    private int _currentLoops;
    [SerializeField] private GameObject _clonePrefab;
    private CodeLineManager _codeLineManager;
    private InfoTextManager _infoTextManager;

    private LoopManager()
    {
        _loopInstances = new List<GameObject>();
    }

    public void Init()
    {
        _codeLineManager = FindFirstObjectByType<CodeLineManager>();
        _codeLineManager.Init(maxTurns);
        _infoTextManager = FindFirstObjectByType<InfoTextManager>();
        _infoTextManager.UpdateTurnLoopInfo(maxTurns, maxLoops - _currentLoops);
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


        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);
        //Do the action after the delay time has finished.
        // 1) nothing is moving 2) we are not at goal 3) we HAVE loops available
        GameObject clone = Instantiate(_clonePrefab, Vector2.zero, Quaternion.identity);
        clone.SetActive(false);
        clone.GetComponent<LoopInstance>().Init(new List<Turn>(turns), levelManager.StartPosition);
        _loopInstances.Add(clone);
        _codeLineManager.UpdateCode(1);

        levelManager.RestartLevelWithLoop();
        _currentLoops++;
        _infoTextManager.UpdateTurnLoopInfo(maxTurns, maxLoops - _currentLoops);
        levelManager.ResumeLevel();
    }

    public void OnTurnEnd(List<Turn> turns)
    {
        // Updates all clones to take their next step
        foreach (var loopInstance in _loopInstances)
        {
            loopInstance.GetComponent<LoopInstance>().ReplayNext();
        }

        _codeLineManager.UpdateCode(turns.Count + 1);
        _infoTextManager.UpdateTurnLoopInfo(maxTurns - turns.Count, maxLoops - _currentLoops);

        if (_currentLoops >= maxLoops)
        {
            // TODO: Full Reset the level here @MinghaoLi
            // 1) nothing is moving 2) we are not at goal 3) no more loops available
        }
        else if (turns.Count >= maxTurns)
        {
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            levelManager.PauseLevel();
            StartCoroutine(RestartLevelWithLoop(1, turns, levelManager));
        }
    }
}
