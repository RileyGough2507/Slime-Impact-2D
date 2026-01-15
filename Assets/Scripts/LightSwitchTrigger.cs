using UnityEngine;

public class LightMusicSwitchTrigger : MonoBehaviour
{
    [Header("Lighting")]
    public GameObject currentGlobalLight;   // light to disable
    public GameObject newGlobalLight;       // light to enable

    [Header("Music")]
    public AudioSource currentMusicSource;  // music to stop
    public AudioSource newMusicSource;      // music to start

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Switch lights
        if (currentGlobalLight != null)
            currentGlobalLight.SetActive(false);

        if (newGlobalLight != null)
            newGlobalLight.SetActive(true);

        // Stop old music
        if (currentMusicSource != null)
            currentMusicSource.Stop();

        // Start new music
        if (newMusicSource != null)
            newMusicSource.Play();
    }
}
