using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private Camera _cam;

    void Awake()
    {
        _cam = GetComponent<Camera>();
    }

    public void CenterAndZoom(int gridWidth, int gridHeight)
    {
        // Center the camera on the board
        transform.position = new Vector3(gridWidth / 2f - 0.5f, gridHeight / 2f - 0.5f, -10f);

        // Zoom out to fit the entire board (with padding)
        float aspectRatio = (float)Screen.width / Screen.height;
        float verticalSize = gridHeight / 2f + 1f;
        float horizontalSize = (gridWidth / 2f + 1f) / aspectRatio;

        _cam.orthographicSize = Mathf.Max(verticalSize, horizontalSize);
    }
}
