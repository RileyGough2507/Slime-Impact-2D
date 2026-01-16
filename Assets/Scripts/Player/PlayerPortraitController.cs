using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPortraitController : MonoBehaviour
{
    [Header("Portrait Sprites")]
    public Sprite defaultPortrait;
    public Sprite damagePortrait;
    public Sprite talkingPortrait;   // ⭐ NEW — assign your talking sprite here

    [Header("UI Reference")]
    public Image portraitImage;

    [Header("Settings")]
    public float damageDisplayTime = 3f;

    private Coroutine damageRoutine;
    private bool isTalking = false;

    void Start()
    {
        // Set default portrait on start
        portraitImage.sprite = defaultPortrait;
    }

    // -----------------------------
    // TALKING CONTROL (Dialogue)
    // -----------------------------
    public void StartTalking()
    {
        if (damageRoutine != null)
            return; // Damage portrait overrides talking

        isTalking = true;

        if (talkingPortrait != null)
            portraitImage.sprite = talkingPortrait;
    }

    public void StopTalking()
    {
        if (damageRoutine != null)
            return; // Still showing damage portrait

        isTalking = false;

        if (defaultPortrait != null)
            portraitImage.sprite = defaultPortrait;
    }

    // -----------------------------
    // DAMAGE CONTROL (Overrides talking)
    // -----------------------------
    public void OnPlayerDamaged()
    {
        if (damageRoutine != null)
            StopCoroutine(damageRoutine);

        damageRoutine = StartCoroutine(DamagePortraitTimer());
    }

    private IEnumerator DamagePortraitTimer()
    {
        portraitImage.sprite = damagePortrait;

        yield return new WaitForSeconds(damageDisplayTime);

        damageRoutine = null;

        // If dialogue is still talking, return to talking portrait
        if (isTalking && talkingPortrait != null)
        {
            portraitImage.sprite = talkingPortrait;
        }
        else
        {
            portraitImage.sprite = defaultPortrait;
        }
    }
}
