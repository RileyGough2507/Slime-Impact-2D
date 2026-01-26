using UnityEngine;

public class CactusSpike : MonoBehaviour
{
    public float speed = 8f;
    public float lifetime = 5f;
    public int damage = 1;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    public void Init(Vector2 dir)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.velocity = dir.normalized * speed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController2D player = other.GetComponent<PlayerController2D>();
            if (player != null)
                player.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
