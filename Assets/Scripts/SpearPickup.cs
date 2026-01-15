using UnityEngine;

public class SpearPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController2D player = collision.GetComponent<PlayerController2D>();

        if (player != null)
        {
            player.hasSpear = true;

            if (player.spearObject != null)
                player.spearObject.SetActive(true);

            // COMPLETE THE OBJECTIVE
            MissionObjectiveUI.instance.CompleteObjective();

            Destroy(gameObject);
        }
    }
}
