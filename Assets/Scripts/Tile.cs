using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    private SpriteRenderer _highlightRenderer;
    private Coroutine _fadeCoroutine;
    private Coroutine _scaleCoroutine;

    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;

        _highlightRenderer = _highlight.GetComponent<SpriteRenderer>();
        SetAlpha(0f);
        _highlight.SetActive(true);
        _highlight.transform.localScale = Vector3.one;
    }

    void OnMouseEnter()
    {
        StartFade(0.4f, 0.2f);
        StartScale(Vector3.one * 1.2f, 0.25f);
    }

    void OnMouseExit()
    {
        StartFade(0f, 0.2f);
        StartScale(Vector3.one, 0.25f);
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
}
