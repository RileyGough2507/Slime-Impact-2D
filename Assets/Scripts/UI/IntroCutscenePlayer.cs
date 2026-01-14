using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IntroCutsceneController : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;         // Your VideoPlayer
    public RawImage videoScreen;            // RawImage showing the RenderTexture
    public string gameSceneName = "GameScene";

    [Header("UI Elements to Hide")]
    public GameObject[] uiToDisableOnPlay;  // Assign only the UI elements you want hidden

    void Start()
    {
        // Ensure video screen is hidden at start
        if (videoScreen != null)
            videoScreen.gameObject.SetActive(false);

        // Configure VideoPlayer
        videoPlayer.waitForFirstFrame = false;
        videoPlayer.playOnAwake = false;
        videoPlayer.skipOnDrop = false;
    }

    public void PlayGame()
    {
        // Disable selected UI elements
        foreach (GameObject uiElement in uiToDisableOnPlay)
        {
            if (uiElement != null)
                uiElement.SetActive(false);
        }

        // Show video screen
        videoScreen.gameObject.SetActive(true);
        videoScreen.enabled = true;

        // Play video
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene(gameSceneName);
    }
}
