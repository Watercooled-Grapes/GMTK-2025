using UnityEngine;

public class ExeScript : MonoBehaviour
{
    [SerializeField] private Vector2 _pos;
    [SerializeField] private int turnsToAdd;

    private MainCharacter _player;
    private GridManager _gridManager;
    private LoopManager _loopManager;
    private bool canBeCollected = true;

    public void Start()
    {
        _player = FindFirstObjectByType<MainCharacter>();
        _loopManager = FindFirstObjectByType<LoopManager>();
    }
    public void Init(int[,] mapData)
    {
        _gridManager = FindFirstObjectByType<GridManager>();
        if (_gridManager == null)
        {
            Debug.LogError("GridManager not found!");
            return;
        }
        Vector3 pos = _gridManager.GetTileCenterPosition(_pos);
        transform.position = pos;
    }

    public ExeScript tryCollect()
    {
        if (canBeCollected && _player._currentPosition == _pos)
        {
            Debug.Log("collect exe");
            _loopManager.addTurns(turnsToAdd);
            GetComponent<SpriteRenderer>().enabled = false;
            canBeCollected = false;
            return this;
        }
        return null;
    }

    public void OnResetForLoop(int[,] mapData, Vector2 startPosition)
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }

    public void ghostCollect()
    {
        _loopManager.addTurns(turnsToAdd);
        GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
