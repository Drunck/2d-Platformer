using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FlyingEnemyControllScript : MonoBehaviour
{
    private enum State
    {
        Patrolling,
        Idle,
        PlayerDetected,
        Chase,
        ReturnToPatrol,
        Knockback,
        Attack,
        Dead
    }

    public float
        speedCoefficient,
        maxHealth,
        knockbackDuration,
        minIdleDuration,
        maxIdleDuration,
        minAggroRange,
        maxAggroRange,
        maxDistanceToChase = 10f,
        chaseSpeed = 6f,
        nextWaypointDistance = 3f,
        attackRadius;

    public int minDamage, maxDamage;
    public LayerMask playerLayer, ignorLayer;

    public List<Transform> points;
    public Transform Player, attackPosition;
    public Vector2 knockbackSpeed;
    public GameObject hitParticle, deathChunkParticle;

    private State currentState;
    private Rigidbody2D rb;
    private Animator enemyAnim;
    private Transform targetPoint;
    private Vector2 movement;
    private Path path;
    private Seeker seeker;
    private RaycastHit2D attackPlayerRaycast;
    private DropLoot dropLoot;

    [SerializeField] private AudioSource BatAttackSound;
    [SerializeField] private AudioSource BatFlyingSound;


    private float 
        currentHeath, 
        knockbackStartTime, 
        startIdle, 
        idleTime, 
        startAttackCooldownTime,
        distanceToPlayer;
    private int
        facingDirection,
        damageDirection,
        nextID = 0,
        idChangeValue = 1,
        currentWaypoint,
        damage;
    private bool reachedEndOfPath;
    private float[] attackDetails = new float[2];

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        dropLoot = GetComponent<DropLoot>();

        seeker = GetComponent<Seeker>();

        currentHeath = maxHealth;
        facingDirection = 1;
    }

    void Update()
    {
        attackPlayerRaycast = Physics2D.Raycast(attackPosition.position, transform.right, attackRadius, ~ignorLayer);

        switch (currentState)
        {
            case State.Patrolling:
                UpdatePatrollingState();
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
            case State.ReturnToPatrol:
                UpdateReturnToPatrol();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Attack:
                UpdateAttackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    #region Patrolling State
    private void EnterPatrollingState()
    {
    }
    private void UpdatePatrollingState()
    {
        targetPoint = points[nextID];

        distanceToPlayer = Vector2.Distance(Player.position, transform.position);

        if (targetPoint.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speedCoefficient * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            if (nextID == points.Count - 1)
                idChangeValue = -1;
            if (nextID == 0)
                idChangeValue = 1;

            nextID += idChangeValue;
        }

        // if player in a agro range then chase him
        if (distanceToPlayer <= minAggroRange)
        {
            SwitchState(State.PlayerDetected);
        }
    }
    private void ExitPatrollingState()
    { 

    }
    #endregion

    #region Idle State
    private void EnterIdleState()
    {
        startIdle = Time.time;
        idleTime = Random.Range(minIdleDuration, maxIdleDuration);
    }
    private void UpdateIdleState()
    {
        transform.position = targetPoint.position;

        if (Time.time >= startIdle + idleTime)
        { 
            SwitchState(State.Patrolling);
        }

        float distanceToPlayer = Vector2.Distance(Player.position, transform.position);

        if (distanceToPlayer <= minAggroRange)
        {
            SwitchState(State.Chase);
        }
    }
    private void ExitIdleState()
    {
    }
    #endregion

    #region Player Detected State
    public void EnterPlayerDetectedState()
    {

    }
    public void UpdatePlayerDetectedState()
    {

        if (distanceToPlayer <= minAggroRange)
        {
            BatFlyingSound.Play();
            SwitchState(State.Chase);
        }
        
        else if (distanceToPlayer >= maxAggroRange)
        {
            BatFlyingSound.Stop();

            SwitchState(State.ReturnToPatrol);
        }
       

        if (attackPlayerRaycast.collider != null)
        {
            if (attackPlayerRaycast.collider.name == "Player")
                SwitchState(State.Attack);
        }
    }
    public void ExitPlayerDetectedState()
    {

    }
    #endregion

    #region Chase State
    private void EnterChaseState()
    {
        seeker.StartPath(rb.position, Player.position, OnPathComplete);
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }
    private void UpdateChaseState()
    {
        targetPoint = points[nextID];

        if (attackPlayerRaycast.collider != null)
        {
            if (attackPlayerRaycast.collider.name == "Player")
                SwitchState(State.Attack);
        }

        if (Player.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        float distanceBetwennEnemyAndPatrolPoint = Vector2.Distance(transform.position, targetPoint.position);
        float distanceBetwennEnemyAndPlayer = Vector2.Distance(transform.position, Player.position);

        // if distance between enemy and last patrolling point is less than 5 or distance between player and enemy is less than stopAgroDistance
        // then enemy continious chasing player
        if (distanceBetwennEnemyAndPatrolPoint < maxDistanceToChase && distanceBetwennEnemyAndPlayer < maxAggroRange) 
        {
            if (path == null)
                return;

            if (currentWaypoint >= path.vectorPath.Count)
            {
                reachedEndOfPath = true;
                return;
            }
            else
            {
                reachedEndOfPath = false;
            }

            Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector2 force = direction * chaseSpeed * Time.deltaTime;

            rb.velocity = force;

            float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

            if (distance < nextWaypointDistance)
                currentWaypoint++;
        }
        //transform.position = Vector2.MoveTowards(this.transform.position, Player.position, chaseSpeed * Time.deltaTime);
        else
            SwitchState(State.ReturnToPatrol);
    }
    private void ExitChaseState()
    {
        path = null;
    }
    #endregion

    #region ReturnToPatrol
    private void EnterReturnToPatrol()
    {

    }
    private void UpdateReturnToPatrol()
    {
        targetPoint = points[nextID];

        if (targetPoint.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speedCoefficient * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            SwitchState(State.Patrolling);
        }
    }
    private void ExitReturnToPatrol()
    {

    }
    #endregion

    #region KnockBack
    private void EnterKnockbackState()
    {
        knockbackStartTime = Time.time;
        enemyAnim.SetBool("Knockback", true);
        rb.velocity = new Vector2(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
    }
    private void UpdateKnockbackState()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.PlayerDetected);
        }
    }
    private void ExitKnockbackState()
    {
        enemyAnim.SetBool("Knockback", false);
    }
    #endregion

    #region Attack Player State
    private void EnterAttackState()
    {
        BatAttackSound.Play();
        enemyAnim.SetBool("isAttacking", true);
    }
    private void UpdateAttackState()
    {
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

    #region Dead State
    private void EnterDeadState()
    {
       
        Instantiate(deathChunkParticle, rb.transform.position, deathChunkParticle.transform.rotation);
        dropLoot.DropItem();
        Destroy(gameObject);
        BatAttackSound.Stop();
        BatFlyingSound.Stop();


    }
    private void UpdateDeadState()
    { }
    private void ExitDeadState()
    { }
    #endregion

    private void SwitchState(State st)
    {
        switch (currentState)
        {
            case State.Patrolling:
                ExitPatrollingState();
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
            case State.ReturnToPatrol:
                ExitReturnToPatrol();
                break;
            case State.Knockback:
                ExitKnockbackState();
                break;
            case State.Attack:
                ExitAttackState();
                break;
            case State.Dead:
                ExitDeadState();
                break;
        }

        switch (st)
        {
            case State.Patrolling:
                EnterPatrollingState();
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
            case State.ReturnToPatrol:
                EnterReturnToPatrol();
                break;
            case State.Knockback:
                EnterKnockbackState();
                break;
            case State.Attack:
                EnterAttackState();
                break;
            case State.Dead:
                EnterDeadState();
                break;
        }

        currentState = st;
    }

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
        if (attackPlayerRaycast.collider != null)
        {
            
            if (attackPlayerRaycast.collider.name == "Player")
                SwitchState(State.Attack);
        }
    }

    private void UpdatePath()
    {
        if (seeker.IsDone())
            seeker.StartPath(rb.position, Player.position, OnPathComplete);
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
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

    public void SetVelocity(float velocity)
    {
        movement.Set(facingDirection * velocity, rb.velocity.y);
        rb.velocity = movement;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, minAggroRange);
        Gizmos.DrawWireSphere(attackPosition.position, attackRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxAggroRange);
    }
}
