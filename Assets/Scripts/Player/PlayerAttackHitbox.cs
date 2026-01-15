using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    public PlayerController2D player;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (!player.isAttacking)
            return;

        BlueSlime slime = other.GetComponent<BlueSlime>();
        if (slime != null)
        {
            slime.TakeHit();
        }
    }
}
