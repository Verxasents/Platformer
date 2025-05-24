using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject fruitDrop;
    [SerializeField] private DifficultyType gameDifficulty;
    private GameManager gameManager;
    public static Player instance;

    private Rigidbody2D rb;
    private Animator anim;
    private CapsuleCollider2D cd;

    private bool canBeControlled = false;
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private float defaultGravityScale;
    private bool canDoubleJump;
    public bool canMove = true;

    [Header("Speed Boost")]
    [SerializeField] private float speedBoostDelay = 2f; // Kaç saniye sonra hýzlansýn
    [SerializeField] private float maxSpeedBoost = 1.5f; // En fazla kaç kat hýzlansýn  
    [SerializeField] private float speedBoostRate = 0.2f; // Ne kadar hýzlý artsýn

    private float runningTime = 0f;

    [Header("Buffer & Coyote jump")]
    [SerializeField] private float bufferJumpWindow = .25f;
    private float bufferJumpActivated = -1;
    [SerializeField] private float coyoteJumpWindow = .5f;
    private float coyoteJumpActivated = -1;

    [Header("Wall interactions")]
    [SerializeField] private float wallJumpDuration = .6f;
    [SerializeField] private Vector2 wallJumpForce;
    private bool isWallJumping;
    [SerializeField] private float wallSlideSpeed = 2f;

    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 1;
    [SerializeField] private Vector2 knockbackPower;
    private bool isKnocked;


    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] public float wallCheckDistance;
    [SerializeField] private LayerMask whatIsEnemy;
    [Space]
    [SerializeField] private Transform enemyCheck;
    [SerializeField] private float enemyCheckRadius;
    [SerializeField] private LayerMask whatIsGround;
    private bool isGrounded;
    private bool isAirborne;
    private bool isWallDetected;



    private Joystick joystick;
    private float xInput;
    private float yInput;
    

    private bool facingRight = true;
    private int facingDir = 1;

    [Header("VFX")]
    [SerializeField] private AnimatorOverrideController[] animators;
    [SerializeField] private GameObject deathVfx;
    [SerializeField] private ParticleSystem dustFx;
    [SerializeField] private int skinId;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CapsuleCollider2D>();
        anim = GetComponentInChildren<Animator>();

        FindFirstObjectByType<UI_JumpButton>().UpdatePlayersRef(this);
        joystick = FindFirstObjectByType<Joystick>();

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

    }

    private void Start()
    {
        defaultGravityScale = rb.gravityScale;
        gameManager = GameManager.instance;

        UpdateGameDifficulty();
        RespawnFinished(false);
        UpdateSkin();

    }

   

    private void Update()
    {
        UpdateAirborneStatus();

        if (canBeControlled == false)
        {
            HandleAnimations();
            HandleCollision();
            return;
        }

        if (isKnocked)
            return;

        HandleEnemyDetection();
        HandleInput();
        HandleWallSlide();

        if (canMove)
        {
         HandleMovement();
        }
        HandleFlip();
        HandleCollision();
        HandleAnimations();

    }
    
    public void Damage()
    {
        if (gameDifficulty == DifficultyType.Normal)
        {
            if (gameManager.FruitsCollected() <= 0)
            {
                Die();
                gameManager.RestartLevel();
            }
            else
            {
                ObjectCreator.instance.CreateObject(fruitDrop, transform,true);
                gameManager.RemoveFruit();
            }

            return;
        }

        if (gameDifficulty == DifficultyType.Hard)
        {
            Die();
            gameManager.RestartLevel();
        }
    }


    private void UpdateGameDifficulty()
    {
        DifficultyManager difficultyManager = DifficultyManager.instance;

        if (difficultyManager != null)
            gameDifficulty = difficultyManager.difficulty;
    }

    public void UpdateSkin()
    {
        SkinManager skinManager = SkinManager.instance;

        if (skinManager == null)
            return;

        anim.runtimeAnimatorController = animators[skinManager.choosenSkinId];

    }

    private void HandleEnemyDetection()
    {
        if (rb.velocity.y >= 0)
            return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(enemyCheck.position, enemyCheckRadius, whatIsEnemy);

        foreach (var enemy in colliders)
        {
            Enemy newEnemy = enemy.GetComponent<Enemy>();
            if (newEnemy != null)
            {
                AudioManager.instance.PlaySFX(1);
                newEnemy.Die();
                Jump();
            }
        }

    
    }

    public void RespawnFinished(bool finished)
    {

        if (finished)
        {
            rb.gravityScale = defaultGravityScale;
            canBeControlled = true;
            cd.enabled = true;

            AudioManager.instance.PlaySFX(9);
        }
        else
        {
            rb.gravityScale = 0;
            canBeControlled = false;
            cd.enabled = false;
        }

    }

    public void ResetPlayerAfterMenu()
    {
        // Karakter kontrolünü yeniden etkinleþtir
        canBeControlled = true;

        // Hareket kýsýtlamalarýný sýfýrla
        canMove = true;
        isKnocked = false;
        isWallJumping = false;

        // Hýz ve durumu sýfýrla
        rb.velocity = Vector2.zero;

        // Animator parametrelerini sýfýrla
        anim.SetBool("isKnocked", false);

        // Karakter collider'ýný etkinleþtir
        cd.enabled = true;

        // Yerçekimini normale döndür
        rb.gravityScale = defaultGravityScale;

        // Buffer ve coyote jump sýfýrlama
        bufferJumpActivated = -1;
        coyoteJumpActivated = -1;

        // Joystick referansýný yenile (gerekirse)
        if (joystick == null)
            joystick = FindFirstObjectByType<Joystick>();

        // Çift zýplama özelliðini sýfýrla
        if (isGrounded)
            canDoubleJump = true;
    }

    public void Knockback(float sourceDamageXPosition)
    {
        float knockbackDir = 1;

        if(transform.position.x < sourceDamageXPosition)
            knockbackDir = -1;

        if (isKnocked)
            return;

        AudioManager.instance.PlaySFX(11);
        CameraManager.instance.ScreenShake(knockbackDir);
        StartCoroutine(KnockbackRoutine());

        rb.velocity = new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y);
    }

    private IEnumerator KnockbackRoutine()
    {
        isKnocked = true;
        anim.SetBool("isKnocked", true);

        yield return new WaitForSeconds(knockbackDuration);

        isKnocked = false;
        anim.SetBool("isKnocked", false);
    }

    public void Die()
    {
        AudioManager.instance.PlaySFX(0);

        GameObject newDeathVfx = Instantiate(deathVfx,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }

    public void Push(Vector2 direction, float duration = 0)
    {
        StartCoroutine(PushCoroutine(direction,duration));
    }
    private IEnumerator PushCoroutine(Vector2 direction, float duration)
    {
        canBeControlled = false;

        rb.velocity = Vector2.zero;
        rb.AddForce(direction, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        canBeControlled = true;
  
    }

    private void UpdateAirborneStatus()
    {
        if (isGrounded && isAirborne)
            HandleLanding();

        if (!isGrounded && !isAirborne)
            BecomeAirborne();
    }

    private void BecomeAirborne()
    {
        isAirborne = true;

        if (rb.velocity.y < 0)
        {
            ActivateCoyoteJump();
        }
    }

    private void HandleLanding()
    {
        dustFx.Play();

        isAirborne = false;
        canDoubleJump = true;

        AttemptBufferJump();
    }
    private void HandleInput()
    {
        //xInput = Input.GetAxisRaw("Horizontal");
        //yInput = Input.GetAxisRaw("Vertical");


        xInput = joystick.Horizontal;
        yInput = joystick.Vertical;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            JumpButton();
        }
    }

    #region Buffer & Coyote Jump

    private void RequestBufferJump()
    {
            if (isAirborne)
            bufferJumpActivated = Time.time;
    }
    
    private void AttemptBufferJump()
    {
        if (Time.time < bufferJumpActivated + bufferJumpWindow)
        {
            bufferJumpActivated = Time.time - 1;
            Jump();
        }
    }

    private void ActivateCoyoteJump() => coyoteJumpActivated = Time.time;

    private void CancelCoyoteJump() => coyoteJumpActivated -= Time.time - 1;

    #endregion

    public void JumpButton()
    {
        JumpAttempt();
        RequestBufferJump();
    }

    private void JumpAttempt()
    {
        bool coyoteJumpAvailable = Time.time < coyoteJumpActivated + coyoteJumpWindow;
        if (isGrounded || coyoteJumpAvailable)
        {
          Jump();
        }
        else if (isWallDetected)
        {
            WallJump();
        }
        else if (canDoubleJump)
        {
            DoubleJump();
        }

        CancelCoyoteJump();
    }
    private void Jump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(3);
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    }
    private void DoubleJump()
    {
        dustFx.Play();
        isWallJumping = false;
        canDoubleJump = false;
        rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
    }

    private void WallJump()
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(12);
        canDoubleJump = false;
        rb.velocity = new Vector2(wallJumpForce.x * -facingDir, wallJumpForce.y);

        Flip();

        StopAllCoroutines();
        StartCoroutine(WallJumpRoutine());
    }

    private IEnumerator WallJumpRoutine()
    {
        isWallJumping = true;

        yield return new WaitForSeconds(wallJumpDuration);

        isWallJumping = false;
    }

    private void HandleWallSlide()
    {
        // Duvarda olup olmadýðýný ve düþüyor olup olmadýðýný kontrol et
        bool canWallSlide = isWallDetected && !isGrounded && rb.velocity.y < 0;

        if (canWallSlide)
        {
            // Duvara yapýþýk kal - yatay hýzý sýfýrla
            rb.velocity = new Vector2(0, rb.velocity.y);

            // Düþüþ hýzýný sýnýrla
            if (rb.velocity.y < -wallSlideSpeed)
                rb.velocity = new Vector2(0, -wallSlideSpeed);
        }
    }



    private void HandleCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        isWallDetected = Physics2D.Raycast(transform.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);
    }

    private void HandleAnimations()
    {
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isWallDetected", isWallDetected);
    }

    private void HandleMovement()
    {
        if (isWallDetected)
            return;

        if (isWallJumping)
            return;

        bool isMoving = Mathf.Abs(xInput) > 0.1f;
        float currentSpeed = moveSpeed;

        if (isMoving) // && isGrounded kaldýrýldý
        {
            runningTime += Time.deltaTime;

            if (runningTime > speedBoostDelay)
            {
                float speedBoost = Mathf.Min(maxSpeedBoost, 1f + (runningTime - speedBoostDelay) * speedBoostRate);
                currentSpeed = moveSpeed * speedBoost;
            }
        }
        else
        {
            runningTime = 0f; // Sadece hareket etmediðinde sýfýrla
        }

        rb.velocity = new Vector2(xInput * currentSpeed, rb.velocity.y);
    }

    private void HandleFlip()
    {
        if(xInput < 0 && facingRight || xInput > 0 && !facingRight)
        {
         Flip();

        }
    }
    private void Flip()
    {
        facingDir = facingDir * -1;
        transform.Rotate(0, 180, 0);
        facingRight = !facingRight;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(enemyCheck.position, enemyCheckRadius);
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallCheckDistance * facingDir), transform.position.y));
    }
}



