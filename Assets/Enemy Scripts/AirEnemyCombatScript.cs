using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirEnemyCombatScript : MonoBehaviour
{
    private enum State
    {
        Patrolling,
        Idle,
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
        agroDistance,
        stopAgroDistance;

    public List<Transform> points;
    public Transform player;
    public Vector2 knockbackSpeed;

    private State currentState;
    private Rigidbody2D rb;
    private Animator enemyAnim;
    private Vector2 movement;

    private float currentHeath, knockbackStartTime, startIdle, idleTime, startReturningToPatrolPointTime;
    private int
        facingDirection,
        damageDirection,
        nextID = 0,
        idChangeValue = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();

        currentHeath = maxHealth;
        facingDirection = 1;
    }

    void Update()
    {
        switch (currentState)
        {
            case State.Patrolling:
                UpdatePatrollingState();
                break;
            case State.Idle:
                UpdateIdleState();
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
        Transform targetPoint = points[nextID];

        float distanceToPlayer = Vector2.Distance(player.position, transform.position);

        if (targetPoint.transform.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speedCoefficient * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.2f)
        {
            if (nextID == points.Count - 1)
                idChangeValue = -1;
            if (nextID == 0)
                idChangeValue = 1;

            nextID += idChangeValue;
        }

        if (distanceToPlayer <= agroDistance)
        {
            SwitchState(State.Chase);
        }
    }
    private void ExitPatrollingState()
    {

    }
    #endregion

    #region Idle State
    private void EnterIdleState()
    {
    }
    private void UpdateIdleState()
    {
    }
    private void ExitIdleState()
    {
    }
    #endregion

    #region Chase State
    private void EnterChaseState()
    {

    }
    private void UpdateChaseState()
    {
        Transform targetPoint = points[nextID];

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        if (Vector2.Distance(transform.position, targetPoint.position) < agroDistance)
            transform.position = Vector2.MoveTowards(this.transform.position, player.position, speedCoefficient * Time.deltaTime);
        else
            SwitchState(State.Patrolling);
    }
    private void ExitChaseState()
    {

    }
    #endregion

    #region ReturnToPatrol
    private void EnterReturnToPatrol()
    {

    }
    private void UpdateReturnToPatrol()
    {

    }
    private void ExitReturnToPatrol()
    {

    }
    #endregion

    #region KnockBack
    private void EnterKnockbackState()
    { }
    private void UpdateKnockbackState()
    { }
    private void ExitKnockbackState()
    { }
    #endregion

    #region Attack State
    private void EnterAttackState()
    {

    }
    private void UpdateAttackState()
    {

    }
    private void ExitAttackState()
    {

    }
    #endregion

    #region Dead State
    private void EnterDeadState()
    { }
    private void UpdateDeadState()
    { }
    private void ExitDeadState()
    { }
    #endregion

    private void Damage(float[] attackDetails)
    {
        currentHeath -= attackDetails[0];

        //Instantiate(hitParticle, rb.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));

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
            case State.Patrolling:
                ExitPatrollingState();
                break;
            case State.Idle:
                ExitIdleState();
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
            case State.Chase:
                EnterChaseState();
                break;
            case State.ReturnToPatrol:
                EnterReturnToPatrol();
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
        Gizmos.DrawWireSphere(transform.position + transform.right, agroDistance);
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, stopAgroDistance);
    }
}
