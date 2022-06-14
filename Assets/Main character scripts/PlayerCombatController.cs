using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public Transform attack_1AttackPosition;
    public LayerMask damageableObject;
    public GameObject hitParticle;
    public float attack_1Radius, inputTimer;
    public int attack_1Damage;

    public bool combatEnabled;
    private bool gotInput, isAttacking, isFirstAttack;
    private float lastInputTime = Mathf.NegativeInfinity;
    private float[] attackDetails = new float[2];
    private int damageDirection;
    private PlayerController playerController;
    private PlayerHealth playerHealth;
    private Animator anim;
    private Rigidbody2D rb;

    [SerializeField] private AudioSource AttackSoundEffect;

    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnabled);
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        playerHealth = GetComponent<PlayerHealth>();
    }
    // Update is called once per frame
    void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }

    void CheckCombatInput()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (combatEnabled && playerController.canAttack())
            {
                AttackSoundEffect.Play();
                gotInput = true;
                lastInputTime = Time.time;
            }
        }
    }

    void CheckAttacks()
    {
        if(gotInput)
        {
            if (!isAttacking)
            {
                gotInput = false;
                isAttacking = true;
                isFirstAttack = !isFirstAttack;
                anim.SetBool("attack_1", true);
                anim.SetBool("firstAttack", isFirstAttack);
                anim.SetBool("isAttacking", isAttacking);
            }
        }

        if(Time.time >= lastInputTime + inputTimer)
        {
            gotInput = false;
        }
    }

    void Attack()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(attack_1AttackPosition.position, attack_1Radius, damageableObject);

        attackDetails[0] = attack_1Damage;
        attackDetails[1] = transform.position.x;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.SendMessage("Damage", attackDetails);
        }
    }

    void PlayerTakeDamage(float[] attackDetails)
    {
        Instantiate(hitParticle, rb.transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
        Debug.Log("Player taking damage: " + attackDetails[0] + "P: " + transform.position.x);

        float damage = attackDetails[0];
        //playerHealth.TakeDamage(damage);


        if (attackDetails[1] > transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        playerController.Knockback(damageDirection, attackDetails[1]);
    }

    void FinishAttack1()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack_1", false);
    }

    public bool isAttack()
    {
        return isAttacking;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack_1AttackPosition.position, attack_1Radius);
    }
}
