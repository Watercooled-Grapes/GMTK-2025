using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class AppController : MonoBehaviour
{
    public static int APP_DELETE_TURNS_COST = 3;
    [SerializeField] private ParticleSystem _particleSystem;
    private int _loopDestroyedIn;
    [SerializeField] private int loopsToAddOnDestroy = 1;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Vector2 _pos;
    [SerializeField] private AudioClip explosionSound;
    
    private bool _consumedOnce = false;
    private Tile _tile;
    private SpriteRenderer _renderer;

    private LoopManager _loopManager;

    void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _renderer.enabled = true;
        _renderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }

    public void Init()
    {
        _loopManager = LevelManager.Instance.LoopManager;
        transform.position = LevelManager.Instance.GridManager.GetTileCenterPosition(_pos);
        _loopManager.RegisterTriggerableCallback(_pos, Trigger);

        _tile = LevelManager.Instance.GridManager.GetTileAtPosition(_pos);

        GetComponent<Float>().Init();
        LevelManager.Instance.GridManager.RegisterAppController(_pos);
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
        if (!_consumedOnce && loopIndex == currentLoops)
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

            _consumedOnce = true;

            _loopDestroyedIn = currentLoops;
            RunDestroySequence();
        } else if (!_consumedOnce && _loopDestroyedIn == loopIndex)
        {
            // If the clone consumes it, make the tile accessible for the main character.
            _tile.IsOccupied = false;
            RunDestroySequence();
            _consumedOnce = true;
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
        if (_consumedOnce)
        {
            Color color = _renderer.color;
            color.a = 0.3f;
            _renderer.color = color;
        }
        _renderer.enabled = true;

        _tile.IsOccupied = _consumedOnce;

        _consumedOnce = false;
    }
}
