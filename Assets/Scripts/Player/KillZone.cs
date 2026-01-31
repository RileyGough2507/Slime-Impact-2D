using UnityEngine;

public class KillZone : MonoBehaviour
{
    [Header("Player Tag")]
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag))
            return;

        PlayerController2D player = other.GetComponent<PlayerController2D>();

        if (player != null)
        {
            player.ForceKill(); // or player.RespawnAtCheckpoint();
        }
    }
}
