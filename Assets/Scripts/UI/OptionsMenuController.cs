using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuController : MonoBehaviour
{
    [Header("UI Groups")]
    public GameObject[] uiToDisableOnOptions;   // Main menu UI to hide
    public GameObject optionsPanel;             // The options menu panel
    public GameObject backButton;               // Back button to show in options

    [Header("Settings UI")]
    public Slider volumeSlider;
    public Toggle fullscreenToggle;
    public Dropdown aspectRatioDropdown;
    public Dropdown fpsDropdown;

    private float pendingVolume;
    private bool pendingFullscreen;
    private int pendingAspectRatio;
    private int pendingFPS;

    void Start()
    {
        // Hide options panel at start
        optionsPanel.SetActive(false);
        backButton.SetActive(false);

        // Load saved settings
        LoadSettings();
    }

    // Called by Options Button
    public void OpenOptions()
    {
        // Hide main menu UI
        foreach (GameObject ui in uiToDisableOnOptions)
            ui.SetActive(false);

        // Show options
        optionsPanel.SetActive(true);
        backButton.SetActive(true);

        // Load current settings into UI
        volumeSlider.value = pendingVolume;
        fullscreenToggle.isOn = pendingFullscreen;
        aspectRatioDropdown.value = pendingAspectRatio;
        fpsDropdown.value = pendingFPS;
    }

    // Called by Back Button
    public void CloseOptions()
    {
        ApplySettings();

        // Hide options
        optionsPanel.SetActive(false);
        backButton.SetActive(false);

        // Restore main menu UI
        foreach (GameObject ui in uiToDisableOnOptions)
            ui.SetActive(true);
    }

    // Apply settings when leaving options
    private void ApplySettings()
    {
        // Volume
        pendingVolume = volumeSlider.value;
        AudioListener.volume = pendingVolume;

        // Fullscreen
        pendingFullscreen = fullscreenToggle.isOn;
        Screen.fullScreen = pendingFullscreen;

        // Aspect Ratio
        pendingAspectRatio = aspectRatioDropdown.value;
        ApplyAspectRatio(pendingAspectRatio);

        // FPS
        pendingFPS = fpsDropdown.value;
        ApplyFPS(pendingFPS);

        // Save
        SaveSettings();
    }

    private void ApplyAspectRatio(int index)
    {
        switch (index)
        {
            case 0: Screen.SetResolution(1920, 1080, Screen.fullScreen); break; // 16:9
            case 1: Screen.SetResolution(1280, 1024, Screen.fullScreen); break; // 5:4
            case 2: Screen.SetResolution(1600, 1200, Screen.fullScreen); break; // 4:3
            case 3: Screen.SetResolution(2560, 1080, Screen.fullScreen); break; // 21:9
        }
    }

    private void ApplyFPS(int index)
    {
        switch (index)
        {
            case 0: Application.targetFrameRate = 30; break;
            case 1: Application.targetFrameRate = 60; break;
            case 2: Application.targetFrameRate = 120; break;
            case 3: Application.targetFrameRate = 144; break;
            case 4: Application.targetFrameRate = 240; break;
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("volume", pendingVolume);
        PlayerPrefs.SetInt("fullscreen", pendingFullscreen ? 1 : 0);
        PlayerPrefs.SetInt("aspect", pendingAspectRatio);
        PlayerPrefs.SetInt("fps", pendingFPS);
    }

    private void LoadSettings()
    {
        pendingVolume = PlayerPrefs.GetFloat("volume", 1f);
        pendingFullscreen = PlayerPrefs.GetInt("fullscreen", 1) == 1;
        pendingAspectRatio = PlayerPrefs.GetInt("aspect", 0);
        pendingFPS = PlayerPrefs.GetInt("fps", 1);

        // Apply immediately on startup
        AudioListener.volume = pendingVolume;
        Screen.fullScreen = pendingFullscreen;
        ApplyAspectRatio(pendingAspectRatio);
        ApplyFPS(pendingFPS);
    }
}
