using UnityEngine;
using System.Collections;

public class ScientistBossIntro : MonoBehaviour
{
    [Header("References")]
    public PlayerController2D player;
    public CameraFollow cam;
    public Transform scientist;
    public SpriteRenderer scientistSprite;
    public Animator scientistAnim;
    public Transform cameraFocusPoint;
    public GameObject exclamationMark;
    public AudioSource audioSource;
    public AudioClip alertSound;
    public AudioClip enterSuitSound;

    [Header("Animation Names")]
    public string moveAnim;
    public string enterSuitAnim;

    [Header("Suit")]
    public GameObject suitObject;

    [Header("Boss Fight")]
    public GameObject bossFightScriptObject;

    private bool cutsceneRunning = false;
    private bool walkingRight = false;

    void Start()
    {
        if (exclamationMark != null)
            exclamationMark.SetActive(false);

        if (bossFightScriptObject != null)
            bossFightScriptObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!cutsceneRunning && other.CompareTag("Player"))
        {
            cutsceneRunning = true;
            StartCoroutine(Cutscene());
        }
    }

    IEnumerator Cutscene()
    {
        // Freeze player
        player.enabled = false;
        player.rb.velocity = Vector2.zero;

        // Move camera to scientist
        cam.SetTarget(cameraFocusPoint);
        yield return new WaitForSeconds(1f);

        // Flip scientist to face player
        scientistSprite.flipX = true;

        // Exclamation mark + sound
        exclamationMark.SetActive(true);
        audioSource.PlayOneShot(alertSound);
        yield return new WaitForSeconds(1f);
        exclamationMark.SetActive(false);

        // Start walking RIGHT
        scientistSprite.flipX = false;
        scientistAnim.Play(moveAnim, 0, 0f);
        walkingRight = true;

        // Camera follows scientist
        cam.SetTarget(scientist);
    }

    void Update()
    {
        if (walkingRight)
        {
            scientist.Translate(Vector3.right * 6f * Time.deltaTime, Space.World);
            scientistAnim.Play(moveAnim, 0, 0f);
        }
    }

    // ⭐ THIS is the method SuitTrigger.cs needs
    public void OnScientistHitSuit()
    {
        walkingRight = false; // stop movement immediately

        // Hide suit
        if (suitObject != null)
            suitObject.SetActive(false);

        // Play enter suit animation with full priority
        scientistAnim.Play(enterSuitAnim, 0, 0f);
        audioSource.PlayOneShot(enterSuitSound);

        StartCoroutine(ActivateBossFight());
    }

    IEnumerator ActivateBossFight()
    {
        yield return new WaitForSeconds(1.5f);

        if (bossFightScriptObject != null)
            bossFightScriptObject.SetActive(true);

        cam.SetTarget(player.transform);
        yield return new WaitForSeconds(0.5f);

        player.enabled = true;
    }
}
