using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float radius = 1.5f;
    public float duration = 0.2f;
    public LayerMask damageLayers;     // Enemies + cracked blocks

    [Header("Damage")]
    public int damageAmount = 5;
    public GameObject sourceObject;    // Passed from projectile

    private bool hasExploded = false;

    void Start()
    {
        // Play explosion sound from this prefab
        AudioSource audio = GetComponent<AudioSource>();
        if (audio != null)
            audio.Play();

        Explode();
        Destroy(gameObject, duration);
    }

    void Explode()
    {
        if (hasExploded)
            return;

        hasExploded = true;

        Vector2 center = transform.position;

        // Detect everything in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius, damageLayers);

        foreach (Collider2D hit in hits)
        {
            // -------------------------
            // BLUE SLIME DAMAGE
            // -------------------------
            BlueSlime slime = hit.GetComponent<BlueSlime>();
            if (slime != null)
            {
                for (int i = 0; i < damageAmount; i++)
                    slime.TakeHit();
                continue;
            }

            // -------------------------
            // RED RIOT DAMAGE
            // -------------------------
            RedRiotBoss boss = hit.GetComponent<RedRiotBoss>();
            if (boss != null)
            {
                boss.TakeHitFrom(sourceObject);
                continue;
            }

            // -------------------------
            // CRACKED BLOCK DESTRUCTION
            // Handles collider on parent OR child
            // -------------------------
            Transform t = hit.transform;

            bool isCracked =
                t.CompareTag("Cracked") ||
                (t.parent != null && t.parent.CompareTag("Cracked"));

            if (isCracked)
            {
                Transform root = t.CompareTag("Cracked") ? t : t.parent;
                Destroy(root.gameObject);
                continue;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
