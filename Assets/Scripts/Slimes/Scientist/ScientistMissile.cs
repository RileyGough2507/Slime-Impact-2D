using UnityEngine;

public class ScientistMissile : MonoBehaviour
{
    public int damage = 2;
    public float fallSpeed = 6f;
    public float lifetime = 5f;

    private float targetX;

    public void Init(float x)
    {
        targetX = x;
    }

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController2D player = other.GetComponent<PlayerController2D>();
        if (player != null)
        {
            player.TakeDamage(damage);
        }
    }
}
