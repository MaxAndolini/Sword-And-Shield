using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    [Header("Camera")]
    // camera and rotation
    public Transform cameraHolder;

    public float mouseSensitivity = 2f;
    public float upLimit = -20;
    public float downLimit = 20;

    [Header("Speed")]
    // character speed
    public float normalSpeed = 1.8f;

    public float sprintSpeed = 5;

    [Header("Health")]
    // character health
    public Slider healthSlider;

    public bool isDead;
    public bool defense;
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    [Header("Shield")]
    // shield
    public GameObject shield;

    public Slider shieldSlider;
    public float maxShieldDurability = 100f;
    public float currentShieldDurability = 100f;

    [Header("Damage")]
    // damage
    public float attackDistance;

    public float damage;
    public float attackCoolDown = 0.9f;

    [Header("Pickup")]
    // pickup
    public GameObject pickup;

    [Header("Dash")]
    // dash
    public Slider dashSlider;

    public float maxDash = 100f;
    public float currentDash;
    public float dashSpeed;
    public float dashTime;
    public float dashRegenerationTime;
    public bool dashActive;

    [Header("Power")]
    // power
    public GameObject power;

    public Slider powerSlider;
    public float maxPower = 100f;
    public float currentPower;
    public float powerDamage;
    public float powerTime;
    public float powerRegenerationTime;
    public bool powerActive;

    [Header("Score")]
    // score
    public int score;

    public Text scoreText;

    // gravity
    private readonly float gravity = 9.87f;

    // pickup spawn points
    private readonly float[,] pickupSpawnPoints =
    {
        {758.47f, 292.38f}, {748.37f, 297.57f}, {738.05f, 302.27f}, {745.42f, 312.4f}, {753.61f, 305.7f},
        {753.61f, 315.43f}, {759.49f, 306.64f}, {746.26f, 326.71f}, {731.15f, 326.71f}, {731.15f, 341.72f},
        {742.7f, 341.72f}, {757.9f, 341.72f}, {756.21f, 349.75f}, {763.34f, 349.75f}, {776.89f, 349.75f},
        {790.74f, 349.75f}, {802.71f, 358.65f}, {802.71f, 339.87f}, {792.91f, 331.58f}, {797.49f, 324.83f},
        {806.17f, 319.53f}, {814.21f, 314.83f}, {812.74f, 304.28f}, {798.78f, 308.4f}, {792.3f, 312.98f},
        {792.3f, 304.32f}, {792.3f, 297.3f}, {783.55f, 297.3f}, {783.55f, 312.73f}, {777.86f, 298.47f}
    };

    // animator
    private Animator animator;

    private bool canAttack = true;
    private CharacterController characterController;
    private int clicks;
    private float currentDashTime;
    private float currentPowerTime;
    private AudioSource footStep;
    private float lastFootStep;
    private int locked;
    private float verticalSpeed;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 2f);
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        currentShieldDurability = maxShieldDurability;
        shieldSlider.maxValue = maxShieldDurability;
        shieldSlider.value = currentShieldDurability;
        if (currentShieldDurability <= 0) shield.SetActive(false);

        currentDash = maxDash;
        dashSlider.maxValue = maxDash;
        dashSlider.value = currentDash;
        currentDashTime = dashTime * currentDash / maxDash;

        currentPower = maxPower;
        powerSlider.maxValue = maxPower;
        powerSlider.value = currentPower;
        currentPowerTime = powerTime * currentPower / maxPower;

        StartCoroutine(NewPickup());
    }

    private void Update()
    {
        if (GameStart.GameStarted && !Pause.GamePaused && !GameOver.GameFinished && !isDead)
        {
            Move();
            Rotate();
            Power();
            StartCoroutine(AttackRoutine());
            StartCoroutine(DefenseRoutine());
        }
        else
        {
            if (footStep != null && footStep.isPlaying) Destroy(footStep);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("Pickup")) return;
        if (!(currentHealth < maxHealth)) return;
        GiveHealth(30f);
        GiveShield(15f);
        Destroy(other.gameObject);
        StartCoroutine(NewPickup());
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Pickup);
    }

    private void Rotate()
    {
        var horizontalRotation = Input.GetAxis("Mouse X");
        var verticalRotation = Input.GetAxis("Mouse Y");

        transform.Rotate(0, horizontalRotation * mouseSensitivity, 0);
        cameraHolder.Rotate(-verticalRotation * mouseSensitivity, 0, 0);

        var currentRotation = cameraHolder.localEulerAngles;
        if (currentRotation.x > 180) currentRotation.x -= 360;
        currentRotation.x = Mathf.Clamp(currentRotation.x, upLimit, downLimit);
        cameraHolder.localRotation = Quaternion.Euler(currentRotation);
    }

    private void Move()
    {
        var horizontalMove = Input.GetAxis("Horizontal");
        var verticalMove = Input.GetAxis("Vertical");
        bool sprint = false, jump = false;

        if (Input.GetKey(KeyCode.LeftShift)) sprint = true;
        if (characterController.isGrounded) verticalSpeed = 0;
        else verticalSpeed -= gravity * Time.deltaTime;
        var gravityMove = new Vector3(0, verticalSpeed, 0);

        var newSpeed = !sprint ? normalSpeed : sprintSpeed;
        var transform1 = transform;
        var move = transform1.forward * verticalMove + transform1.right * horizontalMove;

        if (Input.GetKey(KeyCode.Space) && !sprint)
        {
            var transform2 = transform;
            move += transform2.up * 2.4f + transform2.forward * 0.5f;
            jump = true;
        }

        if (dashActive)
        {
            if (currentDashTime > 0)
            {
                characterController.Move(dashSpeed * Time.deltaTime * move + gravityMove * Time.deltaTime);
                currentDashTime -= Time.deltaTime;
                currentDash -= maxDash / dashTime * Time.deltaTime;
                dashSlider.value = currentDash;
            }
            else
            {
                dashActive = false;
                currentDash = 0;
            }
        }
        else
        {
            if (currentDash >= maxDash)
            {
                currentDash = maxDash;
                dashSlider.value = currentDash;
            }
            else
            {
                currentDash += maxDash / dashRegenerationTime * Time.deltaTime;
                dashSlider.value = currentDash;
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && !dashActive && !jump && move != Vector3.zero)
        {
            currentDashTime = dashTime * currentDash / maxDash;
            if (currentDashTime > 0)
            {
                FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Dash);
                dashActive = true;
            }
        }
        else
        {
            characterController.Move(newSpeed * Time.deltaTime * move + gravityMove * Time.deltaTime);
        }

        if (verticalMove != 0 || horizontalMove != 0 || jump)
        {
            var characterAnim = 1; // Walking
            if (!jump && sprint) characterAnim = 3; // Sprint
            if (jump) characterAnim = 6; // Jump

            animator.SetInteger("condition", characterAnim);

            if (footStep == null) footStep = FindObjectOfType<Audio>().PlayWalk(gameObject);

            if (characterAnim == 3) footStep.loop = true;

            if (lastFootStep + 0.5f < Time.time && GameStart.GameStarted)
            {
                lastFootStep = Time.time;
                if (!footStep.isPlaying) footStep.Play();
            }

            locked = 1;
        }
        else if (locked != 2)
        {
            animator.SetInteger("condition", 0); // Idle
            Destroy(footStep);
            locked = 0;
        }
    }

    public virtual void CheckHealth()
    {
        if (currentHealth >= maxHealth) currentHealth = maxHealth;
        if (currentHealth <= 0 && !isDead)
        {
            animator.SetTrigger("die");
            currentHealth = 0;
            isDead = true;
            FindObjectOfType<Audio>().PlayOnce(Audio.Audios.CharacterDie);
        }
    }

    public void GiveHealth(float health)
    {
        if (currentHealth + health > maxHealth) health = maxHealth;
        currentHealth += health;
        healthSlider.value = currentHealth;
    }

    public void TakeDamage(float damage)
    {
        if (defense == false && !isDead)
        {
            currentHealth -= damage;
            animator.SetTrigger("takeDamage");

            healthSlider.value = currentHealth;
            FindObjectOfType<Audio>().PlayOnce(Audio.Audios.CharacterHurt);
        }
    }

    private void Power()
    {
        if (powerActive)
        {
            if (currentPowerTime > 0)
            {
                currentPowerTime -= Time.deltaTime;
                currentPower -= maxPower / powerTime * Time.deltaTime;
                powerSlider.value = currentPower;
            }
            else
            {
                FindObjectOfType<Audio>().PlayOnce(Audio.Audios.PowerOff);
                powerActive = false;
                power.SetActive(false);
                currentPower = 0;
            }
        }
        else
        {
            if (currentPower >= maxPower)
            {
                currentPower = maxPower;
                powerSlider.value = currentPower;
            }
            else
            {
                currentPower += maxPower / powerRegenerationTime * Time.deltaTime;
                powerSlider.value = currentPower;
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (powerActive)
            {
                FindObjectOfType<Audio>().PlayOnce(Audio.Audios.PowerOff);
                powerActive = false;
                power.SetActive(false);
            }
            else
            {
                currentPowerTime = powerTime * currentPower / maxPower;
                if (currentPowerTime > 0)
                {
                    FindObjectOfType<Audio>().PlayOnce(Audio.Audios.PowerOn);
                    powerActive = true;
                    power.SetActive(true);
                }
            }
        }
    }

    private IEnumerator AttackRoutine()
    {
        if (!Input.GetMouseButtonDown(0) || locked != 0) yield break;
        switch (clicks)
        {
            case 0:
                animator.SetInteger("condition", 2); // Attack
                FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Sword);
                AttackEnemy();
                locked = 2;
                yield return new WaitForSeconds(1);
                animator.SetInteger("condition", 0); // Idle
                locked = 0;
                clicks++;

                break;
            case 1:
                animator.SetInteger("condition", 4); // Attack
                FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Sword);
                AttackEnemy();
                locked = 2;
                yield return new WaitForSeconds(1);
                animator.SetInteger("condition", 0); // Idle
                locked = 0;
                clicks = 0;

                break;
        }
    }

    private void AttackEnemy()
    {
        if (!canAttack) return;
        var targets = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in targets)
        {
            var distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (!(distance < attackDistance)) continue;
            if (enemy.GetComponent<EnemyAI>().isDead) continue;
            var hurtEnemy = damage;
            if (powerActive) hurtEnemy = powerDamage;
            StartCoroutine(enemy.GetComponent<EnemyHealthManager>().HurtEnemy(hurtEnemy));
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator DefenseRoutine()
    {
        if (!Input.GetMouseButtonDown(1) || locked != 0 || currentShieldDurability <= 0) yield break;

        animator.SetInteger("condition", 5); // Defense
        defense = true;
        locked = 2;
        yield return new WaitForSeconds(1);
        animator.SetInteger("condition", 0); // Idle
        defense = false;
        locked = 0;
    }

    private IEnumerator NewPickup()
    {
        yield return new WaitForSeconds(10f);
        var pos = Random.Range(0, pickupSpawnPoints.Length / 2);
        Instantiate(pickup, new Vector3(pickupSpawnPoints[pos, 0], 1.097f, pickupSpawnPoints[pos, 1]),
            Quaternion.identity);
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCoolDown);
        canAttack = true;
    }

    public void AddScore()
    {
        score++;
        scoreText.text = "SCORE: " + score;
    }

    private void GiveShield(float health)
    {
        if (currentShieldDurability + health > maxShieldDurability) health = maxShieldDurability;
        currentShieldDurability += health;
        shieldSlider.value = currentShieldDurability;
        if (currentShieldDurability > 0) shield.SetActive(true);
    }

    public virtual void CheckShieldDurability()
    {
        if (currentShieldDurability >= maxShieldDurability) currentShieldDurability = maxShieldDurability;
        if (currentShieldDurability <= 0)
        {
            shield.SetActive(false);
            currentShieldDurability = 0;
        }
        else
        {
            shield.SetActive(true);
        }
    }

    public void ShieldTakeDamageToDurability(float shieldDamage)
    {
        if (!defense) return;
        FindObjectOfType<Audio>().PlayOnce(Audio.Audios.Shield);
        currentShieldDurability -= shieldDamage / 2;
        shieldSlider.value = currentShieldDurability < 0 ? 0 : currentShieldDurability;
    }
}