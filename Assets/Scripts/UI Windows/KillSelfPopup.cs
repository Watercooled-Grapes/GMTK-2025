using UnityEngine;

public class KillSelfPopup : MonoBehaviour
{
    public void WindowSuicide()
    {
        Destroy(transform.parent.parent.gameObject);
    }
}
