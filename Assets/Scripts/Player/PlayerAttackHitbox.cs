using UnityEngine;

public class PlayerAttackHitbox : MonoBehaviour
{
    public PlayerController2D player;

    [Header("Red Riot UI")]
    public GameObject redRiotHealthUI;
    public GameObject redRiotUIRoot;

    [Header("Scientist UI")]
    public GameObject scientistHealthUI;
    public GameObject scientistUIRoot;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!player.isAttacking)
            return;

        // BLUE SLIME
        BlueSlime blue = other.GetComponent<BlueSlime>();
        if (blue != null)
        {
            blue.TakeHit();
            return;
        }

        // RED SLIME
        RedSlime red = other.GetComponent<RedSlime>();
        if (red != null)
        {
            red.TakeHit();
            return;
        }

        // CACTUS SLIME
        CactusSlime cactus = other.GetComponent<CactusSlime>();
        if (cactus != null)
        {
            cactus.TakeHit();
            return;
        }

        // RED RIOT
        RedRiotBoss riot = other.GetComponent<RedRiotBoss>();
        if (riot != null)
        {
            redRiotHealthUI.SetActive(true);
            redRiotUIRoot.SetActive(true);
            riot.TakeHitFrom(this.gameObject);
            return;
        }

        // SCIENTIST BOSS
        ScientistBoss sci = other.GetComponent<ScientistBoss>();
        if (sci != null)
        {
            scientistHealthUI.SetActive(true);
            scientistUIRoot.SetActive(true);
            sci.TakeDamage(1);
            return;
        }
    }
}
