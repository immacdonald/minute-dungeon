using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

[RequireComponent(typeof(Controller2D))]
public class Player : Singleton<Player> {

    [Header("Basic Movement")]
    public float baseMoveSpeed = 4;
    public float sprintMultiplier = 1.6f;
    float moveSpeed = 4;

    [Header("Jumping")]
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = 0.4f;
    float accelerationTimeAirborne = 0.2f;
    float accelerationTimeGrounded = 0.1f;

    [Header("Wall Jumping & Sliding")]
    bool wallSliding = false;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = 0.25f;
    float timeToWallUnstick;
    int wallDirX = 0;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    PlayerInputManager inputManager;
    Material material;

    [Header("Dashing")]
    public float dashTime = 0.5f;
    public float dashForce = 10f;
    bool dashing = false;
    private Vector3 dashVelocity;
    private TrailRenderer dashTrail;

    [Header("Attack")]
    public bool canAttack = true;
    public AnimationClip attackAnimation;

    [Header("Skill Options")]
    public int maximumJumps = 1;
    int jumpsRemaining;
    bool hasDash = true;
    public bool wallHoldEnabled = true;
    public bool wallJumpingEnabled = true;
    public bool dashEnabled = true;

    [Header("Health")]
    [SerializeField] private int health;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private Transform healthUI;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    [Header("Sound")]
    [SerializeField] private AudioSource walkSound;
    [SerializeField] private AudioSource jumpSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource hurtSound;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private AudioSource enemyDeathSound;

    [Header("Money")]
    [SerializeField] private int coins;
    [SerializeField] private TextMeshProUGUI coinText;

    [Header("Interaction")]
    [SerializeField] private LayerMask interactionMask;
    [SerializeField] private LayerMask attackInteractionMask;
    private bool damageImmune = false;

    [Header("Camera Volumes")]
    [SerializeField] private LayerMask cameraVolumes;
    [SerializeField] private CameraVolume globalCameraVolume;
    private List<CameraVolume> currentCameraVolumes = new List<CameraVolume>(8);
    [SerializeField] private CinemachineConfiner2D cinemachineContraints;
    [SerializeField] private CinemachineVirtualCamera cinemachineCamera;

    Transform visual;
    Animator animator;
    bool isWalking;
    bool isSprinting;
    bool isAirborne;
    bool jumpKeyDownQueued;
    bool jumpKeyUpQueued;
    [HideInInspector] public bool justJumped = false;

    void Start() {
        controller = GetComponent<Controller2D>();
        inputManager = GetComponent<PlayerInputManager>();
        visual = transform.GetChild(0);
        animator = visual.GetComponent<Animator>();
        material = visual.GetComponentInChildren<Renderer>().sharedMaterial;
        dashTrail = GetComponentInChildren<TrailRenderer>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        currentCameraVolumes.Add(globalCameraVolume);
        SetMaxHealth(maxHealth);
        ModifyCoins(0);
    }

    private void Update() {
        if (GameManager.Instance.gameState == GameState.Regular) {
            if (inputManager.JumpKeyDown)
                jumpKeyDownQueued = true;
            if (inputManager.JumpKeyUp)
                jumpKeyUpQueued = true;
            inputManager.ResetJump();

            AnimateCharacter(velocity, (wallDirX == -1 && wallSliding) ? true : false);
        }
    }

