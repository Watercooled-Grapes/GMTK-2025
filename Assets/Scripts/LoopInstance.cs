using System.Collections.Generic;
using UnityEngine;

public class LoopInstance
{
    private Queue<Action> _actions;

    public LoopInstance(Queue<Action> actions)
    {
        _actions = actions;
    }
    
    public void ReplayNext()
    {
        Action action = _actions.Dequeue();
    }
}
