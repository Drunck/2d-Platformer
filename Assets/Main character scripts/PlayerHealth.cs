using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public bool isDead = false;
    public float MaxHealth;
    private bool isHurt;
    private float currentHealth;
    private Animator anim;
    
    void Start()
    {
        currentHealth = MaxHealth;
        anim = GetComponent<Animator>();
    }

   

    public void PLayerTakeDamage(float damage)
    {
        isHurt = true;
        currentHealth -= damage;
        if (currentHealth <= 0.0f)
        {
           anim.SetBool("Hurt", false);
           anim.SetBool("Die", true);
        }
    }


    public void TakeDamageOff()
    {
        anim.SetBool("Hurt", false);
    }

    public void Die()
    {
        isDead = true;
        Destroy(gameObject);
    }

    public void Update()
    {
        if (isHurt)
        {
            anim.SetBool("Hurt", true);
            isHurt = !isHurt;
        }
    }
}
