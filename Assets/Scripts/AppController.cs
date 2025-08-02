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
    private bool _consumed = false;
    [SerializeField] private AudioClip explosionSound;
    

    private LoopManager _loopManager;

    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }

    public void Init()
    {
        gameObject.SetActive(true);
        GetComponent<Renderer>().enabled = true;
        _loopManager = LevelManager.Instance.LoopManager;
        transform.position = LevelManager.Instance.GridManager.GetTileCenterPosition(_pos);
        _loopManager.RegisterTriggerableCallback(_pos, Trigger);
    }
    
    IEnumerator DelayedDestroy(float delayTime)
    {
        GetComponent<Renderer>().enabled = false;
        _particleSystem.Play();
        GetComponent<AudioSource>().PlayOneShot(explosionSound);
        yield return new WaitForSeconds(delayTime);
        
        gameObject.SetActive(false);
    }
    
    public void Trigger(int loopCreatedIn)
    {
        if (!_consumed && loopCreatedIn == -1)
        {
            // Consume it only when this is a main character
            for (int i = 0; i < AppController.APP_DELETE_TURNS_COST; i++)
            {
                if (_loopManager.HasTurnsRemaining())
                {
                    Turn turn = new Turn
                    {
                        Position = LevelManager.Instance.MainCharacter.GetCurrentPosition(),
                    };
                    LevelManager.Instance.MainCharacter.AddTurn(turn);

                    _loopManager.EndTurn(LevelManager.Instance.MainCharacter.GetTurns());
                }
            }

            _consumed = true;

            LevelManager.Instance.LoopManager.addLoops(loopsToAddOnDestroy);
            _loopDestroyedIn = LoopManager.CurrentLoops;
        }

        RunDestroySeqeuence();
    }

    private void RunDestroySeqeuence()
    {
        CinemachineImpulseSource cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        cinemachineImpulseSource.GenerateImpulse();

        StartCoroutine(DelayedDestroy(0.5f));
    }

    public void OnResetForLoop(int[,] mapData, Vector2 pos)
    {
        // WHAT ARE YOU LOOKING AT?
    }
}
