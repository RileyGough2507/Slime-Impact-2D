using UnityEngine;

public class TilemapCheckpointTeleporter : MonoBehaviour
{
    public string playerTag = "Player";

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered by: " + other.name);

        if (!other.CompareTag(playerTag))
        {
            Debug.Log("Not player, ignoring.");
            return;
        }

        if (CheckpointManager.instance == null)
        {
            Debug.LogError("CheckpointManager.instance is NULL!");
            return;
        }

        Vector3 checkpoint = CheckpointManager.instance.GetLastCheckpointPosition();
        Debug.Log("Teleporting player to checkpoint: " + checkpoint);

        other.transform.position = checkpoint;
    }
}
