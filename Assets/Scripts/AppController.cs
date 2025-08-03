using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class AppController : MonoBehaviour
{
    public static int APP_DELETE_TURNS_COST = 3;
    [SerializeField] private ParticleSystem _particleSystem;
    private int _loopDestroyedIn = -1;
    [SerializeField] private int loopsToAddOnDestroy = 1;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Vector2 _pos;
    [SerializeField] private AudioClip explosionSound;
    
    private bool _consumed = false;
    private Tile _tile;
    private SpriteRenderer _renderer;
    private TextMeshPro _infoText;

    private LoopManager _loopManager;

    public bool IsConsumed => _consumed;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.enabled = true;
        _renderer.sprite = sprites[Random.Range(0, sprites.Length)];

        _infoText = gameObject.transform.GetChild(0).gameObject.GetComponent<TextMeshPro>();
        _infoText.text = $"+{loopsToAddOnDestroy} loops\n -{APP_DELETE_TURNS_COST} turns";
    }

    public void Init()
    {
        _loopManager = LevelManager.Instance.LoopManager;
        transform.position = LevelManager.Instance.GridManager.GetTileCenterPosition(_pos);
        _loopManager.RegisterTriggerableCallback(_pos, Trigger);

        _tile = LevelManager.Instance.GridManager.GetTileAtPosition(_pos);

        GetComponent<Float>().Init();
        LevelManager.Instance.GridManager.RegisterAppController(_pos, this);
        _tile.hoverEnter += onHoverEnter;
        _tile.hoverExit += onHoverExit;
    }
    
    IEnumerator DelayedDestroy(float delayTime)
    {
        _renderer.enabled = false;
        _particleSystem.Play();
        GetComponent<AudioSource>().PlayOneShot(explosionSound);
        yield return new WaitForSeconds(delayTime);
        
        _tile.IsOccupied = false;
    }
    
    public void Trigger(int loopIndex)
    {
        int currentLoops = LevelManager.Instance.LoopManager.CurrentLoops;
        if (!_consumed && loopIndex == currentLoops)
        {
            // Consume it only when this is a main character
            for (int i = 0; i < AppController.APP_DELETE_TURNS_COST; i++)
            {
                if (LevelManager.Instance.LoopManager.HasTurnsRemaining())
                {
                    Turn turn = new Turn
                    {
                        Position = LevelManager.Instance.MainCharacter.GetCurrentPosition(),
                        Tile = _tile,
                        TeleportToPos = null,
                    };
                    LevelManager.Instance.MainCharacter.AddTurn(turn);
                    LevelManager.Instance.LoopManager.EndTurn(LevelManager.Instance.MainCharacter.GetTurns(), false);
                } else {
                    break;
                }
            }

            LevelManager.Instance.LoopManager.AddLoops(loopsToAddOnDestroy);

            _consumed = true;
            _tile.IsOccupied = false;

            _loopDestroyedIn = currentLoops;
            RunDestroySequence();
        } else if (!_consumed && _loopDestroyedIn == loopIndex)
        {
            // If the clone consumes it, make the tile accessible for the main character.
            _tile.IsOccupied = false;
            RunDestroySequence();
            _consumed = true;
        }
    }

    private void RunDestroySequence()
    {
        CinemachineImpulseSource cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        cinemachineImpulseSource.GenerateImpulse();

        StartCoroutine(DelayedDestroy(0.5f));
    }

    public void OnResetForLoop(int[,] mapData, Vector2 pos)
    {
        if (_consumed)
        {
            Color color = _renderer.color;
            color.a = 0.3f;
            _renderer.color = color;
        }
        _renderer.enabled = true;

        _tile.IsOccupied = _consumed;

        _consumed = false;
    }

    void onHoverEnter()
    {
        if (_loopDestroyedIn == -1)
        {
            _infoText.enabled = true;
        }
    }

    void onHoverExit()
    {
        if (_loopDestroyedIn == -1)
        {
            _infoText.enabled = false;
        }
    }
}
