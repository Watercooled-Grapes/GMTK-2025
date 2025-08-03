using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer), typeof(Collider2D))]
public class Reset : MonoBehaviour
{
    private SpriteRenderer _renderer;
    private Color _originalColor;
    private bool _isHovered = false;

    private void Start()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _originalColor = _renderer.color;
    }

    private void OnMouseEnter()
    {
        _isHovered = true;
        _renderer.color = _originalColor * 0.8f; // darker gray
    }

    private void OnMouseExit()
    {
        _isHovered = false;
        _renderer.color = _originalColor;
    }

    private void OnMouseDown()
    {
        _renderer.color = _originalColor * 0.6f; // even darker when clicked
    }

    private void OnMouseUp()
    {
        if (_isHovered)
        {
            // Reset color and reload scene
            _renderer.color = _originalColor * 0.8f;
            GameManager.Instance.RestartLevel();
        }
        else
        {
            _renderer.color = _originalColor;
        }
    }
}
