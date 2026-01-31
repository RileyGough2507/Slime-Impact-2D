using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TilemapFadeKillZone : MonoBehaviour
{
    [Header("Player Tag")]
    public string playerTag = "Player";

    [Header("Fade Settings")]
    public Image fadeImage;        // Fullscreen black Image
    public float fadeSpeed = 2f;   // Higher = faster fade

    [Header("Audio")]
    public AudioSource fadeAudioSource;   // Plays the fade sound
    public AudioClip fadeSound;           // Teleport sound (plays once)

    private bool isProcessing = false;

    private void Start()
    {
        // Make sure fade image starts invisible
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.enabled = false; // Hidden at start
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isProcessing)
            return;

        if (!other.CompareTag(playerTag))
            return;

        StartCoroutine(FadeAndRespawn(other.gameObject));
    }

    IEnumerator FadeAndRespawn(GameObject player)
    {
        isProcessing = true;

        // Enable fade image so it can be seen
        fadeImage.enabled = true;

        // Fade to black
        yield return StartCoroutine(FadeToAlpha(1f));

        // Play teleport sound ONCE, fully
        if (fadeAudioSource != null && fadeSound != null)
            fadeAudioSource.PlayOneShot(fadeSound);

        // Wait for the sound to finish
        if (fadeSound != null)
            yield return new WaitForSeconds(fadeSound.length);
        else
            yield return new WaitForSeconds(1f);

        // Teleport player
        Vector3 checkpoint = CheckpointManager.instance.GetLastCheckpointPosition();
        player.transform.position = checkpoint;

        // Fade back to clear
        yield return StartCoroutine(FadeToAlpha(0f));

        // Hide fade image again
        fadeImage.enabled = false;

        isProcessing = false;
    }

    IEnumerator FadeToAlpha(float targetAlpha)
    {
        Color c = fadeImage.color;

        if (targetAlpha > c.a)
        {
            // Fade in
            while (c.a < targetAlpha)
            {
                c.a += Time.deltaTime * fadeSpeed;
                fadeImage.color = c;
                yield return null;
            }
        }
        else
        {
            // Fade out
            while (c.a > targetAlpha)
            {
                c.a -= Time.deltaTime * fadeSpeed;
                fadeImage.color = c;
                yield return null;
            }
        }

        c.a = targetAlpha;
        fadeImage.color = c;
    }
}
