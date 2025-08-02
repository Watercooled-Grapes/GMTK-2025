using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class AppController : MonoBehaviour
{
    public static int AppDeleteTurnsCost = 3;
    [SerializeField] private ParticleSystem _particleSystem;
    private Tile _tile;
    private int _loopDestroyedIn;
    [SerializeField] private int loopsToAddOnDestroy = 1;
    
    public bool BeenConsumedAtSomePoint {get; private set;} = false;
    public void Init(Tile tile)
    {
        _tile = tile;
        _tile.IsAppDeleted = false;
        gameObject.SetActive(true);
        GetComponent<Renderer>().enabled = true;
        if (BeenConsumedAtSomePoint)
        {
            // TODO: Show the icon with lower transparency 
        }
        else
        {
            // TODO: Show the normal icon
        }
    }
    
    IEnumerator DelayedDestroy(float delayTime)
    {
        GetComponent<Renderer>().enabled = false;
        _particleSystem.Play();
        yield return new WaitForSeconds(delayTime);
        
        gameObject.SetActive(false);
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        LoopInstance loopInstance = col.gameObject.GetComponent<LoopInstance>();
        if (loopInstance != null && BeenConsumedAtSomePoint && loopInstance.LoopCreatedIn == _loopDestroyedIn)
        {
            RunDestroySeqeuence();
        } else if (!BeenConsumedAtSomePoint && col.gameObject.GetComponent<MainCharacter>() != null)
        {
            LevelManager.Instance.LoopManager.addLoops(loopsToAddOnDestroy);
            BeenConsumedAtSomePoint = true;
            _loopDestroyedIn = LoopManager.CurrentLoops;
            RunDestroySeqeuence();
        }
    }

    private void RunDestroySeqeuence()
    {
        CinemachineImpulseSource cinemachineImpulseSource =
            GetComponent<CinemachineImpulseSource>();
        cinemachineImpulseSource.GenerateImpulse();
        
        if (_tile != null && _tile.TileType == GridManager.TileType.AppTile)
        {
            _tile.IsAppDeleted = true;
            _tile.IsAppScheduledForDeletion = true;
        }
        StartCoroutine(DelayedDestroy(0.5f));
    }
}
