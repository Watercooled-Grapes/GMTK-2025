using System.Collections;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor, _wallColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private TMP_Text _textMesh;

    private SpriteRenderer _highlightRenderer;
    private Coroutine _fadeCoroutine;
    private Coroutine _scaleCoroutine;
    
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;

    public bool IsWall { get; set; } = false;

    // IsOccupied does not block character moving
    public bool IsOccupied { get; set; } = false;

    private bool _isHighlighted = false;

    private const float SELECTED_COLOR_ALPHA = 0.4f;

    public void Init(bool isOffset, int x, int y, bool isWall = false)
    {
        IsWall = isWall;
        _renderer.color = IsWall ? _wallColor : (isOffset ? _offsetColor : _baseColor);

        _highlightRenderer = _highlight.GetComponent<SpriteRenderer>();
        SetAlpha(0f);
        _highlight.transform.localScale = Vector3.one;

        X = x;
        Y = y;
    }

    void OnMouseEnter()
    {
        if (IsWall) return;
        StartFade(SELECTED_COLOR_ALPHA, 0.2f);

        if (IsOccupied) return;
        StartScale(Vector3.one * 1.05f, 0.25f);
    }

    void OnMouseExit()
    {
        StartScale(Vector3.one, 0.25f);
        if (_isHighlighted) return;
        StartFade(0f, 0.2f);
    }

    public void HighlightAsMoveOption() {
        SetAlpha(SELECTED_COLOR_ALPHA);
        _isHighlighted = true;
    }

    public void RemoveHighlight() {
        SetAlpha(0f);
        _isHighlighted = false;
    }

    private void StartFade(float targetAlpha, float duration)
    {
        if (_fadeCoroutine != null)
            StopCoroutine(_fadeCoroutine);

        _fadeCoroutine = StartCoroutine(FadeHighlight(targetAlpha, duration));
    }

    private IEnumerator FadeHighlight(float targetAlpha, float duration)
    {
        float startAlpha = _highlightRenderer.color.a;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            SetAlpha(currentAlpha);
            time += Time.deltaTime;
            yield return null;
        }

        SetAlpha(targetAlpha);
    }

    private void SetAlpha(float alpha)
    {
        if (_highlightRenderer == null) return;

        Color c = _highlightRenderer.color;
        c.a = alpha;
        _highlightRenderer.color = c;
    }

    private void StartScale(Vector3 targetScale, float duration)
    {
        if (_scaleCoroutine != null)
            StopCoroutine(_scaleCoroutine);

        _scaleCoroutine = StartCoroutine(ScaleHighlight(targetScale, duration));
    }

    private IEnumerator ScaleHighlight(Vector3 targetScale, float duration)
    {
        Vector3 startScale = _highlight.transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            float t = time / duration;
            _highlight.transform.localScale = Vector3.Lerp(startScale, targetScale, Mathf.SmoothStep(0f, 1f, t));
            time += Time.deltaTime;
            yield return null;
        }

        _highlight.transform.localScale = targetScale;
    }

    public void DisplayText(string text) {
        _textMesh.text = text;
    }
}
