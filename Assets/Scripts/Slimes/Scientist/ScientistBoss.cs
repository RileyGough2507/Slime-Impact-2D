using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScientistBoss : MonoBehaviour
{
    public enum BossState
    {
        Idle,
        Moving,
        Laser,
        Missile,
        ChargeSpin,
        Spinning,
        Dead
    }

    [Header("Core References")]
    public PlayerController2D player;
    public CameraFollow cam;
    public Slider healthSlider;
    public SpriteRenderer spriteRenderer;
    public Animator anim;
    public AudioSource audioSource;

    [Header("Room Bounds")]
    public Transform leftBound;
    public Transform rightBound;

    [Header("Movement")]
    public float moveSpeed = 6f;
    public float keepDistance = 8f;

    [Header("Health")]
    public int maxHealth = 40;
    public Sprite deadSprite;

    [Header("Animation Names")]
    public string walkLeftAnim;
    public string walkRightAnim;
    public string laserLeftAnim;
    public string laserRightAnim;
    public string missileLeftAnim;
    public string missileRightAnim;
    public string chargeLeftAnim;
    public string chargeRightAnim;
    public string spinAnim;
    public string deathAnim;

    [Header("Laser Attack")]
    public Transform laserFirePoint;
    public GameObject laserPrefab;
    public float laserDistance = 10f;
    public float laserCooldown = 5f;
    public AudioClip laserChargeSfx;
    public AudioClip laserFireSfx;

    [Header("Missile Attack")]
    public Transform missileFirePoint;
    public GameObject missilePrefab;
    public float missileCooldown = 20f;
    public AudioClip missileFireSfx;

    [Header("Spin Attack")]
    public float spinCooldown = 40f;
    public float spinSpeed = 12f;
    public AudioClip spinSfx;

    [Header("Death")]
    public AudioClip deathSfx;

    private BossState state = BossState.Idle;
    private int currentHealth;
    private float laserTimer;
    private float missileTimer;
    private float spinTimer;
    private bool facingRight = true;
    private bool isAttacking = false;
    private bool isDead = false;

    private Vector3 spinTarget;

    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        laserTimer = 0f;
        missileTimer = 0f;
        spinTimer = 0f;
    }

    void Update()
    {
        if (isDead) return;

        laserTimer += Time.deltaTime;
        missileTimer += Time.deltaTime;
        spinTimer += Time.deltaTime;

        float dist = Mathf.Abs(player.transform.position.x - transform.position.x);

        switch (state)
        {
            case BossState.Idle:
            case BossState.Moving:
                HandleMovement(dist);
                HandleAttacks(dist);
                break;

            case BossState.Spinning:
                HandleSpinMovement();
                break;
        }
    }

    // ---------------------------------------------------------
    // MOVEMENT
    // ---------------------------------------------------------
    void HandleMovement(float dist)
    {
        if (isAttacking) return;

        state = BossState.Moving;

        float playerX = player.transform.position.x;
        float myX = transform.position.x;

        facingRight = playerX > myX;
        spriteRenderer.flipX = !facingRight;

        float dir = 0f;

        if (dist < keepDistance)
            dir = facingRight ? -1f : 1f;
        else if (dist > keepDistance + 1f)
            dir = facingRight ? 1f : -1f;

        float nextX = transform.position.x + dir * moveSpeed * Time.deltaTime;

        if (nextX < leftBound.position.x)
        {
            dir = 1f;
            facingRight = true;
            spriteRenderer.flipX = false;
        }
        else if (nextX > rightBound.position.x)
        {
            dir = -1f;
            facingRight = false;
            spriteRenderer.flipX = true;
        }

        if (dir != 0f)
        {
            transform.Translate(Vector3.right * dir * moveSpeed * Time.deltaTime);

            anim.Play(facingRight ? walkRightAnim : walkLeftAnim);
        }
    }

    // ---------------------------------------------------------
    // ATTACK LOGIC
    // ---------------------------------------------------------
    void HandleAttacks(float dist)
    {
        if (isAttacking) return;

        if (spinTimer >= spinCooldown && dist > laserDistance)
        {
            StartCoroutine(DoSpin());
            return;
        }

        if (missileTimer >= missileCooldown)
        {
            StartCoroutine(DoMissile());
            return;
        }

        if (dist > laserDistance && laserTimer >= laserCooldown)
        {
            StartCoroutine(DoLaser());
            return;
        }
    }

    // ---------------------------------------------------------
    // LASER ATTACK
    // ---------------------------------------------------------
    IEnumerator DoLaser()
    {
        isAttacking = true;
        state = BossState.Laser;
        laserTimer = 0f;

        facingRight = player.transform.position.x > transform.position.x;
        spriteRenderer.flipX = !facingRight;

        if (laserChargeSfx != null)
            audioSource.PlayOneShot(laserChargeSfx);

        anim.Play(facingRight ? laserRightAnim : laserLeftAnim);

        yield return new WaitForSeconds(1.5f);

        isAttacking = false;
        state = BossState.Idle;
    }

    // Called by animation event
    public void FireLaserEvent()
    {
        Instantiate(laserPrefab, laserFirePoint.position, laserFirePoint.rotation);

        if (laserFireSfx != null)
            audioSource.PlayOneShot(laserFireSfx);
    }

    // ---------------------------------------------------------
    // MISSILE ATTACK
    // ---------------------------------------------------------
    IEnumerator DoMissile()
    {
        isAttacking = true;
        state = BossState.Missile;
        missileTimer = 0f;

        facingRight = player.transform.position.x > transform.position.x;
        spriteRenderer.flipX = !facingRight;

        anim.Play(facingRight ? missileRightAnim : missileLeftAnim);

        yield return new WaitForSeconds(0.5f);

        Vector3 target = player.transform.position;

        GameObject missile = Instantiate(missilePrefab, missileFirePoint.position, Quaternion.identity);

        // Your missile script should handle falling toward target.x
        // Example: missile.GetComponent<Missile>().Init(target);

        if (missileFireSfx != null)
            audioSource.PlayOneShot(missileFireSfx);

        yield return new WaitForSeconds(0.7f);

        isAttacking = false;
        state = BossState.Idle;
    }

    // ---------------------------------------------------------
    // SPIN ATTACK
    // ---------------------------------------------------------
    IEnumerator DoSpin()
    {
        isAttacking = true;
        state = BossState.ChargeSpin;
        spinTimer = 0f;

        facingRight = player.transform.position.x > transform.position.x;
        spriteRenderer.flipX = !facingRight;

        anim.Play(facingRight ? chargeRightAnim : chargeLeftAnim);

        yield return new WaitForSeconds(0.7f);

        state = BossState.Spinning;

        if (spinSfx != null)
            audioSource.PlayOneShot(spinSfx);

        spinTarget = facingRight ? rightBound.position : leftBound.position;

        anim.Play(spinAnim);
    }

    void HandleSpinMovement()
    {
        float dir = facingRight ? 1f : -1f;

        transform.Translate(Vector3.right * dir * spinSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - spinTarget.x) < 0.1f)
        {
            isAttacking = false;
            state = BossState.Idle;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (state == BossState.Spinning && other.CompareTag("Player"))
            player.TakeDamage(1);
    }

    // ---------------------------------------------------------
    // DAMAGE + DEATH
    // ---------------------------------------------------------
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            StartCoroutine(DoDeath());
        }
    }

    IEnumerator DoDeath()
    {
        isDead = true;
        isAttacking = false;
        state = BossState.Dead;

        cam.SetTarget(transform);

        if (deathSfx != null)
            audioSource.PlayOneShot(deathSfx);

        anim.Play(deathAnim);

        yield return new WaitForSeconds(2f);

        healthSlider.gameObject.SetActive(false);

        anim.enabled = false;
        spriteRenderer.sprite = deadSprite;

        yield return new WaitForSeconds(1f);

        cam.SetTarget(player.transform);

        this.enabled = false;
    }

    // Called by PlayerController2D when player dies
    public void OnPlayerDied()
    {
        if (isDead) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + 10);
        healthSlider.value = currentHealth;
    }
}
