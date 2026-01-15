using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class MissionObjectiveUI : MonoBehaviour
{
    public static MissionObjectiveUI instance;

    [Header("UI Elements")]
    public RectTransform panel;          // the whole UI panel
    public TMP_Text objectiveText;       // TMP text instead of normal Text
    public Color normalColor = Color.white;
    public Color completeColor = Color.green;

    [Header("Animation Settings")]
    public float slideDuration = 0.6f;
    public float slideDistance = 300f;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip completeSound;

    private void Awake()
    {
        instance = this;
    }

    public void SetObjective(string text)
    {
        objectiveText.text = text;
        objectiveText.color = normalColor;

        // Reset position (onscreen)
        panel.anchoredPosition = new Vector2(0, 0);
    }

    public void CompleteObjective()
    {
        StartCoroutine(CompleteSequence());
    }

    IEnumerator CompleteSequence()
    {
        // Turn text green
        objectiveText.color = completeColor;

        // Play sound
        if (audioSource != null && completeSound != null)
            audioSource.PlayOneShot(completeSound);

        // Wait a moment before sliding away
        yield return new WaitForSeconds(0.5f);

        // Slide off screen
        Vector2 startPos = panel.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(slideDistance, 0);

        float t = 0f;
        while (t < slideDuration)
        {
            t += Time.deltaTime;
            float lerp = t / slideDuration;
            panel.anchoredPosition = Vector2.Lerp(startPos, endPos, lerp);
            yield return null;
        }
    }
}
