using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler
{
    private RectTransform dragRectTransform;
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        if (dragRectTransform == null)
        {
            dragRectTransform = transform.parent.GetComponent<RectTransform>();
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.position.x > Screen.width || eventData.position.x < 0 || eventData.position.y > Screen.height || eventData.position.y < 0  ) return;
        dragRectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
}
