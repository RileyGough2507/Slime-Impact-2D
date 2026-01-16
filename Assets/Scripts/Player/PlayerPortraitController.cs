using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPortraitController : MonoBehaviour
{
    [Header("Portrait Sprites")]
    public Sprite defaultPortrait;     // Used AFTER dialogue closes
    public Sprite idlePortrait;        // Used during dialogue when NOT talking
    public Sprite talkingPortrait;     // Alternates with idle while typing
    public Sprite damagePortrait;      // Overrides everything

    [Header("UI Reference")]
    public Image portraitImage;

    [Header("Damage Settings")]
    public float damageDisplayTime = 3f;

    private Coroutine damageRoutine;
    private Coroutine talkingRoutine;

    private bool isTalking = false;
    private bool isDialogueActive = false;

    void Start()
    {
        portraitImage.sprite = defaultPortrait;
    }

    // ---------------------------------------------------------
    // START TALKING (called when typing begins)
    // ---------------------------------------------------------
    public void StartTalking()
    {
        isDialogueActive = true;
        isTalking = true;

        // Damage overrides talking
        if (damageRoutine != null)
            return;

        // Start alternating between idle and talking sprites
        if (talkingRoutine != null)
            StopCoroutine(talkingRoutine);

        talkingRoutine = StartCoroutine(TalkingAnimation());
    }

    IEnumerator TalkingAnimation()
    {
        while (isTalking)
        {
            portraitImage.sprite = talkingPortrait;
            yield return new WaitForSeconds(0.1f);

            portraitImage.sprite = idlePortrait;
            yield return new WaitForSeconds(0.1f);
        }
    }

    // ---------------------------------------------------------
    // STOP TALKING (called when typing finishes)
    // ---------------------------------------------------------
    public void StopTalking()
    {
        isTalking = false;

        if (talkingRoutine != null)
            StopCoroutine(talkingRoutine);

        // Damage still overrides
        if (damageRoutine != null)
            return;

        // After typing ends → show idle portrait
        portraitImage.sprite = idlePortrait;
    }

    // ---------------------------------------------------------
    // DIALOGUE CLOSED (called after fade-out)
    // ---------------------------------------------------------
    public void OnDialogueClosed()
    {
        isDialogueActive = false;
        isTalking = false;

        // Stop talking animation if running
        if (talkingRoutine != null)
            StopCoroutine(talkingRoutine);

        // If damage portrait is active, wait for it to finish
        if (damageRoutine != null)
            return;

        // ⭐ FORCE SWITCH TO DEFAULT
        portraitImage.sprite = defaultPortrait;
    }


    // ---------------------------------------------------------
    // DAMAGE OVERRIDE
    // ---------------------------------------------------------
    public void OnPlayerDamaged()
    {
        if (damageRoutine != null)
            StopCoroutine(damageRoutine);

        damageRoutine = StartCoroutine(DamagePortraitTimer());
    }

    IEnumerator DamagePortraitTimer()
    {
        portraitImage.sprite = damagePortrait;

        yield return new WaitForSeconds(damageDisplayTime);

        damageRoutine = null;

        // If dialogue is active and talking → resume talking animation
        if (isDialogueActive && isTalking)
        {
            if (talkingRoutine != null)
                StopCoroutine(talkingRoutine);

            talkingRoutine = StartCoroutine(TalkingAnimation());
            yield break;
        }

        // If dialogue active but not talking → idle portrait
        if (isDialogueActive)
        {
            portraitImage.sprite = idlePortrait;
            yield break;
        }

        // Otherwise → default portrait
        portraitImage.sprite = defaultPortrait;
    }
}
