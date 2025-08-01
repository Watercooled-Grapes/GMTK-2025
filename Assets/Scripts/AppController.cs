using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class AppController : MonoBehaviour
{
    [SerializeField] private int appBreakCost = 5;
    [SerializeField] private ParticleSystem _particleSystem;
    IEnumerator DelayedDestroy(float delayTime)
    {
        GetComponent<Renderer>().enabled = false;
        _particleSystem.Play();
        yield return new WaitForSeconds(delayTime);
        
        Destroy(gameObject);
    }
    
    void OnTriggerEnter2D(Collider2D col)
    {
        MainCharacter mainCharacter = col.gameObject.GetComponent<MainCharacter>();
        if (mainCharacter != null)
        {
            CinemachineImpulseSource cinemachineImpulseSource =
                GetComponent<CinemachineImpulseSource>();
            cinemachineImpulseSource.GenerateImpulse();
            
            StartCoroutine(DelayedDestroy(0.5f));
        }
    }
}
