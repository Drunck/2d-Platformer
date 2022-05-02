using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    public Transform attack_1AttackPosition;
    public LayerMask damageableObject;
    public float attack_1Radius, inputTimer;
    public int minAttackDamage, maxAttackDamage;

    public bool combatEnabled;
    private bool gotInput, isAttacking, isFirstAttack;
    private float lastInputTime = Mathf.NegativeInfinity, attack_1Damage;
    private float[] attackDetails = new float[2];
    private Animator anim;


    void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("canAttack", combatEnabled);
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
            if(combatEnabled)
            {
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
        attack_1Damage = Random.Range(minAttackDamage, maxAttackDamage);

        attackDetails[0] = attack_1Damage;
        attackDetails[1] = transform.position.x;
        Debug.Log("Player attack damage:" + attack_1Damage);

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.SendMessage("Damage", attackDetails);
        }
    }

    void FinishAttack1()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", isAttacking);
        anim.SetBool("attack_1", false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attack_1AttackPosition.position, attack_1Radius);
    }
}
