using UnityEngine;
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
    private bool hasClosedOnce = false;   // ⭐ NEW — prevents reopening fade
    private Coroutine bobRoutine;

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
        if (isFinished && !hasClosedOnce && Input.GetMouseButtonDown(0))
        {
            hasClosedOnce = true; // ⭐ Prevents future fades
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

        // ⭐ STOP AUDIO IMMEDIATELY
        if (typingAudio != null)
            typingAudio.Stop();

        isTyping = false;
        isFinished = true;
        portraitController.StopTalking();

        continuePrompt.gameObject.SetActive(true);

        // ⭐ Start bobbing animation
        bobRoutine = StartCoroutine(BobContinuePrompt());
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

        // ⭐ CALL THIS LAST — AFTER EVERYTHING IS DONE
        portraitController.OnDialogueClosed();
    }


    // -----------------------------------------
    // ⭐ Bobbing animation for "Click to continue"
    // -----------------------------------------
    IEnumerator BobContinuePrompt()
    {
        Vector3 basePos = continuePrompt.rectTransform.localPosition;
        float timer = 0f;

        while (true)
        {
            timer += Time.deltaTime * 2f; // speed
            float offset = Mathf.Sin(timer) * 5f; // height

            continuePrompt.rectTransform.localPosition =
                basePos + new Vector3(0, offset, 0);

            yield return null;
        }
    }
}
