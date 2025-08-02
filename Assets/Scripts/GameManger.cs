using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    public GameObject cutsceneObject;
    private AsyncOperation loadingOperation;

    public int CurrentSceneIndex { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (cutsceneObject == null) {
            Debug.LogError("Please configure cutscene display for GameManager. The cutscene display can be found in prefabs");
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }

    public void LoadNextLevelWithCutscene(int nextSceneIndex)
    {
        StartCoroutine(PlayCutsceneAndLoad(nextSceneIndex));
    }

    private IEnumerator PlayCutsceneAndLoad(int nextSceneIndex)
    {
        loadingOperation = SceneManager.LoadSceneAsync(nextSceneIndex);
        loadingOperation.allowSceneActivation = false;

        cutsceneObject.SetActive(true);

        yield break;
    }

    public void OnCutsceneFinished()
    {
        Debug.Log("Cutscene Finished");
        StartCoroutine(FinishSceneTransition());
    }

    private IEnumerator FinishSceneTransition()
    {
        cutsceneObject.SetActive(false);

        loadingOperation.allowSceneActivation = true;

        while (!loadingOperation.isDone)
        {
            yield return null;
        }

        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
}
