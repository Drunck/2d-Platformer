using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Attack : MonoBehaviour
{
    private int damage;
    public int maxDamage;
    public int minDamage;


    private Animator anim;



    public Transform HitBox, RaycastPosition;
    public LayerMask playerLayer;
    public float attackDistance;
    public float attack_1Radius;
    private float[] attackDetails = new float[2];


    public float timer;
    private float intTimer; //initial value of timer
    private bool canAttack, cooling;

    public void Start()
    {
        intTimer = timer;
        anim = GetComponent<Animator>();
    }


    public void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(RaycastPosition.position, transform.right, attackDistance);
        if (hit.collider != null)
        {
            Debug.Log(hit.collider.name);

            if (hit.collider.name == "Player")
            {
                anim.SetBool("Attack", true);
            }
            else
            {
                anim.SetBool("Attack", false);
            }
        }

        if(timer<= 0)
        {
            anim.SetBool("Attack", true);
        } else
        {
            timer -= Time.deltaTime;
        }

    }

    public void Attack_Player()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(HitBox.position, attack_1Radius, playerLayer);
        timer = intTimer; //Reset timer
        damage = Random.Range(minDamage, maxDamage);

        attackDetails[0] = damage;
        attackDetails[1] = transform.position.x;
        Debug.Log(damage);

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.SendMessage("PLayerTakeDamage", attackDetails[0]);
            
        }
    }




    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(HitBox.position, attack_1Radius);
        Debug.DrawRay(RaycastPosition.position, Vector2.right * attackDistance, Color.red);
    }

   


}
