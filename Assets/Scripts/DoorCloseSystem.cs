using UnityEngine;
using System.Collections;

public class DoorCloseSystem : MonoBehaviour
{
    [Header("Door Movement")]
    public Transform door;               // The door sprite/object
    public float closeDistance = 3f;     // How far down it moves
    public float closeSpeed = 3f;        // Movement speed

    [Header("Sound")]
    public AudioSource audioSource;      // Must have loop enabled
    public AudioClip closingLoop;        // Looping sound while closing

    [Header("Red Light Flash")]
    public SpriteRenderer redLight;      // The red light sprite
    public float flashSpeed = 8f;        // How fast it flashes

    [Header("Trigger Settings")]
    public string playerTag = "Player";
    public bool oneTimeUse = true;

    private bool activated = false;
    private bool isClosing = false;
    private Vector3 startPos;
    private Vector3 endPos;

    void Start()
    {
        startPos = door.localPosition;
        endPos = startPos + new Vector3(0, -closeDistance, 0);

        if (redLight != null)
            redLight.color = new Color(1, 0, 0, 0); // invisible at start
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated && oneTimeUse)
            return;

        if (other.CompareTag(playerTag))
        {
            activated = true;
            StartCoroutine(CloseDoorRoutine());
        }
    }

    IEnumerator CloseDoorRoutine()
    {
        isClosing = true;

        // Start looping sound
        if (audioSource != null && closingLoop != null)
        {
            audioSource.loop = true;
            audioSource.clip = closingLoop;
            audioSource.Play();
        }

        // Start flashing light
        StartCoroutine(FlashLightRoutine());

        // Move door downward
        while (Vector3.Distance(door.localPosition, endPos) > 0.01f)
        {
            door.localPosition = Vector3.MoveTowards(
                door.localPosition,
                endPos,
                closeSpeed * Time.deltaTime
            );

            yield return null;
        }

        // Stop sound
        if (audioSource != null)
            audioSource.Stop();

        isClosing = false;

        // Turn off light
        if (redLight != null)
            redLight.color = new Color(1, 0, 0, 0);
    }

    IEnumerator FlashLightRoutine()
    {
        while (isClosing)
        {
            float t = Mathf.PingPong(Time.time * flashSpeed, 1f);
            if (redLight != null)
                redLight.color = new Color(1, 0, 0, t);

            yield return null;
        }
    }
}
