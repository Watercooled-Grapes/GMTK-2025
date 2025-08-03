
using UnityEngine;
using UnityEngine.UI;

public class Rewind : MonoBehaviour
{
    private Image _image;
    private AudioSource _audioSource;

    void Start()
    {
        _image = GetComponent<Image>();
        _audioSource = GetComponent<AudioSource>();
        SetInvisible();
    }

    public void SetVisible()
    {
        _image.color = new Color(1, 1, 1, 1);
        _audioSource.Play();
        
    }

    public void SetInvisible()
    {
        _image.color = new Color(0, 0, 0, 0);
         _audioSource.Stop();
    }
}
