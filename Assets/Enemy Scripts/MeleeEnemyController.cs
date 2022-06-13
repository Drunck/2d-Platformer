using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyController : MonoBehaviour
{
    private enum State
    {
        Moving,
        Idle,
        PlayerDetected,
        Chase,
        Attack,
        LookingForPlayer,
        Knockback,
        Dead
    }

    public float
        speedCoefficient,
        groundDistanceToCheck,
        wallDistanceToCheck,
        maxHealth,
        knockbackDuration,
        playerDetectionAreaLength,
        attackCooldownDuration,
        attackPlayerAreaLength,
        attackRadius,
        chaseSpeed = 4f,
        playerDetectedTimeDuration = 0.5f,
        chaseTimeDuration = 0.2f,
        numberOfTurns,
        lookingForPlayerTimeDuration = 0.5f;

    public int
        minIdleDuration,
        maxIdleDuration, 
        minDamage, 
        maxDamage;
        

    public Transform groundCheck, wallCheck, Player, attackPosition, raycastOriginPosition;
    public LayerMask ground, playerLayer, ignorLayer;
    public Vector2 knockbackSpeed;
    public GameObject hitParticle, deathChunkParticle;
    public EnemiesScriptableObjects enemyScripObj;
    
    private DropLoot dropLoot;
    private State currentState;
    private Rigidbody2D rb;
    private Animator enemyAnim;
    private Vector2 movement;
    private RaycastHit2D playerDetectionRaycast, attackPlayerRaycast;

    [SerializeField] private AudioSource MushroomAttackSound;
    [SerializeField] private AudioSource MushroomWalkingSound;


    private bool 
        isGround, 
        isWall, 
        isPlayerDetectedStateTimeOver;

    private float
        currentHeath,
        knockbackStartTime,
        startIdle,
        idleTime,
        startPlayerDetectedTime,
        startChaseTime,
        startAttackCooldownTime,
        startLookingForPlayerTime,
        countTurns;

    private float[] attackDetails = new float[2];

    private int facingDirection, damageDirection, damage;
    private bool shouldTurn;

    void Start()
    {
        minDamage = enemyScripObj.minDamage;
        maxDamage = enemyScripObj.maxDamage;
        maxHealth = enemyScripObj.health;

        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        dropLoot = GetComponent<DropLoot>();
        currentHeath = maxHealth;
        facingDirection = 1;
    }

    void Update()
    {
        playerDetectionRaycast = Physics2D.Raycast(raycastOriginPosition.position, transform.right, playerDetectionAreaLength, ~ignorLayer);
        attackPlayerRaycast = Physics2D.Raycast(raycastOriginPosition.position, transform.right, attackPlayerAreaLength, ~ignorLayer);

        //playerDetectionRaycast = Physics2D.Raycast(transform.position - transform.up * 0.3f, transform.right, playerDetectionAreaLength, ~ignorLayer);
        //attackPlayerRaycast = Physics2D.Raycast(transform.position - transform.up * 0.3f, transform.right, attackPlayerAreaLength, ~ignorLayer);

        isGround = Physics2D.Raycast(groundCheck.position, Vector2.down, groundDistanceToCheck, ground);
        isWall = Physics2D.Raycast(wallCheck.position, transform.right, wallDistanceToCheck, ground);

        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.Idle:
                UpdateIdleState();
                break;
            case State.PlayerDetected:
                UpdatePlayerDetectedState();
                break;
            case State.Chase:
                UpdateChaseState();
                break;
            case State.Attack:
                UpdateAttackState();
                break;
            case State.LookingForPlayer:
                UpdateLookingForPlayerState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    #region Moving State
    private void EnterMovingState()
    {

    }

    private void UpdateMovingState()
    {
        //Debug.Log("UpdateMovingState");
        
        if (!isGround || isWall)
            SwitchState(State.Idle);
        else
            SetVelocity(speedCoefficient);

        if (playerDetectionRaycast.collider != null)
            if (playerDetectionRaycast.collider.name == "Player" && isGround)
                SwitchState(State.PlayerDetected);
    }

    private void ExitMovingState()
    {

    }
    #endregion

    #region Idle State
    private void EnterIdleState()
    {
        startIdle = Time.time;
        idleTime = Random.Range(minIdleDuration, maxIdleDuration);

        SetVelocity(0f);

        enemyAnim.SetBool("isIdle", true);
    }
    private void UpdateIdleState()
    {
        if(Time.time >= startIdle + idleTime)
        {
            FaceFlip();
            SwitchState(State.Moving);
        }

        if (playerDetectionRaycast.collider != null)
            if (playerDetectionRaycast.collider.name == "Player")
                SwitchState(State.PlayerDetected);
    }
    private void ExitIdleState()
    {
        enemyAnim.SetBool("isIdle", false);
    }
    #endregion

    #region Player Detected State
    private void EnterPlayerDetectedState()
    {
        MushroomWalkingSound.Play();
        isPlayerDetectedStateTimeOver = false;
        startPlayerDetectedTime = Time.time;
        SetVelocity(0f);
        enemyAnim.SetBool("isPlayerDetected", true);
    }
    private void UpdatePlayerDetectedState()
    {
        if (Time.time >= startPlayerDetectedTime + playerDetectedTimeDuration)
        {
            SwitchState(State.Chase);
            isPlayerDetectedStateTimeOver = true;
        }

        if (playerDetectionRaycast.collider != null && isPlayerDetectedStateTimeOver)
        {
            if (playerDetectionRaycast.collider.name != "Player")
                SwitchState(State.LookingForPlayer);
        }
        else if (playerDetectionRaycast.collider == null && isPlayerDetectedStateTimeOver)
            SwitchState(State.LookingForPlayer);

        if (attackPlayerRaycast.collider != null)
        {
            if (attackPlayerRaycast.collider.name == "Player" && Time.time >= attackCooldownDuration + startAttackCooldownTime)
                SwitchState(State.Attack);
            else
                SwitchState(State.PlayerDetected);
        }
        
    }
    private void ExitPlayerDetectedState()
    {
        MushroomWalkingSound.Stop();

        enemyAnim.SetBool("isPlayerDetected", false);
    }
    #endregion

    #region Chase State
    private void EnterChaseState()
    {
        startChaseTime = Time.time;
        enemyAnim.SetBool("isChasing", true);
    }
    private void UpdateChaseState()
    {
        SetVelocity(chaseSpeed);

        if (Time.time >= startChaseTime + chaseTimeDuration)
            SwitchState(State.PlayerDetected);

        if (attackPlayerRaycast.collider != null)
            if (attackPlayerRaycast.collider.name == "Player")
                SwitchState(State.Attack);
    }
    private void ExitChaseState()
    {
        enemyAnim.SetBool("isChasing", false);
    }
    #endregion

    #region Attack Player State
    private void EnterAttackState()
    {
        MushroomAttackSound.Play();
        SetVelocity(0f);
        enemyAnim.SetBool("isAttacking", true);
    }
    private void UpdateAttackState()
    {
        //Debug.Log("UpdateAttackState");
        if (attackPlayerRaycast.collider != null)
        {
            if (attackPlayerRaycast.collider.name != "Player")
                SwitchState(State.PlayerDetected);
        }
        else if (attackPlayerRaycast.collider == null)
            SwitchState(State.PlayerDetected);
    }
    private void ExitAttackState()
    {
        enemyAnim.SetBool("isAttacking", false);
    }
    #endregion

    #region Looking For Player State
    private void EnterLookingForPlayerState()
    {
        enemyAnim.SetBool("isIdle", true);
        shouldTurn = true;
    }

    private void UpdateLookingForPlayerState()
    {
        if(shouldTurn)
        {
            FaceFlip();
            startLookingForPlayerTime = Time.time; // last time when enemy is turned
            shouldTurn = false;
        }
        else if (Time.time >= startLookingForPlayerTime + lookingForPlayerTimeDuration && countTurns < numberOfTurns)
        {
            FaceFlip();
            startLookingForPlayerTime = Time.time; // last time when enemy is turned
            countTurns++;
        }

        if(Time.time >= startLookingForPlayerTime + lookingForPlayerTimeDuration && countTurns >= numberOfTurns)
        {
            SwitchState(State.Moving);
        }
    }

    private void ExitLookingForPlayerState()
    {
        enemyAnim.SetBool("isIdle", false);
    }
    #endregion

    #region Knockback State
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;

        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);

        rb.velocity = movement;

        if (Player.position.x < rb.transform.position.x && rb.transform.rotation.y >= 0)
            FaceFlip();
        else if(Player.position.x > rb.transform.position.x && rb.transform.rotation.y < 0)
            FaceFlip();

        enemyAnim.SetBool("Knockback", true);
    }

    private void UpdateKnockbackState()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Moving);
        }
    }

    private void ExitKnockbackState()
    {
        enemyAnim.SetBool("Knockback", false);
    }
    #endregion

    #region Dead State
    private void EnterDeadState()
    {
        MushroomAttackSound.Stop();
        MushroomWalkingSound.Stop();

        Instantiate(deathChunkParticle, rb.transform.position, deathChunkParticle.transform.rotation);
        dropLoot.DropItem();
        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {
        
    }
    #endregion

    public void Attack_Player()
    {
        startAttackCooldownTime = Time.time;

        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attackPosition.position, attackRadius, playerLayer);

        damage = Random.Range(minDamage, maxDamage);

        attackDetails[0] = damage;
        attackDetails[1] = transform.position.x;
        //Debug.Log("Enemy: " + damage);

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.SendMessage("PlayerTakeDamage", attackDetails);
        }
    }

    public void FinishAttackAnimation()
    {
        enemyAnim.SetBool("isAttacking", false);

        if (attackPlayerRaycast.collider != null) {
            if (attackPlayerRaycast.collider.name == "Player" && Time.time >= attackCooldownDuration + startAttackCooldownTime)
                SwitchState(State.Attack);
            else
                SwitchState(State.PlayerDetected);
        }
    }

    private void Damage(float[] attackDetails)
    {
        currentHeath -= attackDetails[0];

        Instantiate(hitParticle, rb.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        
        if (attackDetails[1] > rb.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        if (currentHeath > 0.0)
        {
            SwitchState(State.Knockback);
        }
        else if (currentHeath <= 0.0f)
        {
            SwitchState(State.Dead);
        }
    }

    void FaceFlip()
    {
        facingDirection *= -1;
        rb.transform.Rotate(0.0f, 180.0f, 0.0f);
    }

    private void SwitchState(State st)
    {
        switch (currentState)
        {
            case State.Moving:
                ExitMovingState();
                break;
            case State.Idle:
                ExitIdleState();
                break;
            case State.PlayerDetected:
                ExitPlayerDetectedState();
                break;
            case State.Chase:
                ExitChaseState();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Attack:
                ExitAttackState();
                break;
            case State.LookingForPlayer:
                ExitLookingForPlayerState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }
        
        switch (st)
        {
            case State.Moving:
                EnterMovingState();
                break;
            case State.Idle:
                EnterIdleState();
                break;
            case State.PlayerDetected:
                EnterPlayerDetectedState();
                break;
            case State.Chase:
                EnterChaseState();
                break;
            case State.Attack:
                EnterAttackState();
                break;
            case State.LookingForPlayer:
                EnterLookingForPlayerState();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = st;
    }

    public void SetVelocity(float velocity)
    {
        movement.Set(facingDirection * velocity, rb.velocity.y);
        rb.velocity = movement;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundDistanceToCheck));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallDistanceToCheck, wallCheck.position.y)); 
        Gizmos.DrawWireSphere(attackPosition.position, attackRadius);
        Debug.DrawRay(raycastOriginPosition.position, Vector2.right * playerDetectionAreaLength);
        Debug.DrawRay(raycastOriginPosition.position, Vector2.right * attackPlayerAreaLength, Color.red);
        //Debug.DrawRay(transform.position - transform.up * 0.3f, Vector2.right * playerDetectionAreaLength);
        //Debug.DrawRay(transform.position - transform.up * 0.3f, Vector2.right * attackPlayerAreaLength, Color.red);
    }
}
