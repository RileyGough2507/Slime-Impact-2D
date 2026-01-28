using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [Header("Shield Settings")]
    public GameObject shieldVisual;     // The sprite around the player
    public int maxShieldHits = 3;
    public float cooldownTime = 60f;    // 1 minute cooldown
    public float visibleDuration = 3f;  // How long shield shows after blocking
    public AudioClip shieldHitSound;
    public AudioClip shieldBreakSound;  // ⭐ NEW

    private int shieldHitsRemaining;
    private bool shieldActive = false;
    private bool onCooldown = false;

    private AudioSource audioSource;
    private Coroutine hideRoutine;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (shieldVisual != null)
            shieldVisual.SetActive(false); // ⭐ Start invisible
    }

    void Update()
    {
        if (shieldVisual != null)
            shieldVisual.transform.position = transform.position;
    }

    // Called by NPC after talking
    public void ActivateShield()
    {
        if (onCooldown)
            return;

        shieldHitsRemaining = maxShieldHits;
        shieldActive = true;

        // ⭐ Do NOT show the shield yet — only show when blocking a hit
        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }

    // Called by PlayerController BEFORE taking damage
    public bool TryBlockHit()
    {
        if (!shieldActive)
            return false;

        shieldHitsRemaining--;

        // ⭐ Show shield for 3 seconds
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);

            if (hideRoutine != null)
                StopCoroutine(hideRoutine);

            hideRoutine = StartCoroutine(HideShieldAfterDelay());
        }

        // ⭐ Play hit sound
        if (audioSource != null && shieldHitSound != null)
            audioSource.PlayOneShot(shieldHitSound);

        if (shieldHitsRemaining <= 0)
            BreakShield();

        return true; // Damage blocked
    }

    System.Collections.IEnumerator HideShieldAfterDelay()
    {
        yield return new WaitForSeconds(visibleDuration);

        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }

    void BreakShield()
    {
        shieldActive = false;
        onCooldown = true;

        if (shieldVisual != null)
            shieldVisual.SetActive(false);

        // ⭐ Play cooldown sound
        if (audioSource != null && shieldBreakSound != null)
            audioSource.PlayOneShot(shieldBreakSound);

        Invoke(nameof(ResetShield), cooldownTime);
    }

    void ResetShield()
    {
        onCooldown = false;
        // Shield stays invisible until next hit
    }
}
