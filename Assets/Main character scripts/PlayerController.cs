using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalMove;
    public float speedCoefficient;
    public float jumpCoefficient;
    public float wallSlideSpeed;
    public float DistanceToCheck, // for ground checking
        knockbackDuration, wallCheckDistance, correctDistance = 0.01f;

    public LayerMask Ground;
    public Vector2 knockbackSpeed, wallJumpClimb, /*wallJumpOff,*/ wallLeap;
    public Transform Player, PositionOfFeet, WallCheck, LedgeCheck, FinishWallClimbPosition;


    private Rigidbody2D rb;
    private Animator anim;
    private PlayerCombatController playerCombatController;
    private Vector3 velocity;


    private int facingDirection = 1;
    private float knockbackStartTime, offsetY;
    private bool
        isRight, //for checking if character turned to the right
        canMove = true,
        canFlip = true,
        IsOnTheGround,
        IsRunning,
        knockback,
        isTouchingWall,
        isWallSliding,
        isTouchingLedge,
        canClimbWall = false,
        ledgeDetected = false;

    [SerializeField] private AudioSource JumpSoundEffect;
    [SerializeField] private AudioSource RunSoundEffect;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerCombatController = GetComponent<PlayerCombatController>();
    }

    void Update()
    {
        int wallDirectionX = (int)transform.localScale.x;

        IsOnTheGround = Physics2D.OverlapCircle(PositionOfFeet.position, DistanceToCheck, Ground);
        isTouchingWall = Physics2D.Raycast(WallCheck.position, new Vector2(transform.localScale.x, 0), wallCheckDistance, Ground);
        isTouchingLedge = Physics2D.Raycast(LedgeCheck.position, new Vector2(transform.localScale.x, 0), wallCheckDistance, Ground);

        bool attacking = playerCombatController.isAttack();

        if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
        {
            ledgeDetected = true;
            canMove = false;
            canFlip = false;
        }

        if (IsOnTheGround && Input.GetKeyDown(KeyCode.Space) && !knockback && !attacking)
        {
            JumpSoundEffect.Play();
            rb.velocity = Vector2.up * jumpCoefficient;
            
        }

        if (isWallSliding && Input.GetKeyDown(KeyCode.Space))
        {
            if (horizontalMove == 0)
            {
                velocity.x = -wallDirectionX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            }
            else
            {
                velocity.x = -wallDirectionX * wallLeap.x;
                velocity.y = wallLeap.y;
            }
            rb.velocity = velocity;
        }

        UpdateAnimations();
        CheckWallSliding();
        CheckKnockback();
        CheckWallClimb();
    }

    void FixedUpdate() 
    {
        horizontalMove = Input.GetAxis("Horizontal");
        if (!knockback && canFlip && canMove)
        {
            rb.velocity = new Vector2(horizontalMove * speedCoefficient, rb.velocity.y);
            if (isRight && horizontalMove > 0)
            {
                FaceFlip();
            }
            else if (!isRight & horizontalMove < 0)
            {
                FaceFlip();
            }

            if (rb.velocity.x != 0.0f && IsOnTheGround)
            {
                IsRunning = true;
            }
            else
            {
                IsRunning = false;
            }

            if (IsRunning && IsOnTheGround)
            {

                if (!IsOnTheGround)
                {
                    RunSoundEffect.Stop();

                }
                else if (!RunSoundEffect.isPlaying)
                {
                    RunSoundEffect.Play();
                }
            }
            else
            {
                RunSoundEffect.Stop();
            }

        }
        if (isWallSliding)
        {
            if (rb.velocity.y < -wallSlideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -wallSlideSpeed);
            }
        }
    }
    private void CheckWallSliding()
    {
        if (isTouchingWall && !IsOnTheGround && rb.velocity.y < 0 && !ledgeDetected)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void CheckWallClimb()
    {
        if (ledgeDetected && !canClimbWall)
        {
            canClimbWall = true;

            offsetY = Physics2D.Raycast(new Vector2(LedgeCheck.position.x + wallCheckDistance * transform.localScale.x, LedgeCheck.position.y), Vector2.down, (LedgeCheck.position.y - WallCheck.position.y), Ground).distance;

            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, 0);

            transform.position = new Vector3(transform.position.x, transform.position.y - offsetY + correctDistance, transform.position.z);
            //myanimation.Play("player_wall_climb");
        }
    }

    public void FinishWallClimb()
    {
        canClimbWall = false;
        //myanimation.Play("player_idle");
        transform.position = FinishWallClimbPosition.position;

        ledgeDetected = false;
        rb.gravityScale = 1;
        canMove = true;
        canFlip = true;
    }
    private void UpdateAnimations()
    {
        anim.SetBool("isTookDamage", knockback);
        anim.SetBool("isRunning", IsRunning);
        anim.SetBool("isGrounded", IsOnTheGround);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("canClimbWall", canClimbWall);
    }

    public bool canAttack()
    {
        return (IsOnTheGround && !IsRunning);
    }

    public void Knockback(int damageDirection, float enemyPosition)
    {
        knockback = true;
        knockbackStartTime = Time.time;
        rb.velocity = new Vector2(knockbackSpeed.x * damageDirection, knockbackSpeed.y);

        if (enemyPosition < rb.transform.position.x && rb.transform.localScale.x >= 0)
            FaceFlip();
        else if (enemyPosition > rb.transform.position.x && rb.transform.localScale.x < 0)
            FaceFlip();
    }

    private void CheckKnockback()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration && knockback)
        {
            knockback = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }

    void FaceFlip()
    {
        if (canFlip && !isWallSliding)
        {
            facingDirection *= -1;
            isRight = !isRight;
            Vector3 temp = transform.localScale;
            temp.x *= -1;
            transform.localScale = temp;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(PositionOfFeet.position, DistanceToCheck);
        Gizmos.DrawLine(WallCheck.position, new Vector2(WallCheck.position.x + wallCheckDistance * transform.localScale.x, WallCheck.position.y));
        Gizmos.DrawLine(LedgeCheck.position, new Vector2(LedgeCheck.position.x + wallCheckDistance * transform.localScale.x, LedgeCheck.position.y));
        Gizmos.color = Color.green;
        //Debug.DrawRay(new Vector2(LedgeCheck.position.x + wallCheckDistance * transform.localScale.x, LedgeCheck.position.y), Vector2.down * (LedgeCheck.position.y - WallCheck.position.y));
        Gizmos.DrawLine(new Vector2(LedgeCheck.position.x + wallCheckDistance * transform.localScale.x, LedgeCheck.position.y), new Vector2(WallCheck.position.x + wallCheckDistance * transform.localScale.x, WallCheck.position.y));
    }
}
