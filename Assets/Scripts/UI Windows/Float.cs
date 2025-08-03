using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Float : MonoBehaviour
{
    [Header("Float Settings")]
    [SerializeField] private float amplitude = 0.5f;      // How far to float up/down
    [SerializeField] private float frequency = 1.0f;      // Speed of the floating motion
    private float _xFrequency;
    private float _yFrequency;
    
    // Original position
    private Vector3 _startPosition;

    public void Init()
    {
        // Store initial position
        _startPosition = transform.position;
        _xFrequency = frequency * Random.Range(0.7f, 1.3f);
        _yFrequency = frequency * Random.Range(0.7f, 1.3f);
    }

    void Update()
    {
        // Calculate the new position offset using a sine wave
        float xTime = Time.time * _xFrequency;
        float yTime = Time.time * _yFrequency;
        
        // Create a new position with the sine wave offset
        Vector3 newPosition = _startPosition;
    
        newPosition.y = _startPosition.y + Mathf.Sin(yTime) * amplitude;
        newPosition.x = _startPosition.x + Mathf.Cos(xTime) * amplitude;
        
        // Apply the new position
        transform.position = newPosition;
    }
}
