using System.Collections;
using System.Collections.Generic;
using System.Xml;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static GridManager;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor, _wallColor;
    [SerializeField] private Color _lineColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private TMP_Text _textMesh;

    private List<LineRenderer> _crosshatching = new List<LineRenderer>();
    private SpriteRenderer _highlightRenderer;
    private LineRenderer _lineRenderer;
    private Coroutine _fadeCoroutine;
    private Coroutine _scaleCoroutine;
    
    public int X { get; set; } = 0;
    public int Y { get; set; } = 0;
    
    [SerializeField] public TileType TileType { get; set; }

    // IsOccupied does not block character moving
    public bool IsOccupied { get; set; } = false;

    private bool _isHighlighted = false;

    private const float SELECTED_COLOR_ALPHA = 0.4f;

    public void Init(bool isOffset, int x, int y, TileType tileType = TileType.EmptyTile)
    {
        TileType = tileType;

        X = x;
        Y = y;

        if (tileType != TileType.WallTile)
        {
            initLineRenderer();
            initHatchRenderer();
        }

        _renderer.color = tileType == TileType.WallTile ? _wallColor : _baseColor;
        _highlightRenderer = _highlight.GetComponent<SpriteRenderer>();
        SetAlpha(0f);
        _highlight.transform.localScale = Vector3.one;

    }

    void initLineRenderer()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.material = new Material(Shader.Find("Unlit/Transparent"));
        _lineRenderer.startColor = _lineColor;
        _lineRenderer.endColor = _lineColor;
        _lineRenderer.startWidth = 0.05f;
        _lineRenderer.endWidth = 0.05f;

        _lineRenderer.positionCount = 4;

        _lineRenderer.SetPosition(0, new Vector3(X - 0.5f, Y - 0.5f, 0));
        _lineRenderer.SetPosition(1, new Vector3(X + 0.5f, Y - 0.5f, 0));
        _lineRenderer.SetPosition(2, new Vector3(X + 0.5f, Y + 0.5f, 0));
        _lineRenderer.SetPosition(3, new Vector3(X - 0.5f, Y + 0.5f, 0));

        _lineRenderer.loop = true;
    }

    void enableHatch()
    {
        foreach (LineRenderer lr in _crosshatching)
        {
            lr.enabled = true;
        }
    }

    void disableHatch()
    {
        foreach (LineRenderer lr in _crosshatching)
        {
            lr.enabled = false;
        }
    }

    void initHatchRenderer()
    {
        for (int i = 1; i <= 5; i++)
        {
            GameObject child = new GameObject("hatch");
            child.transform.parent = this.transform;
            LineRenderer newLine = child.AddComponent<LineRenderer>();
            newLine.material = new Material(Shader.Find("Unlit/Texture"));
            newLine.startColor = _lineColor;
            newLine.endColor = _lineColor;
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;
            newLine.positionCount = 2;

            newLine.SetPosition(0, new Vector3(X - 0.5f + (i * (1 / 5f)), Y - 0.5f, 0));
            newLine.SetPosition(1, new Vector3(X - 0.5f, Y - 0.5f + (i * (1 / 5f)), 0));

            newLine.sortingOrder = 3;

            _crosshatching.Add(newLine);
        }

        for (int i = 1; i <= 5; i++)
        {
            GameObject child = new GameObject("hatch");
            child.transform.parent = this.transform;
            LineRenderer newLine = child.AddComponent<LineRenderer>();
            newLine.material = new Material(Shader.Find("Unlit/Texture"));
            newLine.startColor = _lineColor;
            newLine.endColor = _lineColor;
            newLine.startWidth = 0.05f;
            newLine.endWidth = 0.05f;
            newLine.positionCount = 2;

            newLine.SetPosition(0, new Vector3(X + 0.5f - (i * (1 / 5f)), Y + 0.5f, 0));
            newLine.SetPosition(1, new Vector3(X + 0.5f, Y + 0.5f - (i * (1 / 5f)), 0));

            newLine.sortingOrder = 3;
            _crosshatching.Add(newLine);
        }

        disableHatch();
    }

    void OnMouseEnter()
    {
        if (TileType == TileType.WallTile) return;
        enableHatch();
    }

    void OnMouseExit()
    {
        disableHatch();
    }

    public void HighlightAsMoveOption()
    {
        SetAlpha(SELECTED_COLOR_ALPHA);
        _isHighlighted = true;
    }

    public void RemoveHighlight()
    {
        SetAlpha(0f);
        _isHighlighted = false;
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
