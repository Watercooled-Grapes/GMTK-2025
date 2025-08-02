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
        FindFirstObjectByType<GameManager>().LoadNextLevelWithCutscene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
