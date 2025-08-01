using UnityEngine;

public class AppController : MonoBehaviour
{

    /**
     * TODO: onCollision -> throw + destroy -> camera shake 
     */
    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log("OnCollisionEnter2D " + col.gameObject.name);
        Destroy(gameObject);
    }
}
