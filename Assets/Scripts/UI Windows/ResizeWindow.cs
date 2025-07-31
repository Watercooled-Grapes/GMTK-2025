using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResizeWindow : MonoBehaviour, IDragHandler
{
    private RectTransform windowTransform;
    private Canvas canvas;
    private Window windowScript;

    private void Awake()
    {
        if (windowTransform == null) windowTransform = transform.parent.GetComponent<RectTransform>();
        if (canvas == null) canvas = transform.parent.parent.GetComponent<Canvas>();
        if (windowScript == null) windowScript = transform.parent.GetComponent<Window>();

        this.GetComponent<Image>().color = new Color32(0,0,0,0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float deltaX = eventData.delta.x / canvas.scaleFactor;
        float deltaY = eventData.delta.y / canvas.scaleFactor;
        
        float currentWidth = windowTransform.rect.width;
        float currentHeight = windowTransform.rect.height;

        float newWidth = currentWidth - deltaX;
        float newHeight = currentHeight + deltaY;

        if (newWidth < windowScript.minWidth) deltaX = 0;
        if (newHeight < windowScript.minHeight) deltaY = 0; 
        
        windowTransform.offsetMin += new Vector2(deltaX, 0);
        windowTransform.offsetMax += new Vector2(0, deltaY);
    }
}
