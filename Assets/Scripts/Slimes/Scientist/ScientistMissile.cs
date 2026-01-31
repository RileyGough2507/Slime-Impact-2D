using UnityEngine;

public class ScientistMissile : MonoBehaviour
{
    public int damage = 2;

    public float forwardSpeed = 10f;     // speed when firing forward
    public float fallSpeed = 6f;         // speed when falling down
    public float forwardTime = 0.4f;     // how long it flies forward
    public float lifetime = 5f;

    private float targetX;
    private bool falling = false;
    private Vector2 forwardDir = Vector2.right;

    // x = player X at fire time, dir = +1 (right) or -1 (left)
    public void Init(float x, int dir)
    {
        targetX = x;
        forwardDir = (dir >= 0) ? Vector2.right : Vector2.left;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
        Invoke(nameof(StartFalling), forwardTime);
    }

    private void Update()
    {
        if (!falling)
        {
            // Phase 1: fly horizontally in chosen direction
            transform.Translate(forwardDir * forwardSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // Phase 2: fall straight down toward locked X
            Vector3 pos = transform.position;
            pos.x = Mathf.MoveTowards(pos.x, targetX, 50f * Time.deltaTime);
            transform.position = pos;

            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime, Space.World);
        }
    }

    private void StartFalling()
    {
        falling = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore Scientist
        if (other.GetComponent<ScientistBoss>() != null)
            return;

        PlayerController2D player = other.GetComponent<PlayerController2D>();
        if (player != null)
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
