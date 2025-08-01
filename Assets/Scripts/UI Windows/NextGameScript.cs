using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextGameScript : MonoBehaviour
{
    public float delayUntilNextLevel = 2;

    public void GoNext()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(delayUntilNextLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
