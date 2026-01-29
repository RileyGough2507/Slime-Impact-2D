using UnityEngine;

public class IfaEndTrigger : MonoBehaviour
{
    [Header("Dialogue Trigger")]
    public GameObject dialogueObject;
    // Assign your dialogue system object here (or a script)

    private void OnTriggerEnter2D(Collider2D other)
    {
        IfaEscortController ifa = other.GetComponent<IfaEscortController>();
        if (ifa != null)
        {
            ifa.ReachDestination(); // Tell Ifa he made it

            if (dialogueObject != null)
                dialogueObject.SetActive(true); // Start your dialogue
        }
    }
}
