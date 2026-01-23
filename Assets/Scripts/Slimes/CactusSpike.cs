using UnityEngine;

public class CactusSpike : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 2;
    public LayerMask playerLayer;

    private Vector2 direction;

    public void Init(Vector2 dir)
    {
        direction = dir.normalized;
        Destroy(gameObject, 4f);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        CheckHit();
    }

    void CheckHit()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.3f, playerLayer);

        if (hit != null)
        {
            PlayerController2D p = hit.GetComponent<PlayerController2D>();
            if (p != null)
                p.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
