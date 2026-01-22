using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RedRiotBoss : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public AudioSource audioSource;
    public SpriteRenderer spriteRenderer;

    [Header("Music")]
    public AudioSource normalMusic;
    public AudioSource bossMusic;

    [Header("Health")]
    public int maxHealth = 40;
    private int currentHealth;

    [Header("Health Bar (Slider)")]
    public GameObject healthBarObject;
    public Slider healthSlider;

    [Header("Teleport Sprite")]
    public GameObject teleportSpritePrefab;
    public Vector2 spriteOffset = new Vector2(0f, -0.5f);
    private GameObject activeTeleportSprite;

    [Header("Teleport Points")]
    public Transform[] teleportPoints;

    [Header("Animation Names")]
    public string idleAnimName = "RedRiotIdle";
    public string attackAnimName = "RedRiotAttack";
    public string deathAnimName = "RedRiotDeath";

    [Header("Damage Flash")]
    public Color hitColor = Color.red;
    public float flashDuration = 0.15f;
    private Color originalColor;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;
    public float timeBetweenShots = 3f;

    [Header("Bomb")]
    public GameObject bombPrefab;
    public Transform bombFirePoint;
    public float bombForce = 8f;
    public int shotsBeforeBomb = 10;

    [Header("Drop On Death")]
    public GameObject deathDropPrefab;

    [Header("Audio Clips")]
    public AudioClip bulletFireClip;
    public AudioClip bombFireClip;
    public AudioClip teleportOpenClip;
    public AudioClip teleportCloseClip;
    public AudioClip deathClip;
    public AudioClip damageClip;

    private bool fightStarted = false;
    private bool canAttack = false;
    private bool isTeleporting = false;
    private bool isDead = false;

    private int shotCounter = 0;
    private int currentTeleportIndex = -1;

    private Vector3 originalPosition;
    private Collider2D hitbox;

    void Start()
    {
        currentHealth = maxHealth;
        originalPosition = transform.position;

        hitbox = GetComponent<Collider2D>();

        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        animator.Play(idleAnimName);

        if (healthSlider != null)
            healthSlider.maxValue = maxHealth;

        if (healthBarObject != null)
            healthBarObject.SetActive(false);
    }

    // -----------------------------
    // START BOSS FIGHT
    // -----------------------------
    public void StartBoss()
    {
        if (isDead)
            return;

        fightStarted = true;
        canAttack = true;

        // MUSIC SWITCH
        if (normalMusic != null) normalMusic.Stop();
        if (bossMusic != null && !bossMusic.isPlaying) bossMusic.Play();

        if (healthBarObject != null)
            healthBarObject.SetActive(true);

        UpdateHealthBar();
        StartCoroutine(AttackLoop());
    }


    IEnumerator AttackLoop()
    {
        while (fightStarted && !isDead)
        {
            if (canAttack && !isTeleporting)
                animator.Play(attackAnimName);

            yield return new WaitForSeconds(timeBetweenShots);
        }
    }

    // -----------------------------
    // SHOOTING
    // -----------------------------
    public void FireShot()
    {
        if (!fightStarted || isTeleporting || isDead)
            return;

        shotCounter++;

        if (shotCounter >= shotsBeforeBomb)
        {
            shotCounter = 0;
            FireBomb();
        }
        else
        {
            FireBullet();
        }
    }

    void FireBullet()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        RedRiotBullet bullet = bulletObj.GetComponent<RedRiotBullet>();
        if (bullet != null)
        {
            Vector2 dir = (player.position - firePoint.position).normalized;
            bullet.Init(dir * bulletSpeed);
        }

        PlaySound(bulletFireClip);
    }

    void FireBomb()
    {
        GameObject bombObj = Instantiate(bombPrefab, bombFirePoint.position, Quaternion.identity);

        RedRiotBomb bomb = bombObj.GetComponent<RedRiotBomb>();
        if (bomb != null)
        {
            Vector2 dir = (player.position - bombFirePoint.position).normalized;
            Vector2 launch = new Vector2(dir.x, 1f).normalized * bombForce;
            bomb.Init(launch);
        }

        PlaySound(bombFireClip);
    }

    // -----------------------------
    // DAMAGE
    // -----------------------------
    public void TakeHitFrom(GameObject source)
    {
        if (isDead || isTeleporting)
            return;

        PlayerController2D player = source.GetComponentInParent<PlayerController2D>();
        if (player == null)
            return;

        // If this hit will kill him, DO NOT teleport
        if (currentHealth <= 1)
        {
            currentHealth--;
            UpdateHealthBar();
            Die();
            return;
        }

        PlaySound(damageClip);
        StartCoroutine(FlashRed());
        StartCoroutine(TeleportSequence());

        currentHealth--;
        UpdateHealthBar();

        if (currentHealth <= 0)
            Die();
    }


    IEnumerator FlashRed()
    {
        spriteRenderer.material.color = hitColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.material.color = originalColor;
    }
    // -----------------------------
    // TELEPORT
    // -----------------------------
    IEnumerator TeleportSequence()
    {
        // Do NOT teleport if dying
        if (isDead || currentHealth <= 1)
            yield break;



        isTeleporting = true;
        canAttack = false;

        spriteRenderer.enabled = false;
        if (hitbox != null)
            hitbox.enabled = false;

        PlaySound(teleportOpenClip);

        if (teleportSpritePrefab != null)
        {
            Vector3 spritePos = transform.position + (Vector3)spriteOffset;
            activeTeleportSprite = Instantiate(teleportSpritePrefab, spritePos, Quaternion.identity);
        }

        yield return new WaitForSeconds(2f);

        TeleportToRandomPoint();

        if (activeTeleportSprite != null)
        {
            Destroy(activeTeleportSprite);
            activeTeleportSprite = null;
        }

        spriteRenderer.enabled = true;
        if (hitbox != null)
            hitbox.enabled = true;

        isTeleporting = false;
        canAttack = true;
        animator.Play(idleAnimName);
    }

    void TeleportToRandomPoint()
    {
        if (teleportPoints == null || teleportPoints.Length == 0)
            return;

        int newIndex = currentTeleportIndex;

        if (teleportPoints.Length > 1)
        {
            while (newIndex == currentTeleportIndex)
                newIndex = Random.Range(0, teleportPoints.Length);
        }
        else
        {
            newIndex = 0;
        }

        currentTeleportIndex = newIndex;

        Vector3 targetPos = teleportPoints[newIndex].position;
        transform.position = new Vector3(targetPos.x, targetPos.y, transform.position.z);

        PlaySound(teleportCloseClip);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = Vector2.zero;
    }

    // -----------------------------
    // DEATH
    // -----------------------------
    void Die()
    {
        if (isDead)
            return;

        isDead = true;
        canAttack = false;
        isTeleporting = false;

        // Stop all ongoing actions (teleport, attack loop, etc.)
        StopAllCoroutines();

        // Stop boss music, return to normal music
        if (bossMusic != null) bossMusic.Stop();
        if (normalMusic != null && !normalMusic.isPlaying) normalMusic.Play();

        // Hide Red Riot visually + disable hitbox
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (hitbox != null)
            hitbox.enabled = false;

        // Play death sound
        PlaySound(deathClip);

        // Play explosion sound + spawn explosion
        if (explosionPrefab != null)
        {
            Vector3 spawnPos = transform.position + (Vector3)spriteOffset;
            GameObject explosion = Instantiate(explosionPrefab, spawnPos, Quaternion.identity);
            Destroy(explosion, 1.5f);
        }

        PlaySound(explosionSound);

        // Hide health bar
        if (healthBarObject != null)
            healthBarObject.SetActive(false);

        // Destroy Red Riot AFTER sounds finish
        Destroy(gameObject, 2f);

        // Disable assigned objects
        if (objectsToDisable != null)
        {
            foreach (GameObject obj in objectsToDisable)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }

    }





    // -----------------------------
    // PLAYER DEATH RESET
    // -----------------------------
    public void OnPlayerDied()
    {
        // Restore 10 HP, but cap at 25
        currentHealth = Mathf.Min(currentHealth + 10, 25);

        UpdateHealthBar();
    }



    // -----------------------------
    // MANUAL RESET (OPTIONAL)
    // -----------------------------
    public void ResetBoss()
    {
        StopAllCoroutines();

        fightStarted = false;
        canAttack = false;
        isTeleporting = false;
        isDead = false;

        currentHealth = maxHealth;
        shotCounter = 0;

        transform.position = originalPosition;

        animator.Play(idleAnimName);

        if (healthBarObject != null)
            healthBarObject.SetActive(false);

        UpdateHealthBar();
    }

    // -----------------------------
    // HELPERS
    // -----------------------------
    void UpdateHealthBar()
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    [Header("Death Explosion")]
    public GameObject explosionPrefab;   // explosion animation prefab
    public AudioClip explosionSound;     // sound to play on death

    [Header("Disable On Death")]
    public GameObject[] objectsToDisable;

}
