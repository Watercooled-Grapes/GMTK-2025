using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum GameState {
    CUTSCENE = 0,
    PLAYING = 1,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    public GameObject cutsceneObject;
    private AsyncOperation loadingOperation;

    public int CurrentSceneIndex { get; private set; }

    // GameState is a lock to prevent race condition
    public GameState CurrentState { get; private set; }

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

        CurrentState = GameState.PLAYING;
    }

    public void LoadNextLevelWithCutscene(int nextSceneIndex)
    {
        CurrentState = GameState.CUTSCENE;
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
        StartCoroutine(FinishSceneTransition());
    }

    private IEnumerator FinishSceneTransition()
    {
        yield return new WaitForSeconds(1f);

        loadingOperation.allowSceneActivation = true;

        while (!loadingOperation.isDone)
        {
            yield return null;
        }

        cutsceneObject.SetActive(false);
        CurrentState = GameState.PLAYING;
        CurrentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    }
}
