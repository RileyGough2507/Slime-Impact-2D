using UnityEngine;

public class GhostProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;
    public float lifetime = 5f;

    [Header("Explosion")]
    public GameObject explosionPrefab;

    [Header("Audio")]
    public AudioClip explosionSound;
    public AudioSource audioSource;

    [Header("Sprites")]
    public Sprite rightSprite;
    public Sprite leftSprite;
    private SpriteRenderer spriteRenderer;

    [Header("Damage Source")]
    public GameObject sourceObject;

    private float direction;
    private float timer;
    private bool hasExploded = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(float dir, GameObject source)
    {
        direction = dir;
        sourceObject = source;
        timer = lifetime;

        if (spriteRenderer != null)
            spriteRenderer.sprite = direction > 0 ? rightSprite : leftSprite;
    }

    void Update()
    {
        if (hasExploded)
            return;

        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);

        timer -= Time.deltaTime;
        if (timer <= 0f)
            Explode();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasExploded)
            Explode();
    }

    void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        // Spawn explosion prefab (this now handles damage)
        if (explosionPrefab != null)
        {
            GameObject exp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Pass the source to the explosion
            ExplosionDamage expScript = exp.GetComponent<ExplosionDamage>();
            if (expScript != null)
                expScript.sourceObject = sourceObject;
        }

        // Play sound
        if (audioSource != null && explosionSound != null)
            audioSource.PlayOneShot(explosionSound);

        Destroy(gameObject);
    }
}
