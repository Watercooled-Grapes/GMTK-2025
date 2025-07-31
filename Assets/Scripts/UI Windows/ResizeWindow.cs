using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResizeWindow : MonoBehaviour, IDragHandler
{
    private RectTransform windowTransform;
    private Canvas canvas;
    [SerializeField] private Window.Orientation pos;

    private void Awake()
    {
        if (windowTransform == null) windowTransform = transform.parent.GetComponent<RectTransform>();
        if (canvas == null) canvas = transform.parent.parent.GetComponent<Canvas>();

        this.GetComponent<Image>().color = new Color32(0, 0, 0, 0);
    }

    public void OnDrag(PointerEventData eventData)
    {
        float deltaX = eventData.delta.x / canvas.scaleFactor;
        float deltaY = eventData.delta.y / canvas.scaleFactor;

        float currentWidth = windowTransform.rect.width;
        float currentHeight = windowTransform.rect.height;

        float newWidth;
        float newHeight;

        switch (pos)
        {
            case Window.Orientation.TL:
                newWidth = currentWidth - deltaX;
                newHeight = currentHeight + deltaY;

                if (newWidth < Window.minWidth || newWidth > Window.maxWidth) deltaX = 0;
                if (newHeight < Window.minHeight || newHeight > Window.maxHeight) deltaY = 0;

                windowTransform.offsetMin += new Vector2(deltaX, 0);
                windowTransform.offsetMax += new Vector2(0, deltaY);

                break;
            case Window.Orientation.TR:
                newWidth = currentWidth + deltaX;
                newHeight = currentHeight + deltaY;

                if (newWidth < Window.minWidth || newWidth > Window.maxWidth) deltaX = 0;
                if (newHeight < Window.minHeight || newHeight > Window.maxHeight) deltaY = 0;

                windowTransform.offsetMax += new Vector2(deltaX, deltaY);

                break;
            case Window.Orientation.BL:
                newWidth = currentWidth - deltaX;
                newHeight = currentHeight - deltaY;

                if (newWidth < Window.minWidth || newWidth > Window.maxWidth) deltaX = 0;
                if (newHeight < Window.minHeight || newHeight > Window.maxHeight) deltaY = 0;

                windowTransform.offsetMin += new Vector2(deltaX, deltaY);

                break;
            case Window.Orientation.BR:
                newWidth = currentWidth + deltaX;
                newHeight = currentHeight - deltaY;

                if (newWidth < Window.minWidth || newWidth > Window.maxWidth) deltaX = 0;
                if (newHeight < Window.minHeight || newHeight > Window.maxHeight) deltaY = 0;

                windowTransform.offsetMin += new Vector2(0, deltaY);
                windowTransform.offsetMax += new Vector2(deltaX, 0);

                break;
        }
    }
}