    void FixedUpdate() {
        if (GameManager.Instance.gameState == GameState.Regular) {
            isSprinting = inputManager.Sprint;
            moveSpeed = baseMoveSpeed * (isSprinting ? sprintMultiplier : 1);
            Vector2 input = new Vector2(inputManager.HorizontalMotion, inputManager.VerticallMotion);
            wallDirX = controller.collisions.left ? -1 : controller.collisions.right ? 1 : 0;

            float targetVelocityX = input.x * moveSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

            if(controller.collisions.below)
            {
                jumpsRemaining = maximumJumps;
                hasDash = true;
            }

            // Re-check wall sliding each frame
            wallSliding = false;
            if (wallHoldEnabled && (controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y <= float.Epsilon) {
                wallSliding = true;
                if (timeToWallUnstick > 0) {
                    velocityXSmoothing = 0;
                    velocity.x = 0;
                    if (input.x != wallDirX && input.x != 0) {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                    else {
                        timeToWallUnstick = wallStickTime;
                    }
                }
                else {
                    timeToWallUnstick = wallStickTime;
                }

            }
           
            if (jumpKeyDownQueued) {
                jumpKeyDownQueued = false;
                if (wallSliding) {
                    if (wallDirX == input.x) {
                        velocity.x = -wallDirX * wallJumpClimb.x;
                        velocity.y = wallJumpClimb.y;
                    }
                    else if (input.x == 0) {
                        velocity.x = -wallDirX * wallJumpOff.x;
                        velocity.y = wallJumpOff.y;
                    }
                    else {
                        velocity.x = -wallDirX * wallLeap.x;
                        velocity.y = wallLeap.y;
                    }
                    // Forcibly set wall sliding to false to prevent an extra frame of the wall slide animation
                    wallSliding = false;
                } else if (controller.collisions.below || jumpsRemaining > 0) {
                    velocity.y = maxJumpVelocity;
                    StartCoroutine(JustJumped());
                    jumpsRemaining--;
                } else if (!controller.collisions.below && jumpsRemaining == 0 && hasDash && dashEnabled)
                {
                    hasDash = false;
                    StartCoroutine(Dash());
                }
            }
            if (jumpKeyUpQueued) {
                jumpKeyUpQueued = false;
                if (velocity.y > minJumpVelocity) {
                    velocity.y = minJumpVelocity;
                }
            }

            velocity.y += gravity * Time.deltaTime;
            if (wallSliding && velocity.y < -wallSlideSpeedMax)
            {
                velocity.y = -wallSlideSpeedMax;
            }
            if (dashing) {
                velocity = dashVelocity;
            }
            controller.Move(velocity * Time.deltaTime, input);

            if (controller.collisions.above || controller.collisions.below) {
                velocity.y = 0;
            }

            isAirborne = !controller.collisions.below;
            isWalking = (input.x != 0) && !isAirborne;
        }

    }

    IEnumerator JustJumped() {
        jumpSound.Play();
        justJumped = true;
        yield return new WaitForSeconds(0.1f);
        justJumped = false;
    }

    IEnumerator Dash() {
        dashing = true;
        dashVelocity = new Vector3(velocity.x > 0 ? 1 : -1, 0, 0) * dashForce;
        dashTrail.emitting = true;
        yield return new WaitForSeconds(dashTime);
        dashTrail.emitting = false;
        dashVelocity = Vector3.zero;
        dashing = false;
    }

    void AnimateCharacter(Vector3 direction, bool wallSlideOverrideFix)
    {
        if (direction.x > 0)
        {
            visual.transform.rotation = Quaternion.Euler(visual.transform.rotation.x, 0, visual.transform.rotation.z);
        }
        else if (direction.x < 0)
        {
            visual.transform.rotation = Quaternion.Euler(visual.transform.rotation.x, 180, visual.transform.rotation.z);
        }

        animator.SetBool("isWalking", isWalking && !wallSliding && !isSprinting);
        animator.SetBool("isSprinting", isSprinting && isWalking && !wallSliding);
        animator.SetBool("isAirborne", isAirborne);
        animator.SetBool("isWallSliding", wallSliding);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"Collided with {collision.transform.name}");
        if ((cameraVolumes.value & (1 << collision.gameObject.layer)) > 0)
        {
            if (collision.transform.name == "Global Camera Volume")
            {
                return;
            }

            currentCameraVolumes.Add(collision.gameObject.GetComponent<CameraVolume>());
            cinemachineContraints.m_BoundingShape2D = currentCameraVolumes[currentCameraVolumes.Count - 1].volume;
            cinemachineCamera.m_Lens.OrthographicSize = currentCameraVolumes[currentCameraVolumes.Count - 1].GetCameraSize();
        } else if ((interactionMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            Interaction(collision.transform.GetComponent<IInteractable>().Interact());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((cameraVolumes.value & (1 << collision.gameObject.layer)) > 0)
        {
            if (collision.transform.name == "Global Camera Volume")
            {
                return;
            }

            currentCameraVolumes.Remove(collision.gameObject.GetComponent<CameraVolume>());
            cinemachineContraints.m_BoundingShape2D = currentCameraVolumes[currentCameraVolumes.Count - 1].volume;
            cinemachineCamera.m_Lens.OrthographicSize = currentCameraVolumes[currentCameraVolumes.Count - 1].GetCameraSize();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log($"Player hit {collision.collider.gameObject.name}");
        if(collision.collider.gameObject.CompareTag("Enemy"))
        {
            Vector3 contactLocal = collision.collider.transform.InverseTransformPoint(collision.GetContact(0).point);
            Debug.Log(contactLocal);
            if (dashing)
            {
                enemyDeathSound.Play();
                Destroy(collision.collider.gameObject);
                return;
            }
            else if (!damageImmune)
            {
                Vector2 direction = new Vector2(transform.position.x - collision.GetContact(0).point.x, transform.position.y - collision.GetContact(0).point.y).normalized;
                velocity.x = direction.x * 40;
                velocity.y = 10;
                //ModifyHealth(-1);
            }
        }
        if ((interactionMask.value & (1 << collision.gameObject.layer)) > 0)
        {
            Interaction(collision.transform.GetComponent<IInteractable>().Interact());
        }
    }

    private void ModifyHealth(int amount)
    {
        health += amount;
        if(health <= 0)
        {
            health = 0;
            deathSound.Play();
            Debug.Log("Dead");
        } else if (health > maxHealth)
        {
            health = maxHealth;
        }

        if(amount < 0)
        {
            hurtSound.Play();
            StartCoroutine(DamageImmunity());
        }

        for (int i = 0; i < maxHealth; i++)
        {
            healthUI.GetChild(i).GetComponent<Image>().sprite = i < health ? fullHeart : emptyHeart;
        }
    }

    private void SetMaxHealth(int maxHealth)
    {
        if(maxHealth < 1)
        {
            maxHealth = 1;
        }

        this.maxHealth = maxHealth;

        // Remove all heart objects
        for(int i = healthUI.childCount - 1; i >= 1; i--)
        {
            Destroy(healthUI.GetChild(i).gameObject);
        }

        for (int i = 1; i < maxHealth; i++)
        {
            GameObject newHeart = Instantiate(healthUI.GetChild(0).gameObject);
            newHeart.transform.SetParent(healthUI);
            newHeart.transform.localPosition = new Vector3(10 + 40 * i, -10, 0);
            newHeart.transform.localScale = Vector3.one;
        }

        ModifyHealth(maxHealth);
    }

    private void ModifyCoins(int amount)
    {
        coins += amount;
        coinText.text = $"{coins}";
    }

    public void ClearCoins()
    {
        coins = 0;
        coinText.text = $"{coins}";
    }

    public int GetCoinCount()
    {
        return coins;
    }


    public void Interaction(string with)
    {
        if(with == "coin" || with == "silver_coin")
        {
            ModifyCoins(1);
        } else if (with == "heart")
        {
            ModifyHealth(1);
        } else if (with =="kill")
        {
            ModifyHealth(-99);
        }
    }

    IEnumerator DamageImmunity()
    {
        damageImmune = true;
        yield return new WaitForSeconds(1f);
        damageImmune = false;
    }

    public void Attack()
    {
        if (canAttack)
        {
            animator.SetTrigger("attack");
            attackSound.Play();
            StartCoroutine(RepeatAttack());

            StartCoroutine(AttackDelay());
        }
    }

    IEnumerator RepeatAttack()
    {
        for (int i = 0; i < 2; i++)
        {
            Vector2 boxSize = new Vector2(2f, 1.5f);


            Vector2 direction = transform.right;
            Vector2 origin = transform.position;

            // Perform the boxcast
            RaycastHit2D hit = Physics2D.BoxCast(origin, boxSize, 0f, direction, 3, attackInteractionMask);

            // If the boxcast hits something on the specified layer
            if (hit.collider != null)
            {
                Debug.Log($"Hit {hit.collider.gameObject}");
                IHittable hittable;
                if (hit.collider.gameObject.TryGetComponent<IHittable>(out hittable))
                {
                    hittable.Hit();
                    if(hit.collider.tag == "Enemy")
                    {
                        enemyDeathSound.Play();
                    }
                }
            }

            yield return new WaitForSeconds(0.25f);
        }
    }

    IEnumerator AttackDelay()
    {
        canAttack = false;
        yield return new WaitForSeconds(1.5f);
        canAttack = true;
    }
}