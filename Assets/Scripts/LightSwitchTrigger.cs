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

        // If the new music is already playing, do nothing
        if (newMusicSource != null)
        {
            bool sameClip = currentMusicSource != null &&
                            currentMusicSource.clip == newMusicSource.clip;

            bool alreadyPlaying = newMusicSource.isPlaying;

            if (!sameClip && !alreadyPlaying)
            {
                if (currentMusicSource != null)
                    currentMusicSource.Stop();

                newMusicSource.Play();
            }
        }
    }
}
