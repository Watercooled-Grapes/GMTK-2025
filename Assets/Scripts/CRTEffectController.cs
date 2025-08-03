using UnityEngine;

public class CRTEffectController : MonoBehaviour
{
    [SerializeField]
    private Material _material;

    private int _numScanLines = Shader.PropertyToID("_Number_Of_Scan_Lines");
    
    void Start()
    {
        _material.SetFloat(_numScanLines, 0);
    }
}
