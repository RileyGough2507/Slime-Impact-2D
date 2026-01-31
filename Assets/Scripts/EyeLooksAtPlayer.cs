using UnityEngine;

public class PupilInsideCircle : MonoBehaviour
{
    public Transform player;
    public CircleCollider2D boundary;
    public float followStrength = 1f;

    [Header("Eye Glow")]
    public SpriteRenderer eyeRenderer;   // The WHITE part of the eye
    public Color normalColor = Color.white;
    public Color alertColor = Color.red;
    public float glowDistance = 3f;      // How close player must be to turn red
    public float glowSpeed = 5f;         // How fast it transitions

    Vector3 originalLocalPos;

    void Start()
    {
        originalLocalPos = transform.localPosition;

        if (boundary == null)
            boundary = transform.parent.GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (player == null || boundary == null)
            return;

        // --- PUPIL MOVEMENT ---
        Vector3 dirWorld = (player.position - boundary.transform.position).normalized;
        Vector3 dirLocal = boundary.transform.InverseTransformDirection(dirWorld);

        Vector3 desiredLocal = originalLocalPos + dirLocal * boundary.radius * followStrength;

        if (desiredLocal.magnitude > boundary.radius)
            desiredLocal = desiredLocal.normalized * boundary.radius;

        transform.localPosition = desiredLocal;

        // --- EYE GLOW ---
        float dist = Vector2.Distance(player.position, boundary.transform.position);
        Color targetColor = dist <= glowDistance ? alertColor : normalColor;

        if (eyeRenderer != null)
        {
            eyeRenderer.color = Color.Lerp(
                eyeRenderer.color,
                targetColor,
                Time.deltaTime * glowSpeed
            );
        }
    }
}
