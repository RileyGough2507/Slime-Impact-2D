using UnityEngine;
using System.Collections;

public class CameraZoomTrigger : MonoBehaviour
{
    [Header("Zoom Settings")]
    public Camera targetCamera;          // Assign your main camera
    public float zoomMultiplier = 3f;    // How much to zoom out (3x)
    public float zoomSpeed = 2f;         // How fast the zoom happens

    [Header("Trigger Settings")]
    public bool zoomOut = true;          // True = zoom out, False = zoom in
    public bool oneTimeUse = true;       // Trigger only once

    private bool hasActivated = false;
    private float originalSize;

    private void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        originalSize = targetCamera.orthographicSize;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasActivated)
            return;

        if (!other.CompareTag("Player"))
            return;

        hasActivated = true;

        float targetSize = zoomOut ? originalSize * zoomMultiplier : originalSize;
        StartCoroutine(SmoothZoom(targetSize));
    }

    IEnumerator SmoothZoom(float targetSize)
    {
        while (Mathf.Abs(targetCamera.orthographicSize - targetSize) > 0.05f)
        {
            targetCamera.orthographicSize = Mathf.Lerp(
                targetCamera.orthographicSize,
                targetSize,
                Time.deltaTime * zoomSpeed
            );

            yield return null;
        }

        targetCamera.orthographicSize = targetSize;
    }
}
