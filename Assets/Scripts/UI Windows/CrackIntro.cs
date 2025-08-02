
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;
using System.Collections;
using UnityEngine.Playables;
using System;

public class CrackIntro : MonoBehaviour
{
    [SerializeField] GameObject prevObject;
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private AudioClip glassBreakingSound;
    private AudioSource _audioSource;
    CinemachineImpulseSource cinemachineImpulseSource;
    public PlayableDirector dir;
    public SpriteRenderer lilGuy;
    private Image _image;

    [SerializeField] private Sprite[] cracks;
    private float delayBetweenCracks = 1f;
    public float delayBeforeLilGuy = 3f;

    void Start()
    {
        prevObject.GetComponent<TypewriterOnEvent>().StartGameInput += StartCracking;
        _image = GetComponent<Image>();
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        _audioSource = GetComponent<AudioSource>();
    }

    void StartCracking()
    {
        StartCoroutine(Crack());
    }

    private IEnumerator Crack()
    {
        _image.sprite = cracks[0];
        _particleSystem.Play();
        cinemachineImpulseSource.GenerateImpulse();
        _audioSource.pitch = Mathf.Pow(2, UnityEngine.Random.Range(-2,2)/12);
        _audioSource.PlayOneShot(glassBreakingSound);
        _image.color = new Color(255, 255, 255, 1);
        yield return new WaitForSeconds(delayBetweenCracks);
        _image.sprite = cracks[1];
        _particleSystem.Play();
        cinemachineImpulseSource.GenerateImpulse();
         _audioSource.PlayOneShot(glassBreakingSound);
        yield return new WaitForSeconds(delayBetweenCracks);
        _image.sprite = cracks[2];
        _particleSystem.Play();
        cinemachineImpulseSource.GenerateImpulse();
         _audioSource.PlayOneShot(glassBreakingSound);
        yield return new WaitForSeconds(delayBeforeLilGuy);
        dir.Play();
        lilGuy.color = new Color(lilGuy.color.r, lilGuy.color.g, lilGuy.color.b, 1);
    }
}
