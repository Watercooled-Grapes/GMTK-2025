using UnityEngine;
using UnityEngine.Video;

public class OutroPlayer : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath,"outro_anim.mp4"); 
        videoPlayer.loopPointReached += OnVideoFinished;
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        GameManager.Instance.OnCutsceneFinished();
    }
}
