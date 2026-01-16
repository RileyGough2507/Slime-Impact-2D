using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialogueTyper : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup dialogueCanvas;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI continuePrompt;
    public PlayerPortraitController portraitController;

    [Header("Typing Settings")]
    [TextArea]
    public string fullText;
    public float typeSpeed = 0.05f;
    public AudioSource typingAudio;
    public AudioClip typingSound;

    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;

    [Header("Hide While Talking")]
    public List<GameObject> hideObjects = new List<GameObject>();

    private bool isTyping = false;
    private bool isFinished = false;

    void Start()
    {
        // Hide assigned objects
        foreach (var obj in hideObjects)
            if (obj != null) obj.SetActive(false);

        dialogueCanvas.alpha = 1f;
        dialogueCanvas.blocksRaycasts = true;
        continuePrompt.gameObject.SetActive(false);
        dialogueText.text = "";

        StartCoroutine(TypeText());
    }

    void Update()
    {
        if (isFinished && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(FadeOutDialogue());
        }
    }

    IEnumerator TypeText()
    {
        isTyping = true;
        portraitController.StartTalking();

        foreach (char c in fullText)
        {
            dialogueText.text += c;

            if (typingAudio != null && typingSound != null)
                typingAudio.PlayOneShot(typingSound);

            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        isFinished = true;
        portraitController.StopTalking();
        continuePrompt.gameObject.SetActive(true);
    }

    IEnumerator FadeOutDialogue()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            dialogueCanvas.alpha = 1f - (t / fadeDuration);
            yield return null;
        }

        dialogueCanvas.alpha = 0f;
        dialogueCanvas.blocksRaycasts = false;

        // Restore hidden objects
        foreach (var obj in hideObjects)
            if (obj != null) obj.SetActive(true);
    }
}
