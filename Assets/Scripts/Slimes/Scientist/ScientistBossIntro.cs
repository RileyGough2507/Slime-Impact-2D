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
    public Transform suitObject;       // Suit visual object
    public float stopDistance = 1f;    // Distance to stop near suit

    [Header("Boss")]
    public GameObject bossObject;      // Boss sprite/prefab to enable
    public GameObject bossFightScriptObject;

    [Header("Movement")]
    public float walkSpeed = 6f;

    private bool cutsceneRunning = false;
    private bool walkingRight = false;
    private bool suitEntered = false;

    void Start()
    {
        if (exclamationMark != null)
            exclamationMark.SetActive(false);

        if (bossObject != null)
            bossObject.SetActive(false);

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
        player.enabled = false;
        player.rb.velocity = Vector2.zero;

        cam.SetTarget(cameraFocusPoint);
        yield return new WaitForSeconds(1f);

        scientistSprite.flipX = true;
        exclamationMark.SetActive(true);
        audioSource.PlayOneShot(alertSound);
        yield return new WaitForSeconds(1f);
        exclamationMark.SetActive(false);

        scientistSprite.flipX = false;
        scientistAnim.Play(moveAnim, 0, 0f);
        walkingRight = true;

        cam.SetTarget(scientist);
    }

    void Update()
    {
        if (walkingRight)
        {
            scientist.Translate(Vector3.right * walkSpeed * Time.deltaTime, Space.World);

            float distance = Vector2.Distance(scientist.position, suitObject.position);

            if (distance <= stopDistance && !suitEntered)
            {
                walkingRight = false;
                suitEntered = true;

                suitObject.gameObject.SetActive(false);

                scientistAnim.Play(enterSuitAnim, 0, 0f);
                audioSource.PlayOneShot(enterSuitSound);

                StartCoroutine(HandleBossAppearance());
            }
        }
    }

    IEnumerator HandleBossAppearance()
    {
        yield return new WaitForSeconds(1.5f);

        scientist.gameObject.SetActive(false);

        if (bossObject != null)
        {
            bossObject.transform.position = scientist.position;
            bossObject.SetActive(true);
        }

        if (bossFightScriptObject != null)
            bossFightScriptObject.SetActive(true);

        cam.SetTarget(player.transform);
        yield return new WaitForSeconds(0.5f);

        player.enabled = true;
    }
}
