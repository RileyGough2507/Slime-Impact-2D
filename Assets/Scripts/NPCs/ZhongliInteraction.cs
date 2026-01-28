using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    [Header("Player Detection")]
    public Transform player;
    public float interactRadius = 3f;

    [Header("UI Elements")]
    public GameObject pressFIcon;               // Icon above NPC
    public GameObject dialogueUIRoot;           // Root of DialogueTyper UI
    public DialogueTyper dialogueTyper;         // Reference to DialogueTyper script

    [Header("Dialogue")]
    [TextArea]
    public string dialogueText;

    [Header("Mission")]
    public string missionText = "Talk to the village elder.";

    private bool playerInRange = false;
    private bool dialogueActive = false;
    private bool hasTalkedOnce = false;         // 🔥 NEW — prevents F icon from showing again

    private BoxCollider2D npcCollider;
    private SpriteRenderer sr;

    void Start()
    {
        npcCollider = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();

        if (pressFIcon != null)
            pressFIcon.SetActive(false);

        if (dialogueUIRoot != null)
            dialogueUIRoot.SetActive(false);
    }

    void Update()
    {
        FacePlayer();

        float distance = Vector2.Distance(transform.position, player.position);

        // If already talked once → never show F again
        if (hasTalkedOnce)
            return;

        if (distance <= interactRadius && !dialogueActive)
        {
            if (!playerInRange)
            {
                playerInRange = true;
                pressFIcon.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.F))
                StartDialogue();
        }
        else
        {
            if (playerInRange)
            {
                playerInRange = false;
                pressFIcon.SetActive(false);
            }
        }
    }

    void FacePlayer()
    {
        if (player == null || sr == null)
            return;

        // 🔥 Corrected facing logic (Zhongli now looks AT the player)
        sr.flipX = player.position.x > transform.position.x;
    }

    void StartDialogue()
    {
        dialogueActive = true;
        pressFIcon.SetActive(false);

        if (dialogueUIRoot != null)
            dialogueUIRoot.SetActive(true);

        dialogueTyper.fullText = dialogueText;
        dialogueTyper.StartTyping();

        StartCoroutine(WaitForDialogueToClose());
    }

    System.Collections.IEnumerator WaitForDialogueToClose()
    {
        while (dialogueTyper != null && !dialogueTyper.hasClosedOnce)
            yield return null;

        dialogueActive = false;

        // 🔥 Prevent future interaction
        hasTalkedOnce = true;

        // 🔥 Disable collider so player can walk through
        if (npcCollider != null)
            npcCollider.enabled = false;

        // Trigger mission
        MissionObjectiveUI.instance.SetObjective(missionText);

        // ⭐ Give player the shield buff
        PlayerShield shield = player.GetComponent<PlayerShield>();
        if (shield != null)
            shield.ActivateShield();

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
