using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using static MainCharacter;

public enum GameState {
    CUTSCENE = 0,
    PLAYING = 1,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    public GameObject cutsceneObject;
    [SerializeField] private AudioClip turnoffSound;
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
        DontDestroyOnLoad(cutsceneObject);

        if (cutsceneObject == null)
        {
            Debug.LogError("Please configure cutscene display for GameManager. The cutscene display can be found in prefabs");
        }

        CurrentState = GameState.PLAYING;
    }

    public void RestartLevel()
    {
        CurrentState = GameState.CUTSCENE;
        StartCoroutine(SpawnPopups());

        FindFirstObjectByType<CodeLineManager>().GoCrazy();
        FindFirstObjectByType<InfoTextManager>().GoCrazy();
        FindFirstObjectByType<MainCharacter>().IsInteractable = false;
        GameObject[] clones = GameObject.FindGameObjectsWithTag("Clones");
        foreach (GameObject go in clones)
        {
            go.GetComponent<LoopInstance>().GoCrazy();
        }
    }

    private IEnumerator SpawnPopups()
    {
        yield return new WaitForSeconds(3);
        for (int i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.1f);
            FindFirstObjectByType<PopupManager>().SpawnPopup(PopupTypes.Img);
        }
        CurrentState = GameState.PLAYING;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void LoadNextLevelWithCutscene(int nextSceneIndex)
    {
        LoopManager mgr = FindFirstObjectByType<LoopManager>();
        if (mgr) mgr._isWinning = true;
        CurrentState = GameState.CUTSCENE;
        StartCoroutine(PlayCutsceneAndLoad(nextSceneIndex));
    }

    private IEnumerator PlayCutsceneAndLoad(int nextSceneIndex)
    {
        loadingOperation = SceneManager.LoadSceneAsync(nextSceneIndex);
        loadingOperation.allowSceneActivation = false;

        cutsceneObject.SetActive(true);
        GetComponent<AudioSource>().PlayOneShot(turnoffSound);

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
