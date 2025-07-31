using System.Collections.Generic;
using UnityEngine;

public class LoopManager : MonoBehaviour, MainCharacter.IObserver
{
    private List<GameObject> _loopInstances;
    [SerializeField] private int maxLoops;
    public int maxTurns;
    private int _currentLoops;
    [SerializeField] private GameObject _clonePrefab;
    
    private LoopManager()
    {
        _loopInstances = new List<GameObject>();
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
            loopInstance.GetComponent<LoopInstance>().Reset();
        }
    }
    
    public void OnTurnEnd(List<Turn> turns)
    {
        // Updates all clones to take their next step
        foreach (var loopInstance in _loopInstances)
        {
            Debug.Log("replaying");

            loopInstance.GetComponent<LoopInstance>().ReplayNext();
        }

        Debug.Log(turns.Count);
        if (_currentLoops >= maxLoops)
        {
            Debug.Log("full resetting");
            // TODO: Full Reset the level here @MinghaoLi
            // 1) nothing is moving 2) we are not at goal 3) no more loops available
        } else if (turns.Count >= maxTurns)
        {   
            Debug.Log("resetting turn");
            // 1) nothing is moving 2) we are not at goal 3) we HAVE loops available
            GameObject clone = Instantiate(_clonePrefab, Vector2.zero, Quaternion.identity);
            clone.SetActive(false);
            _loopInstances.Add(clone);
            
            FindFirstObjectByType<LevelManager>().RestartLevelWithLoop();
        }
    }
}