using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mushroomEnemyController : MonoBehaviour
{
    private enum State
    {
        Moving,
        Idle,
        Knockback,
        Dead
    }

    public float
        speedCoefficient,
        groundDistanceToCheck,
        wallDistanceToCheck,
        maxHealth,
        knockbackDuration,
        minIdleDuration,
        maxIdleDuration;
        

    public Transform groundCheck, wallCheck, Player;
    public LayerMask ground;
    public Vector2 knockbackSpeed;
    public GameObject hitParticle, deathChunkParticle, deathBloodParticle;

    private State currentState;
    private Rigidbody2D rb;
    private Animator enemyAnim;
    private Vector2 movement;

    private bool isGround, isWall, isRight;
    private float currentHeath, knockbackStartTime, startIdle, idleTime;
    private int facingDirection, damageDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();

        currentHeath = maxHealth;
        facingDirection = 1;
    }

    // Update is called once per frame
    void Update()
    {
        switch(currentState)
        {
            case State.Moving:
                UpdateMovingState();
                break;
            case State.Idle:
                UpdateIdleState();
                break;
            case State.Knockback:
                UpdateKnockbackState();
                break;
            case State.Dead:
                UpdateDeadState();
                break;
        }
    }

    private void FixedUpdate()
    {
        
    }

    #region Moving State
    private void EnterMovingState()
    {

    }

    private void UpdateMovingState()
    {
        isGround = Physics2D.Raycast(groundCheck.position, Vector2.down, groundDistanceToCheck, ground);
        isWall = Physics2D.Raycast(wallCheck.position, transform.right, wallDistanceToCheck, ground);

        if (!isGround || isWall)
        {
            //Switch to Idle state
            SwitchState(State.Idle);
        }
        else
        {
            movement.Set(speedCoefficient * facingDirection, rb.velocity.y);
            rb.velocity = movement;
        }
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
    }
    private void ExitIdleState()
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

        if (Player.position.x < rb.transform.position.x && rb.transform.rotation.y == 0)
        {
            FaceFlip();
        }
        else if(Player.position.x > rb.transform.position.x && rb.transform.rotation.y < 0)
        {
            FaceFlip();
        }

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
    // Dead /------------------------------------
    private void EnterDeadState()
    {
        Instantiate(deathChunkParticle, rb.transform.position, deathChunkParticle.transform.rotation);
        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {
        
    }
    #endregion
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
            case State.Knockback:
                ExitKnockbackState();
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
    }
}
