using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class AppController : MonoBehaviour
{
    // public static int appBreakCost = 5;
    [SerializeField] private ParticleSystem _particleSystem;
    private Tile _tile;
    public bool BeenConsumedAtSomePoint {get; set;} = false;
    public bool BeenConsumed {get; set;} = false;
    public void Init(Tile tile)
    {
        _tile = tile;
        _tile.AppBroken = false;
        gameObject.SetActive(true);
        GetComponent<Renderer>().enabled = true;
        if (BeenConsumedAtSomePoint)
        {
            // Show the icon with lower transparency 
        }
        else
        {
            // Show the normal icon
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
        MainCharacter mainCharacter = col.gameObject.GetComponent<MainCharacter>();
        LoopInstance loopInstance = col.gameObject.GetComponent<LoopInstance>();
        if (mainCharacter != null || loopInstance != null)
        {
            if (loopInstance != null)
                Debug.Log("TOUCHED BY CLONE");
            
            // TODO: We need to track which loop instance touched this
            CinemachineImpulseSource cinemachineImpulseSource =
                GetComponent<CinemachineImpulseSource>();
            cinemachineImpulseSource.GenerateImpulse();
            
            if (_tile != null && _tile.TileType == GridManager.TileType.AppTile)
            {
                _tile.AppBroken = true;
            }
            StartCoroutine(DelayedDestroy(0.5f));
        }
    }
}
