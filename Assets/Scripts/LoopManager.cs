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
    [SerializeField] private AudioClip[] song1Layers;
    [SerializeField] private AudioClip[] song2Layers;
    [SerializeField] private AudioClip[] song3Layers;
    private int _selectedSong;
    private AudioSource _audioSource;
    private CodeLineManager _codeLineManager;
    private InfoTextManager _infoTextManager;
    private int _tilesToMove;
    private bool _isRestarting = false;
    public bool _isWinning = false;
    private float songTime = 0f;
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

    void Update()
    {
        songTime += Time.deltaTime;  
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
        _audioSource = GetComponent<AudioSource>();

        _selectedSong = UnityEngine.Random.Range(0, 3);

        switch (_selectedSong)
            {
                case (0):
                    _audioSource.resource = song1Layers[0];
                    _audioSource.Play();
                    break;
                case (1):
                    _audioSource.resource = song2Layers[0];
                    _audioSource.Play();
                    break;
                case (2):
                    _audioSource.resource = song3Layers[0];
                    _audioSource.Play();
                    break;
            }
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
        if (CurrentLoops < 4)
        {
            switch (_selectedSong)
            {
                case (0):
                    _audioSource.resource = song1Layers[CurrentLoops];
                    _audioSource.time = songTime;
                    _audioSource.Play();
                    break;
                case (1):
                    _audioSource.resource = song2Layers[CurrentLoops];
                    _audioSource.time = songTime;
                    _audioSource.Play();
                    break;
                case (2):
                    _audioSource.resource = song3Layers[CurrentLoops];
                    _audioSource.time = songTime;
                    _audioSource.Play();
                    break;
            }
            songTime = 0;
        }
        foreach (var loopInstance in _loopInstances)
        {
            loopInstance.SetActive(true);
            loopInstance.GetComponent<LoopInstance>().Reset();
        }
    }

    IEnumerator RestartLevelWithLoop(float delayTime, List<Turn> turns, int currentLoop)
    {
        curMaxTurns = maxTurns;

        // Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);
        yield return StartCoroutine(PlayRewindCoroutine());

        //Do the action after the delay time has finished.
        // 1) nothing is moving 2) we are not at goal 3) we HAVE loops available
        GameObject clone = Instantiate(_clonePrefab, Vector2.zero, Quaternion.identity);
        clone.SetActive(false);
        clone.GetComponent<LoopInstance>().Init(turns, LevelManager.Instance.StartPosition, currentLoop);
        _loopInstances.Add(clone);
        _codeLineManager.UpdateCode(1);
        LevelManager.Instance.ResumeLevel();
        
        RestartLevelIfNecessary(currentLoop);
        
        LevelManager.Instance.RestartLevelWithLoop();
        _infoTextManager.UpdateTurnLoopInfo(maxTurns, maxLoops - currentLoop);
    }

    private void RestartLevelIfNecessary(int currentLoop)
    {
        if (currentLoop+1 > maxLoops && GameManager.Instance.CurrentState == GameState.PLAYING && !_isWinning)
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

    public void EndTurn(List<Turn> turns, bool appTurn=false, bool emitMessage = true)
    {
        Turn currentTurn = turns[turns.Count - 1];
        int currentLoop = CurrentLoops;
        // The main character
        if (emitMessage) InvokeCallbacksForPosition(currentTurn.Position, currentLoop);

        // Complete the turn and update all clones to take their next step
        Debug.Log("Number of clones " + _loopInstances.Count);
        foreach (var loopInstanceObj in _loopInstances)
        {
            LoopInstance loopInstance = loopInstanceObj.GetComponent<LoopInstance>();
            if (emitMessage) InvokeCallbacksForPosition(loopInstance.GetCurrentTilePosition(), loopInstance.LoopCreatedIn);
            loopInstance.ReplayNext(appTurn);
        }

        _codeLineManager.UpdateCode(turns.Count + 1);
        _infoTextManager.UpdateTurnLoopInfo(curMaxTurns - turns.Count, maxLoops - currentLoop);

        // if no more turns can be made, restart the loop
        if (turns.Count >= curMaxTurns && !_isRestarting)
        {
            _isRestarting = true;
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            levelManager.PauseLevel();
            CurrentLoops++;
            StartCoroutine(RestartLevelWithLoop(1, turns, currentLoop));
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
        Debug.Log("rewinding");
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
