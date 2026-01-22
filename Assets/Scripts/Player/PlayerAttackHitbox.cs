using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    public PlayerController2D player;

    public GameObject bossHealthUI;
    public GameObject bossUIRoot;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!player.isAttacking)
            return;

        BlueSlime slime = other.GetComponent<BlueSlime>();
        RedRiotBoss boss= other.GetComponent<RedRiotBoss>();
        if (slime != null)
        {
            slime.TakeHit();
        }
        if(boss != null)
        {
            bossHealthUI.SetActive(true);
            bossUIRoot.SetActive(true);
            boss.TakeHitFrom(this.gameObject);
        }
    }
}
