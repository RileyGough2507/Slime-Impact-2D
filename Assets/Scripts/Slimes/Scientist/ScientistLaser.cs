using UnityEngine;

public class ScientistLaser : MonoBehaviour
{
    public int damage = 2;
    public float lifetime = 2f;

    private void Start()
    {
        Destroy(gameObject, lifetime);
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
