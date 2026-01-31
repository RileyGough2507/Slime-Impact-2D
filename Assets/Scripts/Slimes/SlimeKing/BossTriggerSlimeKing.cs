using UnityEngine;

public class BossTriggerSlimeKing : MonoBehaviour
{
    [Header("Music")]
    public AudioSource currentMusic;   // Normal background music
    public AudioSource bossMusic;      // Boss theme

    [Header("Boss Activation")]
    public GameObject bossObject;      // Boss root object (disabled at start)

    [Header("Boss UI")]
    public GameObject bossUI;          // Health bar + UI (disabled at start)

    [Header("Trigger Settings")]
    public string playerTag = "Player";
    private bool activated = false;

    private void Start()
    {
        // Ensure boss and UI are off at game start
        if (bossObject != null)
            bossObject.SetActive(false);

        if (bossUI != null)
            bossUI.SetActive(false);

        // Boss music should NOT play at start
        if (bossMusic != null)
            bossMusic.Stop();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated)
            return;

        if (other.CompareTag(playerTag))
        {
            activated = true;
            ActivateBossSequence();
        }
    }

    private void ActivateBossSequence()
    {
        // Turn off current music
        if (currentMusic != null)
            currentMusic.Stop();

        // Turn on boss music
        if (bossMusic != null)
            bossMusic.Play();

        // Activate boss object
        if (bossObject != null)
            bossObject.SetActive(true);

        // Enable UI
        if (bossUI != null)
            bossUI.SetActive(true);
    }
}
