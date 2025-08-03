using UnityEngine;
using UnityEngine.Video;

public class CutscenePlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath,"close.mp4"); 
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        GameManager.Instance.OnCutsceneFinished();
    }
}
