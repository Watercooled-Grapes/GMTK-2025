using UnityEngine;

public class SetCursor : MonoBehaviour
{
    [SerializeField] private Texture2D texture;
    private CursorMode cursorMode = CursorMode.Auto;
    private Vector2 hotSpot = Vector2.zero;

    void Start()
    {
        Cursor.SetCursor(texture, hotSpot, cursorMode);
    }
    
}
