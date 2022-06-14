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


    private Rigidbody2D rb;
    private Animator anim;

    bool isRight; //for checking if character turned to the right


    public Transform PositionOfFeet, WallCheck;

    public float DistanceToCheck, knockbackDuration, wallCheckDistance; 
    public LayerMask Ground;
    public Vector2 knockbackSpeed;
    public Transform Player;

    private PlayerCombatController playerCombatController;
    private float knockbackStartTime;
    private bool IsOnTheGround, IsRunning, knockback, isTouchingWall, isWallSliding;

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
        IsOnTheGround = Physics2D.OverlapCircle(PositionOfFeet.position, DistanceToCheck, Ground);
        isTouchingWall = Physics2D.OverlapCircle(WallCheck.position, wallCheckDistance, Ground);

        bool attacking = playerCombatController.isAttack();

        if (IsOnTheGround && Input.GetKeyDown(KeyCode.Space) && !knockback && !attacking)
        {
            JumpSoundEffect.Play();
            rb.velocity = Vector2.up * jumpCoefficient;
            
        }

      /*  if (!Input.GetKeyDown(KeyCode.Space) && !knockback && !attacking && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)) )
        {
            RunSoundEffect.Play();
        }
        else if (!Input.GetKeyDown(KeyCode.A) || !Input.GetKeyDown(KeyCode.D) || !Input.GetKeyDown(KeyCode.LeftArrow) || !Input.GetKeyDown(KeyCode.RightArrow))
        {
            RunSoundEffect.Pause();
        }*/


        UpdateAnimations();
        CheckWallSliding();
        CheckKnockback();
    }

    private void CheckWallSliding()
    {
        if(isTouchingWall && !IsOnTheGround && rb.velocity.y < 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false; 
        }
    }

    void FixedUpdate() 
    {
        if (!knockback)
        {

            horizontalMove = Input.GetAxis("Horizontal");
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

    private void UpdateAnimations()
    {
        anim.SetBool("isTookDamage", knockback);
        anim.SetBool("isRunning", IsRunning);
        anim.SetBool("isGrounded", IsOnTheGround);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isWallSliding", isWallSliding);
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
        
        isRight = !isRight;
        Vector3 temp = transform.localScale;
        temp.x *= -1;
        transform.localScale = temp;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(PositionOfFeet.position, DistanceToCheck);
        Gizmos.DrawWireSphere(WallCheck.position, wallCheckDistance);
    }
}
