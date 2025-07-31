using System.Collections.Generic;
using UnityEngine;

public class LoopManager : MonoBehaviour, MainCharacter.IObserver
{
    private List<LoopInstance> _loopInstances;
    [SerializeField] private int maxLoops;
    public int maxLoopTurns;
    private int _currentLoops;


    private LoopManager()
    {
        _loopInstances = new List<LoopInstance>();
    }
    

    public void Reset()
    {
        // Resets all loop data for the main character
        _loopInstances.Clear();
    }

    public void InstaniateLoopInstances()
    {
        foreach (var loopInstance in _loopInstances)
        {
            
        }
    }
    
    public void OnTurnEnd(Queue<Action> actions)
    {
        // Updates all clones to take their next step
        foreach (var loopInstance in _loopInstances)
        {
            loopInstance.ReplayNext();
        }

        if (_currentLoops >= maxLoops)
        {
            // TODO: Full Reset the level here @MinghaoLi
        } else if (actions.Count >= maxLoopTurns)
        {
            // Reached end of turn, save the actions made
            LoopInstance loopInstance = new LoopInstance(actions);
            _loopInstances.Add(loopInstance);
            LevelManager.RestartLevelWithLoop();
        }
    }
}